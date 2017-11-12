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

namespace skillsBackend.Controllers
{
    [Route("[controller]")]
    //[EnableCors("default")]
    public class EditClientProfileController : Controller
    {
        private readonly SkillsContext _context;

        public EditClientProfileController(SkillsContext context)
        {
            _context = context;
        }
        
        // PUT api/editclientprofile
        [HttpPut()]
        //[Authorize]
        public ActionResult Put([FromBody] EditClientProfile value)
        {

            string userName = "jz@gmail.com"; // This value will be taken from the JWT claim
            Console.WriteLine("Updating Client profile: " + userName);

            // Retrieve objects related to the authorized user 
            // (!!! it is assumed that all these tables are populated when the account was first created)
            var user = _context.Users.FirstOrDefault(j => j.Username == userName);
            var userClientDetails = _context.UserClientDetails.FirstOrDefault(jd => jd.UserId == user.Id);
            var address = _context.Address.FirstOrDefault(i => i.UserId == user.Id); // this needs to consider 2 things: update new images and remove deleted, skipping this for now
            
            // Procede with update if the job id exist
            if (user != null)
            {
                // Update User record fields 
                user.Name = value.name;
                user.Surname = value.surname;
                _context.SaveChanges();

                // Update Client Details (if it does not exist then create new record)
                if (userClientDetails != null)
                { 
                    userClientDetails.ContactEmail = value.contactEmail;
                    userClientDetails.ContactTelephone1 = value.contactTelephone1;
                    userClientDetails.ContactTelephone2 = value.contactTelephone2;
                    userClientDetails.ProfileImageUrl = value.profileImageUrl;
                    _context.SaveChanges();
                }
                else
                {
                    var newUserClientDetails = new UserClientDetails
                    {
                        UserId = user.Id,
                        ContactEmail = value.contactEmail ?? "",
                        ContactTelephone1 = value.contactTelephone1 ?? "",
                        ContactTelephone2 = value.contactTelephone2 ?? "",
                        ProfileImageUrl = value.profileImageUrl ?? "https://s3.eu-west-2.amazonaws.com/images.angular4.test1/defaults/DefaultProfilePicture.PNG"
                    };
                    _context.UserClientDetails.Add(newUserClientDetails);
                    _context.SaveChanges();
                }

                // Update Address 
                if (address != null)
                {
                    address.City = value.city;
                    address.AddressLine1 = value.addressLine1;
                    address.AddressLine2 = value.addressLine2;
                    address.PostCode = value.postCode;
                    _context.SaveChanges();
                }
                else
                {
                    var newClientAddress = new Address
                    {
                        UserId = user.Id,
                        City = value.city ?? "",
                        AddressLine1 = value.addressLine1 ?? "",
                        AddressLine2 = value.addressLine2 ?? "",
                        PostCode = value.postCode ?? ""
                    };
                    _context.Address.Add(newClientAddress);
                    _context.SaveChanges();
                }
            }
            else
            {
                Console.WriteLine("User: " + userName + " does not exist");
                return NotFound();
            }
            //return new job id back to Angular;
            //return existingJob.Id.ToString();
            return Ok();
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

        
        // DELETE api/joblocation/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
    
    public class EditClientProfile
    {
        // from Users
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string username { get; set; }
        //public byte[] password { get; set; }
        //public string addressId { get; set; }
        //public bool? haveAcar { get; set; }
        //public bool? isClient { get; set; }
        //public bool? isProvider { get; set; }
        public DateTime? lastLoginTime { get; set; }

        // from UserClientDetails
        public string contactEmail { get; set; }
        public string contactTelephone1 { get; set; }
        public string contactTelephone2 { get; set; }
        public string profileImageUrl { get; set; }

        // from Address
        public string country { get; set; }
        public string city { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string postCode { get; set; }
    }
}
