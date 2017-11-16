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

namespace skillsBackend.Controllers
{
    [Route("[controller]")]
    public class GetOfferProviderDetailsController : Controller
    {

        private readonly SkillsContext _context;

        public GetOfferProviderDetailsController(SkillsContext context)
        {
            _context = context;
        }

        // GET api/providerprofile
        [HttpGet("{provider_id}")]
        [Authorize]
        public OfferProviderDetails Get(int provider_id)
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            //var jwtuser = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            //Console.WriteLine("Authenticated user name is: " + jwtuser.Value); //it's in a {key: value} format
            //var userName = jwtuser.Value;

            Console.WriteLine("-- GetOfferProviderDetails for Provider ID: " + provider_id);
            
            var providerSkills = (from mps in _context.MapProviderSkills
                                  join s in _context.Skills on mps.SkillId equals s.Id
                                  where mps.ProviderId == provider_id
                                  select s.Name).ToArray();

            var providerProfile = (from u in _context.Users 
                                 join upd in _context.UserProviderDetails on u.Id equals upd.UserId into jupd // this into jucd is needed to do the LEFT JOIN
                                    from jjupd in jupd.DefaultIfEmpty() // to do the LEFT JOIN DefaultIfEmpty() method is used
                                 join a in _context.Address on u.Id equals a.UserId into ja
                                    from jaa in ja.DefaultIfEmpty()
                                 where u.Id == provider_id
                                 select new OfferProviderDetails
                                 {
                                    // from Users
                                    id = u.Id,
                                    name = u.Name,
                                    surname = u.Surname,
                                    //username = u.Username,
                                    //password = u.Password,
                                    //addressId = u.AddressId,
                                    //haveAcar = u.HaveAcar,
                                    //isClient = u.IsClient,
                                    //isProvider = u.IsProvider,
                                    //lastLoginTime = u.LastLoginTime,

                                     // from UserClientDetails
                                     //contactEmail = jjupd.ContactEmail,
                                     contactTelephone1 = jjupd.ContactTelephone1,
                                     contactTelephone2 = jjupd.ContactTelephone2,
                                     profileImageUrl = jjupd.ProfileImageUrl,
                                     haveAcar = jjupd.HaveAcar,
                                     locationLat = jjupd.LocationLat,
                                     locationLng = jjupd.LocationLng,
                                     rating = jjupd.Rating,
                                     
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

    public class OfferProviderDetails
    {
        // from Users
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        //public string username { get; set; }
        //public byte[] password { get; set; }
        //public string addressId { get; set; }
        //public bool? haveAcar { get; set; }
        //public bool? isClient { get; set; }
        //public bool? isProvider { get; set; }
        //public DateTime? lastLoginTime { get; set; }

        // from UserProviderDetails
        public string contactTelephone1 { get; set; }
        public string contactTelephone2 { get; set; }
        //public string contactEmail { get; set; }
        public string profileImageUrl { get; set; }
        public bool? haveAcar { get; set; }
        public decimal? locationLat { get; set; }
        public decimal? locationLng { get; set; }
        public byte? rating { get; set; }

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
