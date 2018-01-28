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
    public class GetProviderJobsController : Controller
    {

        private readonly SkillsContext _context;

        public GetProviderJobsController(SkillsContext context)
        {
            _context = context;
        }

        // GET api/myjobs
        [HttpGet]
        [Authorize]
        public IEnumerable<ProviderJobs> Get()
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            var jwtuser = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            Console.WriteLine("Authenticated user name is: " + jwtuser.Value); //it's in a {key: value} format
            var userName = jwtuser.Value;

            Console.WriteLine("Requesting all Provider related jobs from the ProviderJobsController");
            
            // Retrieve Provider details
            var provider = _context.Users.FirstOrDefault(u => u.Username == userName);

            // Get all jobs linked to the Provider Id
            var allProviderJobs = from p in _context.Jobs
                              join a in _context.Applications on p.Id equals a.JobId
                              join d in _context.JobDetails on p.Id equals d.JobId into jd // this into jd is needed to do the LEFT JOIN
                                from jdd in jd.DefaultIfEmpty() // to do the LEFT JOIN DefaultIfEmpty() method is used
                              where a.ProviderId == provider.Id
                              select new ProviderJobs
                              {
                                  id = p.Id,
                                  clientId = p.ClientId,
                                  jobTitle = p.JobTitle,
                                  locationPostCode = p.LocationPostCode,
                                  ratePerHour = p.RatePerHour,
                                  rateFixed = p.RateFixed,
                                  durationDays = p.DurationDays,
                                  durationHours = p.DurationHours,
                                  jobState = p.JobState,
                                  plannedStartDate = p.PlannedStartDate,
                                  plannedFinishDate = p.PlannedFinishDate,
                                  locationLat = p.LocationLat,
                                  locationLng = p.LocationLng,
                                  
                                  jobDescription = jdd.JobDescription
                              };


            //foreach (var jobDetail in jobDetails)
            //{
            //    Console.WriteLine("Job Title: " + jobDetail.jobTitle);
            //}

            return allProviderJobs;
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

    public class ProviderJobs
    {
        public int id { get; set; }
        public int clientId { get; set; }
        public string jobTitle { get; set; }
        public string locationPostCode { get; set; }
        public decimal? ratePerHour { get; set; }
        public decimal? rateFixed { get; set; }
        public int? durationDays { get; set; }
        public int? durationHours { get; set; }
        public int? jobState { get; set; }
        public DateTime? plannedStartDate { get; set; }
        public DateTime? plannedFinishDate { get; set; }
        public decimal? locationLat { get; set; }
        public decimal? locationLng { get; set; }

        public string jobDescription { get; set; }
    }
}
