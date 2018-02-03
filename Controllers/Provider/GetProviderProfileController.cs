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

namespace LetSkillsBackend.Controllers
{
    [Route("[controller]")]
    public class GetProviderProfileController : Controller
    {
        private readonly SkillsContext _context;

        public GetProviderProfileController(SkillsContext context)
        {
            _context = context;
        }

        // GET api/providerprofile
        [HttpGet]
        [Authorize]
        public ProviderProfile Get()
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            var jwtuser = User?.Claims?.Where(x => x.Type == "name").FirstOrDefault();
            Console.WriteLine("Authenticated user name is: " + (jwtuser == null ? "unknown" : jwtuser.Value)); //it's in a {key: value} format
            
            // To make unit testing of the method simple 
            #if DEBUG
                Console.WriteLine("Debug mode (Unit Testing)"); 
                var userName = "support@letskills.com";                 
            #else
                var userName = jwtuser.Value; // value is taken from the JWT claim
            #endif

            Console.WriteLine("-- GetProviderProfile - for the user name: " + userName);

            // An empty enumerable will be returned if Provider has no skills
            // It's not a null, just an empty enumerable, so no NullReferenceException
            var providerSkills = (from u in _context.Users
                                  join upd in _context.UserProviderDetails on u.Id equals upd.UserId
                                  join mps in _context.MapProviderSkills on upd.Id equals mps.ProviderId
                                  join s in _context.Skills on mps.SkillId equals s.Id
                                  where u.Username == userName
                                  select s.Name).ToArray();

            var providerProfile = (from u in _context.Users 
                                 join upd in _context.UserProviderDetails on u.Id equals upd.UserId into jupd // this into jucd is needed to do the LEFT JOIN
                                    from jjupd in jupd.DefaultIfEmpty() // to do the LEFT JOIN DefaultIfEmpty() method is used
                                 join a in _context.Address on u.Id equals a.UserId into ja
                                    from jaa in ja.DefaultIfEmpty()
                                 where u.Username == userName
                                 select new ProviderProfile
                                 {
                                    // from Users
                                    id = u.Id,
                                    name = u.Name,
                                    surname = u.Surname,
                                    username = u.Username,
                                    //addressId = u.AddressId,
                                    //haveAcar = u.HaveAcar,
                                    //isClient = u.IsClient,
                                    //isProvider = u.IsProvider,
                                    //lastLoginTime = u.LastLoginTime,

                                     // from UserProviderDetails
                                     contactEmail = jjupd.ContactEmail,
                                     contactTelephone1 = jjupd.ContactTelephone1,
                                     contactTelephone2 = jjupd.ContactTelephone2,
                                     profileImageUrl = jjupd.ProfileImageUrl,
                                     haveAcar = jjupd.HaveAcar,
                                     locationLat = jjupd.LocationLat,
                                     locationLng = jjupd.LocationLng,
                                     telephone1Verified = jjupd.Telephone1Verified,

                                     // from Address
                                     //country = jaa.Country,
                                     //city = jaa.City,
                                     //addressLine1 = jaa.AddressLine1,
                                     //addressLine2 = jaa.AddressLine2,
                                     //postCode = jaa.PostCode,

                                     // from Skills
                                     skills = providerSkills
                                 }).SingleOrDefault();
        

            return providerProfile;
        }

        // POST api/getproviderprofile
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/getproviderprofile/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE api/getproviderprofile/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }

    public class ProviderProfile
    {
        // from Users
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string username { get; set; }
        //public string addressId { get; set; }
        //public bool? haveAcar { get; set; }
        //public bool? isClient { get; set; }
        //public bool? isProvider { get; set; }
        //public DateTime? lastLoginTime { get; set; }

        // from UserProviderDetails
        public string contactTelephone1 { get; set; }
        public string contactTelephone2 { get; set; }
        public string contactEmail { get; set; }
        public string profileImageUrl { get; set; }
        public bool? haveAcar { get; set; }
        public decimal? locationLat { get; set; }
        public decimal? locationLng { get; set; }
        public string telephone1Verified { get; set; }

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
