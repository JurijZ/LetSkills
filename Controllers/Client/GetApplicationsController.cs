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
    public class GetApplicationsController : Controller
    {

        private readonly SkillsContext _context;

        public GetApplicationsController(SkillsContext context)
        {
            _context = context;
        }

        // GET api/getproviders/33
        [HttpGet("{job_id}")]
        [Authorize]
        public async Task<IEnumerable<Providers>> Get(int job_id)
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            var jwtuser = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            Console.WriteLine("Authenticated user name is: " + jwtuser.Value); //it's in a {key: value} format
            var userName = jwtuser.Value;

            //var providerSkills = (from u in _context.Users
            //                      join upd in _context.UserProviderDetails on u.Id equals upd.UserId
            //                      join mps in _context.MapProviderSkills on upd.Id equals mps.ProviderId
            //                      join s in _context.Skills on mps.SkillId equals s.Id
            //                      where u.Username == userName
            //                      select s.Name).ToArray();

            // This is a security measure to make sure that only valid user can request Applications info
            var job = await (from j in _context.Jobs
                            join u in _context.Users on j.ClientId equals u.Id
                            where u.Username == userName && j.Id == job_id
                            select j ).SingleOrDefaultAsync();

            Console.WriteLine("-- GetApplications for the Job Id: " + job.Id);

            var providers = from a in _context.Applications
                             join u in _context.Users on a.ProviderId equals u.Id
                             join upd in _context.UserProviderDetails on u.Id equals upd.UserId
                             where a.JobId == job.Id
                             select new Providers
                                 {
                                    // from Users
                                    id = u.Id,
                                    name = u.Name,

                                    // from UserProviderDetails
                                    profileImageUrl = upd.ProfileImageUrl,
                                    rating = upd.Rating
                                     
                                    // from Skills
                                    //skills = providerSkills
                                 };
        
            return await providers.ToListAsync(); 
            //return providers
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

    public class Providers
    {
        // from Users
        public int id { get; set; }
        public string name { get; set; }
        //public string surname { get; set; }
        //public string username { get; set; }
        //public byte[] password { get; set; }
        //public string addressId { get; set; }
        //public bool? haveAcar { get; set; }
        //public bool? isClient { get; set; }
        //public bool? isProvider { get; set; }
        //public DateTime? lastLoginTime { get; set; }

        // from UserProviderDetails
        //public string contactTelephone1 { get; set; }
        //public string contactTelephone2 { get; set; }
        //public string contactEmail { get; set; }
        public string profileImageUrl { get; set; }
        //public bool? haveAcar { get; set; }
        //public decimal? locationLat { get; set; }
        //public decimal? locationLng { get; set; }
        public byte? rating { get; set; }

        // from Address
        //public string country { get; set; }
        //public string city { get; set; }
        //public string addressLine1 { get; set; }
        //public string addressLine2 { get; set; }
        //public string postCode { get; set; }

        // from MapProviderSkills/Skills
        //public string[] skills { get; set; }
    }
}
