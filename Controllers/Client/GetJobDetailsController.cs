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
    public class GetJobDetailsController : Controller
    {

        private readonly SkillsContext _context;

        public GetJobDetailsController(SkillsContext context)
        {
            _context = context;
        }

        // GET api/getjobdetails/33
        [HttpGet("{job_id}")]
        //[Authorize]
        public SingleJobDetails Get(int job_id)
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            //var userName = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            //Console.WriteLine("Authenticated user name is: " + userName.Value); //it's in a JSON format - name: value

            Console.WriteLine("Requested Job ID: " + job_id);

            string userName = "jz@gmail.com"; // This value will be taken from the JWT claim

            // retrieve image urls
            var imageUrls = (from i in _context.JobImages
                             where i.JobId == job_id
                             select i.Url).ToArray();
            
            // get job details (assigns the ImageUrls array into the Images property)
            var jobDetails = (from j in _context.Jobs
                              join u in _context.Users on j.ClientId equals u.Id
                              join d in _context.JobDetails on j.Id equals d.JobId into jd // this into jd is needed to do the LEFT JOIN
                                from jdd in jd.DefaultIfEmpty() // to do the LEFT JOIN DefaultIfEmpty() method is used
                              join s in _context.Skills on j.SkillId equals s.Id into js // to get Skill Name
                                from jss in js.DefaultIfEmpty()
                              where u.Username == userName && j.Id == job_id
                              select new SingleJobDetails
                              {
                                  id = j.Id,
                                  clientId = j.ClientId,
                                  jobTitle = j.JobTitle,
                                  //locationPostCode = j.LocationPostCode,
                                  ratePerHour = j.RatePerHour,
                                  rateFixed = j.RateFixed,
                                  durationDays = j.DurationDays,
                                  durationHours = j.DurationHours,
                                  contactTelephone1 = j.ContactTelephone1,  
                                  contactTelephone2 = j.ContactTelephone2,  
                                  //contactEmail = j.ContactEmail,
                                  jobState = j.JobState,
                                  plannedStartDate = j.PlannedStartDate,
                                  plannedFinishDate = j.PlannedFinishDate,
                                  locationLat = j.LocationLat,
                                  locationLng = j.LocationLng,
                                  telephone1Verified = j.Telephone1Verified,

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

    public class SingleJobDetails
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
        public string telephone1Verified { get; set; }

        public string jobDescription { get; set; }

        public string[] images { get; set; }
    }
}
