using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LetSkillsBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Cors;

namespace LetSkillsBackend.Controllers
{
    [Route("[controller]")]
    //[EnableCors("default")]
    public class EditProviderProfileController : Controller
    {
        private readonly SkillsContext _context;

        public EditProviderProfileController(SkillsContext context)
        {
            _context = context;
        }
        
        // PUT api/editproviderprofile
        [HttpPut()]
        [Authorize]
        public ActionResult Put([FromBody] EditProviderProfile value)
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            var jwtuser = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            Console.WriteLine("Authenticated user name is: " + jwtuser.Value); //it's in a {key: value} format
            var userName = jwtuser.Value;

            Console.WriteLine("Updating Provider profile: " + userName);

            // Retrieve objects related to the authorized user 
            // (!!! it is assumed that all these tables are populated when the account was first created)
            var user = _context.Users.FirstOrDefault(j => j.Username == userName);
            var userProviderDetails = _context.UserProviderDetails.FirstOrDefault(upd => upd.UserId == user.Id);
            //var address = _context.Address.FirstOrDefault(i => i.UserId == user.Id); // this needs to consider 2 things: update new images and remove deleted, skipping this for now
            var newProviderSkills = _context.Skills.Where(s => value.skills.Contains(s.Name)); // select rows that match arrived skills
            var currentSkillIDs = _context.MapProviderSkills.Where(mps => mps.ProviderId == userProviderDetails.Id);

            newProviderSkills.ToList().ForEach(x => Console.WriteLine(x.Id));
            currentSkillIDs.ToList().ForEach(x => Console.WriteLine(x.SkillId));

            // Procede with update if the job id exist
            if (user != null)
            {
                // Update User record fields 
                user.Name = value.name;
                user.Surname = value.surname;
                _context.SaveChanges();

                // Update Provider Details (if it does not exist then create a new record)
                if (userProviderDetails != null)
                {
                    userProviderDetails.ContactEmail = value.contactEmail;
                    userProviderDetails.ContactTelephone1 = value.contactTelephone1;
                    userProviderDetails.ContactTelephone2 = value.contactTelephone2;
                    userProviderDetails.ProfileImageUrl = value.profileImageUrl;
                    userProviderDetails.HaveAcar = value.haveAcar;
                    userProviderDetails.LocationLat = value.locationLat;
                    userProviderDetails.LocationLng = value.locationLng;

                    _context.SaveChanges();
                }
                else
                {
                    var newUserProviderDetails = new UserProviderDetails
                    {
                        UserId = user.Id,
                        ContactEmail = value.contactEmail ?? "",
                        ContactTelephone1 = value.contactTelephone1 ?? "",
                        ContactTelephone2 = value.contactTelephone2 ?? "",
                        ProfileImageUrl = value.profileImageUrl ?? "https://s3.eu-west-2.amazonaws.com/images.angular4.test1/defaults/DefaultProfilePicture.PNG",
                        HaveAcar = value.haveAcar,
                        LocationLat = value.locationLat,
                        LocationLng = value.locationLng
                    };
                    _context.UserProviderDetails.Add(newUserProviderDetails);
                    _context.SaveChanges();
                }

                // Update Address (if it does not exist then create a new record)
                //if (address != null)
                //{
                //    address.City = value.city;
                //    address.AddressLine1 = value.addressLine1;
                //    address.AddressLine2 = value.addressLine2;
                //    address.PostCode = value.postCode;
                //    _context.SaveChanges();
                //}
                //else
                //{
                //    var newClientAddress = new Address
                //    {
                //        UserId = user.Id,
                //        City = value.city ?? "",
                //        AddressLine1 = value.addressLine1 ?? "",
                //        AddressLine2 = value.addressLine2 ?? "",
                //        PostCode = value.postCode ?? ""
                //    };
                //    _context.Address.Add(newClientAddress);
                //    _context.SaveChanges();
                //}

                // Update provider Skills (always overwrite)
                if (currentSkillIDs != null)
                {
                    // Deleting all existing
                    foreach (var currentSkillID in currentSkillIDs)
                    {
                        _context.MapProviderSkills.Remove(currentSkillID);
                    }
                    _context.SaveChanges();

                    // Creating new ones
                    foreach (var newProviderSkill in newProviderSkills)
                    {
                        var providerSkill = new MapProviderSkills
                        {
                            ProviderId = userProviderDetails.Id,
                            SkillId = newProviderSkill.Id
                        };
                        _context.MapProviderSkills.Add(providerSkill);
                    }                    
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
    
    public class EditProviderProfile
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
        //public DateTime? lastLoginTime { get; set; }

        // from UserClientDetails
        public string contactEmail { get; set; }
        public string contactTelephone1 { get; set; }
        public string contactTelephone2 { get; set; }
        public string profileImageUrl { get; set; }
        public bool? haveAcar { get; set; }
        public decimal? locationLat { get; set; }
        public decimal? locationLng { get; set; }

        // from Address
        //public string country { get; set; }
        //public string city { get; set; }
        //public string addressLine1 { get; set; }
        //public string addressLine2 { get; set; }
        //public string postCode { get; set; }

        // from MapProviderSkills/Skills
        public string[] skills { get; set; }
    }
}
