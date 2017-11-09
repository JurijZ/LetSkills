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
    [Route("api/[controller]")]
    public class GetClientLimitsController : Controller
    {

        private readonly SkillsContext _context;

        public GetClientLimitsController(SkillsContext context)
        {
            _context = context;
        }

        // GET api/myjobs
        [HttpGet]
        //[Authorize]
        public ClientLimits Get()
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            //var userName = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            //Console.WriteLine("Authenticated user name is: " + userName.Value); //it's in a JSON format - name: value

            Console.WriteLine("Requesting Client limits from the GetClientLimitsController");

            string userName = "jz@gmail.com"; // This value will be taken from the JWT claim

            // retrieve client details (max allowed active jobs field)
            var client = (from ucd in _context.UserClientDetails
                        join u in _context.Users on ucd.UserId equals u.Id
                        where u.Username == userName
                        select ucd).SingleOrDefault();

            // get the number of published Client Jobs 
            var publishedClientJobs = (from p in _context.Jobs
                                    join u in _context.Users on p.ClientId equals u.Id
                                    where u.Username == userName && p.JobState == 10
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
