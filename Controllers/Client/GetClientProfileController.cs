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
    public class GetClientProfileController : Controller
    {

        private readonly SkillsContext _context;

        public GetClientProfileController(SkillsContext context)
        {
            _context = context;
        }

        // GET api/clientprofile
        [HttpGet]
        [Authorize]
        public ClientProfile Get()
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            var jwtuser = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            Console.WriteLine("Authenticated user name is: " + jwtuser.Value); //it's in a {key: value} format
            var userName = jwtuser.Value;
            
            //string userName = "jzilcov@gmail.com"; // This value will be taken from the JWT claim
            //Console.WriteLine("-- GetClientProfile - Hardcoded user name: " + userName);
            
            var clientProfile = (from u in _context.Users 
                                 join ucd in _context.UserClientDetails on u.Id equals ucd.UserId into jucd // this into jucd is needed to do the LEFT JOIN
                                    from jjucd in jucd.DefaultIfEmpty() // to do the LEFT JOIN DefaultIfEmpty() method is used
                                 join a in _context.Address on u.Id equals a.UserId into ja
                                    from jaa in ja.DefaultIfEmpty()
                                 where u.Username == userName
                                 select new ClientProfile
                                 {
                                    // from Users
                                    id = u.Id,
                                    name = u.Name,
                                    surname = u.Surname,
                                    username = u.Username,
                                    //password = u.Password,
                                    //addressId = u.AddressId,
                                    //haveAcar = u.HaveAcar,
                                    //isClient = u.IsClient,
                                    //isProvider = u.IsProvider,
                                    lastLoginTime = u.LastLoginTime,

                                     // from UserClientDetails
                                     contactEmail = jjucd.ContactEmail,
                                     contactTelephone1 = jjucd.ContactTelephone1,
                                     contactTelephone2 = jjucd.ContactTelephone2,
                                     profileImageUrl = jjucd.ProfileImageUrl,

                                     // from Address
                                     country = jaa.Country,
                                     city = jaa.City,
                                     addressLine1 = jaa.AddressLine1,
                                     addressLine2 = jaa.AddressLine2,
                                     postCode = jaa.PostCode
                                 }).SingleOrDefault();
            
            //foreach (var jobDetail in jobDetails)
            //{
            //    Console.WriteLine("Job Title: " + jobDetail.jobTitle);
            //}

            return clientProfile;
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

    public class ClientProfile
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
        public string contactTelephone1 { get; set; }
        public string contactTelephone2 { get; set; }
        public string contactEmail { get; set; }
        public string profileImageUrl { get; set; }

        // from Address
        public string country { get; set; }
        public string city { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string postCode { get; set; }
    }
}
