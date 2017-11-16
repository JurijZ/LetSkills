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
    public class GetProviderJobDetailsController : Controller
    {

        private readonly SkillsContext _context;

        public GetProviderJobDetailsController(SkillsContext context)
        {
            _context = context;
        }

        // GET api/getjobdetails/33
        [HttpGet("{job_id}")]
        [Authorize]
        public ProviderSingleJobDetails Get(int job_id)
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            var jwtuser = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            Console.WriteLine("Authenticated user name is: " + jwtuser.Value); //it's in a {key: value} format
            var userName = jwtuser.Value;

            Console.WriteLine("Requested Job ID: " + job_id);
            
            // retrieve image urls
            var imageUrls = (from i in _context.JobImages
                             where i.JobId == job_id
                             select i.Url).ToArray();
            
            // get job details (assigns the ImageUrls array into the Images property)
            var jobDetails = (from p in _context.Jobs
                              join e in _context.Users on p.ClientId equals e.Id
                              join d in _context.JobDetails on p.Id equals d.JobId into jd // this into jd is needed to do the LEFT JOIN
                                from jdd in jd.DefaultIfEmpty() // to do the LEFT JOIN DefaultIfEmpty() method is used
                              join s in _context.Skills on p.SkillId equals s.Id into js // to get Skill Name
                                from jss in js.DefaultIfEmpty()
                              where e.Username == userName && p.Id == job_id
                              select new ProviderSingleJobDetails
                              {
                                  id = p.Id,
                                  clientId = p.ClientId,
                                  jobTitle = p.JobTitle,
                                  //locationPostCode = p.LocationPostCode,
                                  ratePerHour = p.RatePerHour,
                                  rateFixed = p.RateFixed,
                                  durationDays = p.DurationDays,
                                  durationHours = p.DurationHours,
                                  contactTelephone1 = p.JobState == 40 ? p.ContactTelephone1 : "",  // Show number only when the job is in the Accepted state
                                  contactTelephone2 = p.JobState == 40 ? p.ContactTelephone2 : "",  // Show number only when the job is in the Accepted state
                                  //contactEmail = p.ContactEmail,
                                  jobState = p.JobState,
                                  plannedStartDate = p.PlannedStartDate,
                                  plannedFinishDate = p.PlannedFinishDate,
                                  locationLat = p.LocationLat,
                                  locationLng = p.LocationLng,

                                  skill = jss.Name,
                                  
                                  jobDescription = jdd.JobDescription,
                                  
                                  images = imageUrls
                              }).SingleOrDefault(); //.Take(1)


            //foreach (var jobDetail in jobDetails)
            //{
            //    Console.WriteLine("Job Title: " + jobDetail.jobTitle);
            //}

            return jobDetails;
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

    public class ProviderSingleJobDetails
    {
        public int id { get; set; }
        public int clientId { get; set; }
        public string jobTitle { get; set; }
        public decimal? ratePerHour { get; set; }
        public decimal? rateFixed { get; set; }
        public int? durationDays { get; set; }
        public int? durationHours { get; set; }
        //public string locationPostCode { get; set; }
        public string contactTelephone1 { get; set; }
        public string contactTelephone2 { get; set; }
        //public string contactEmail { get; set; }
        public int? jobState { get; set; }
        public DateTime? plannedStartDate { get; set; }
        public DateTime? plannedFinishDate { get; set; }
        public decimal? locationLat { get; set; }
        public decimal? locationLng { get; set; }
        public string skill { get; set; }

        public string jobDescription { get; set; }

        public string[] images { get; set; }
    }
}
