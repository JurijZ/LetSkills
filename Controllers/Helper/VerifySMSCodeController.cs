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

namespace skillsBackend.Controllers
{
    [Route("[controller]")]
    //[EnableCors("default")]
    public class VerifySMSCodeController : Controller
    {
        private readonly SkillsContext _context;

        public VerifySMSCodeController(SkillsContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public string Post([FromBody] SMSCode value)
        {
            Console.WriteLine("--Verify SMS Code: " + value.Code + " for the telephone " + value.TelNumber);
            string userName = "jz@gmail.com"; // This value will be taken from the JWT claim
            var user = _context.Users.FirstOrDefault(j => j.Username == userName);

            // Check if there is a code for the specified telephone number
            var smsVerification = _context.SMSVerifications
                                        .Where(s => s.SMSCode == value.Code)
                                        .Where(s => s.TelephoneNumber == value.TelNumber)
                                        .Where(s => s.SMSVerified == null).SingleOrDefault();
            
            // Update SMSVerifications and Jobs table recods
            if (smsVerification != null)
            {
                Console.WriteLine("SMS code is correct");

                // Update verification columns in the SMSVerifications table
                smsVerification.SMSVerified = true;
                smsVerification.SMSVerifiedTime = DateTime.Now;

                // Depending on ObjectIdType update relevant table (0 - Jobs, 1 - UserProviderDetails, 2 - UserClientDetails)
                // Update Jobs table record
                if (smsVerification.ObjectIdType == 0) {
                    var job = _context.Jobs
                                    .Where(j => j.Id == smsVerification.ObjectId)
                                    .Where(j => j.ClientId == user.Id).SingleOrDefault();

                    job.Telephone1Verified = job.ContactTelephone1;
                }

                // Update UserProviderDetails table record
                if (smsVerification.ObjectIdType == 1)
                {
                    var provider = _context.UserProviderDetails
                                    .Where(upd => upd.Id == smsVerification.ObjectId).SingleOrDefault();

                    provider.Telephone1Verified = provider.ContactTelephone1;
                }

                // Update UserClientDetails table record - yet to be implemented !!!

                _context.SaveChanges();

                //technically there is not much sense returning anything appart of Ok;
                return "SMS code is correct";
            }

            // Return error
            Console.WriteLine("--SMS code " + value.Code + " verification FAILED");
            
            this.HttpContext.Response.StatusCode = 404;
            return null;
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

    public class SMSCode
    {
        public string Code { get; set; }
        public string TelNumber { get; set; }
    }
}
