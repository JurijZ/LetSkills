using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using skillsBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Cors;
using System.Net.Http.Headers;
using System.Net;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace skillsBackend.Controllers
{
    [Route("[controller]")]
    //[EnableCors("default")]
    public class SendTelVerificationSMSController : Controller
    {
        private readonly SkillsContext _context;
        IAmazonSimpleNotificationService _smsClient { get; set; }

        public SendTelVerificationSMSController(SkillsContext context, IAmazonSimpleNotificationService smsClient)
        {
            _context = context;
            _smsClient = smsClient;
        }

        [HttpPost]
        [Authorize]
        public string Post([FromBody] TelephoneNumber value)
        {
            Console.WriteLine("--Sending SMS to verify telephone: " + value.TelNumber 
                                + " for the ObjectId: " + value.ObjectId            // Job Id or User ID
                                + " of the ObjectIdType: " + value.ObjectIdType);   // 0 - Job, 1 - Provider, 2 - Client???

            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            var jwtuser = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            Console.WriteLine("Authenticated user name is: " + jwtuser.Value); //it's in a {key: value} format
            var userName = jwtuser.Value;

            //-------------------- Sending SMS via Amazon SNS
            var smsCode = GenerateRandomPIN();                                  // Generate random 4 digits pin
            var SMSSentStatus = SendSMSviaAmazonSNS(smsCode, CleanUpTelephoneNumber(value.TelNumber));  // Send via Amazon SNS

            Console.WriteLine("SMS sending status: " + SMSSentStatus.Result);


            //-------------------- SMSVerifications table Maintenance -------------------
            // Loggin of these deletes needs to be implemented to have visibility of failures
            // Every time Verification is requested this code deletes preveous code requests
            var smsVerificationsInvalid = _context.SMSVerifications
                                        .Where(s => s.SMSVerified == null)
                                        .Where(s => s.ObjectId == value.ObjectId)       // Job Id or User ID
                                        .Where(s => s.TelephoneNumber == value.TelNumber);

            if (smsVerificationsInvalid.Any())
            {
                foreach (var smsVerificationInvalid in smsVerificationsInvalid)
                {
                    Console.WriteLine("Deleting invalid SMS verification: " + smsVerificationInvalid.Id);
                    _context.SMSVerifications.Remove(smsVerificationInvalid);
                }
            }

            // Check if there are expired codes (older then 5 min) for any telephone number
            // This is to cover the case when user requests the code but never finishes the validation
            var smsVerificationsExpired = _context.SMSVerifications
                                            .Where(s => s.SMSSentTime <= DateTime.Now.AddMinutes(-5))
                                            .Where(s => s.SMSVerified == null);
            
            if (smsVerificationsExpired.Any())
            {
                foreach (var smsVerificationExpired in smsVerificationsExpired)
                {
                    Console.WriteLine("Deleting expired SMS verifications: " + smsVerificationExpired.Id);
                    _context.SMSVerifications.Remove(smsVerificationExpired);
                }
            }
            //---------------------------------------

            // Insert into the SMSVerifications table 
            if (SMSSentStatus.Result)
            {
                // Insert verification record into the database
                var smsVerification = new SMSVerifications
                {
                    Username = userName,
                    ObjectId = value.ObjectId,
                    ObjectIdType = value.ObjectIdType,
                    TelephoneNumber = value.TelNumber,
                    SMSCode = smsCode,
                    SMSSentTime = DateTime.Now
                };
                _context.SMSVerifications.Add(smsVerification);
                _context.SaveChanges();

                //technically there is not much sense returning anything appart of Ok;
                return "Verification SMS Sent";
            }

            // Return error
            Console.WriteLine("--SMS sending for the number " + value.TelNumber + " FAILED");
            this.HttpContext.Response.StatusCode = 404;
            return null;
        }

        public string CleanUpTelephoneNumber(string mobileNumber)
        {
            Console.WriteLine("Number Before: " + mobileNumber);

            // Remove any braces
            mobileNumber = mobileNumber.Replace("(", string.Empty).Replace(")", string.Empty);
            mobileNumber = mobileNumber.Replace("[", string.Empty).Replace("]", string.Empty);

            // Remove leading plus and any minuses (in the UK SNS uses numbers without leading plus)
            mobileNumber = mobileNumber.Replace("+", string.Empty).Replace("-", string.Empty);
            
            // Remove spaces
            mobileNumber = mobileNumber.Replace(@"\s+", string.Empty);

            Console.WriteLine("Number After: " + mobileNumber);

            return mobileNumber;
        }

        public string GenerateRandomPIN()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max).ToString();
        }

        public static async Task<bool> SendSMSviaAmazonSNS(string code, string telephone)
        {
            try
            {
                // EU (Ireland):	eu-west-1
                using (var SMSclient = new AmazonSimpleNotificationServiceClient(Amazon.RegionEndpoint.EUWest1))
                {
                    // Prepare Message Attributes
                    var smsAttributes = new Dictionary<String, MessageAttributeValue>();
                    smsAttributes.Add("AWS.SNS.SMS.SenderID", new MessageAttributeValue() {
                        StringValue = "LetSkills", // 11 alphanumerical characters (no spaces) shown on the device
                        DataType = "String"
                    });
                    smsAttributes.Add("AWS.SNS.SMS.SMSType", new MessageAttributeValue() {
                        StringValue = "Promotional",    // Promotional (default) or Transactional
                        DataType = "String"
                    });
                    //smsAttributes.Add("AWS.SNS.SMS.MaxPrice",  new MessageAttributeValue() {
                    //    StringValue = "0.50",
                    //    DataType = "Number"
                    //});

                    var snsRequest = new PublishRequest()
                    {
                        PhoneNumber = telephone,   // plus is not needed for the UK numbers
                        Message = code,
                        MessageAttributes = smsAttributes
                    };

                    //var snsResponse = new PublishResponse();
                    var snsResponse = await SMSclient.PublishAsync(snsRequest);
                    
                    Console.WriteLine("SNS Service Response: " + snsResponse.MessageId);
                    Console.WriteLine("SNS Service Response: " + snsResponse.HttpStatusCode);
                    Console.WriteLine("SNS Service Response: " + snsResponse.ResponseMetadata);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in SendSMSviaAmazonSNS method: " + ex.Message);
                return false;
            }
        }

        // GET api/joblocation/5
        //[HttpGet("{id}")]
        //public IEnumerable<string> Get(int id)
        //{
        //    var userName = _context.Users
        //        .Where(u => u.Id ==id )
        //        .Select(p => p.Name);

        //    return userName;
        //}

        // POST api/joblocation
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/joblocation/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE api/joblocation/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }

    public class TelephoneNumber
    {
        public string TelNumber { get; set; }
        public int ObjectId { get; set; }
        public int ObjectIdType { get; set; }
    }
}
