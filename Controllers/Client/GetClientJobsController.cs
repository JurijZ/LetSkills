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
    public class GetClientJobsController : Controller
    {

        private readonly SkillsContext _context;

        public GetClientJobsController(SkillsContext context)
        {
            _context = context;
        }

        // GET api/myjobs
        [HttpGet]
        //[Authorize]
        public IEnumerable<ClientJobs> Get()
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            //var userName = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            //Console.WriteLine("Authenticated user name is: " + userName.Value); //it's in a JSON format - name: value

            Console.WriteLine("Requesting all Client jobs from the GetClientJobsController");

            string userName = "jz@gmail.com"; // This value will be taken from the JWT claim

            // retrieve image urls
            //var imageUrls = (from i in _context.JobImages
            //                 where i.JobId == job_id
            //                 select i.Url).ToArray();

            // get all jobs linked to the user name
            var allClientJobs = from p in _context.Jobs
                                join e in _context.Users on p.ClientId equals e.Id
                                join d in _context.JobDetails on p.Id equals d.JobId into jd // this into jd is needed to do the LEFT JOIN
                                from jdd in jd.DefaultIfEmpty() // to do the LEFT JOIN DefaultIfEmpty() method is used
                                where e.Username == userName
                                select new ClientJobs
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

                                    jobDescription = jdd.JobDescription,

                                    numberOfApplications = (from a in _context.Applications  // Count number of applications
                                                            where a.JobId == p.Id
                                                            select a).Count()
                              };


            //foreach (var jobDetail in jobDetails)
            //{
            //    Console.WriteLine("Job Title: " + jobDetail.jobTitle);
            //}

            return allClientJobs;
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

    public class ClientJobs
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

        public int? numberOfApplications { get; set; }
    }
}
