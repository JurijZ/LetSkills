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
    public class GetClientLimitsController : Controller
    {

        private readonly SkillsContext _context;

        public GetClientLimitsController(SkillsContext context)
        {
            _context = context;
        }

        // GET api/getclientlimits
        [HttpGet]
        [Authorize]
        public ClientLimits Get()
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            var jwtuser = User?.Claims?.Where(x => x.Type == "name").FirstOrDefault();
            Console.WriteLine("Authenticated user name is: " + (jwtuser == null ? "unknown" : jwtuser.Value)); //it's in a {key: value} format
            
            // To make method unit testing simple 
            #if DEBUG
                Console.WriteLine("Debug mode (Unit Testing)"); 
                var userName = "support@letskills.com";                 
            #else
                var userName = jwtuser.Value; // value is taken from the JWT claim
            #endif

            Console.WriteLine($"Requesting Client {userName} limits from the GetClientLimitsController");
            
            // retrieve client details (max allowed active jobs field)
            var client = (from ucd in _context.UserClientDetails
                        join u in _context.Users on ucd.UserId equals u.Id
                        where u.Username == userName
                        select ucd).SingleOrDefault();

            // get the number of published Jobs by the Client
            var publishedClientJobs = (from p in _context.Jobs
                                      join u in _context.Users on p.ClientId equals u.Id
                                      where u.Username == userName && p.JobState == 10  // 10 - Published Job
                                      select p).Count();
            
            var availableNumberOfNewPublications = client.MaxJobsAllowed - publishedClientJobs;
            Console.WriteLine("Number of available publications: " + availableNumberOfNewPublications);

            var Limits = new ClientLimits() { allowed = true, maxjobsallowed = client.MaxJobsAllowed };
            if (availableNumberOfNewPublications < 1)
            {
                Limits.allowed = false;
                Limits.maxjobsallowed = client.MaxJobsAllowed;
            }            

            return Limits;
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

    public class ClientLimits
    {
        public bool? allowed { get; set; }
        public int? maxjobsallowed { get; set; }
    }
}
