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
using Microsoft.AspNetCore.Cors;
using System.Net.Http.Headers;

namespace skillsBackend.Controllers
{
    [Route("[controller]")]
    //[EnableCors("default")]
    public class SearchJobController : Controller
    {

        private readonly SkillsContext _context;

        public SearchJobController(SkillsContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IEnumerable<JobSearchResult> Post([FromBody] JobSearch jobSearch)
        {

            Console.WriteLine("--"      + jobSearch.Keywords + ", " + jobSearch.Duration
                                 + ", " + jobSearch.LocationLat + ", " + jobSearch.LocationLng
                                 + ", " + jobSearch.Zoom + ", " + jobSearch.Skill);

            // Get Provider ID
            string userName = "jz@gmail.com"; // This value will be taken from the JWT claim
            var provider = (from u in _context.Users
                            where u.Username == userName
                            select u).SingleOrDefault();

            // Get Search area out of the Map center coordinates and Zoom value
            var meters_per_pixel = 156543.03392 * Math.Cos((double)jobSearch.LocationLat * Math.PI / 180) / Math.Pow(2, (double)jobSearch.Zoom);
            var shiftLat = (decimal)(meters_per_pixel * 150 / 111300);
            var shiftLng = (decimal)(meters_per_pixel * 500 / 78701);
                
            // west/east x(meters)/78701
            // north/south y(meters)/111300
            Console.WriteLine(shiftLat + ", " + shiftLng );

            var downLocationLat = jobSearch.LocationLat - shiftLat;
            var upLocationLat = jobSearch.LocationLat + shiftLat;
            var leftLocationLng = jobSearch.LocationLng - shiftLng;
            var rightLocationLng = jobSearch.LocationLng + shiftLng;
            Console.WriteLine("South to North: " + downLocationLat + ", " + upLocationLat);
            Console.WriteLine("West to East: " + leftLocationLng + ", " + rightLocationLng);

            // FILTER by AREA
            var jobSearchResult = from j in _context.Jobs
                                join d in _context.JobDetails on j.Id equals d.JobId into jd // this into jd is needed to do the LEFT JOIN
                                    from jdd in jd.DefaultIfEmpty() // to do the LEFT JOIN DefaultIfEmpty() method is used
                                join s in _context.Skills on j.SkillId equals s.Id into js
                                    from jss in js.DefaultIfEmpty()
                                join a in _context.Applications on j.Id equals a.JobId into ja
                                    from jaa in ja.DefaultIfEmpty()
                                //where j.SkillId == ConvertSkillNameToSkillID(jobSearch.Skill)
                                  where j.LocationLat > downLocationLat &&
                                        j.LocationLat < upLocationLat &&
                                        j.LocationLng > leftLocationLng &&
                                        j.LocationLng < rightLocationLng &&
                                        (j.JobState == 10 ||     // Published
                                        j.JobState == 20)        // Offer Sent (in case offer will be rejected)
                                  select new JobSearchResult
                                {
                                    Id = j.Id,
                                    ClientId = j.ClientId,
                                    JobTitle = j.JobTitle,
                                    RatePerHour = j.RatePerHour,
                                    RateFixed = j.RateFixed,
                                    DurationDays = j.DurationDays,
                                    DurationHours = j.DurationHours,
                                    JobState = j.JobState,
                                    PlannedStartDate = j.PlannedStartDate,
                                    PlannedFinishDate = j.PlannedFinishDate,
                                    LocationLat = j.LocationLat,
                                    LocationLng = j.LocationLng,

                                    Skill = jss.Name,

                                    JobDescription = jdd.JobDescription,

                                    // This is rather inefficient, need to be changed
                                    Applied = IsUserAlreadyApplied(provider.Id, jaa.ProviderId),    // Finds if the user already applied to the job

                                    Images = (from i in _context.JobImages  // inserts an array of images
                                              where i.JobId == j.Id
                                              select i.Url).ToArray()
                                };

            // FILTER by Skill
            if (!string.IsNullOrEmpty(jobSearch.Skill))
            {
                Console.WriteLine("Skill filter activated, filtering for: " + jobSearch.Skill);
                //jobSearchResult = jobSearchResult.Where(s => ConvertSkillNameToSkillID(s.Skill) > ConvertSkillNameToSkillID(jobSearch.Skill));

                // If All is selected then show all results
                if (jobSearch.Skill != "All")
                {
                    jobSearchResult = jobSearchResult.Where(s => s.Skill == jobSearch.Skill);
                }                
            }

            // FILTER by Keywords
            if (!string.IsNullOrEmpty(jobSearch.Keywords))
            {
                Console.WriteLine("Keyword filter activated");
                jobSearchResult = jobSearchResult.Where(s => s.JobDescription.Contains(jobSearch.Keywords));
            }
            
            // Just a check if we retrieve any results at all
            if (jobSearchResult.Any())
            {
                Console.WriteLine("Number of Jobs returning: " + jobSearchResult.Count());
                //Console.WriteLine("Searched Job Title: " + jobSearchResult.FirstOrDefault().JobTitle);
            }
            else {
                Console.WriteLine("Search result is null");
            }

            // POST reply
            return jobSearchResult;
        }

        // Retrieves Skill ID by it's Name
        private int ConvertSkillNameToSkillID(string skill)
        {
            var jobSkill = _context.Skills.FirstOrDefault(s => s.Name == skill);

            if (jobSkill != null)
            {
                Console.WriteLine("Searched Skill ID: " + jobSkill.Id);
                return jobSkill.Id;
            }
            return 2; // 2 - General Purpose
        }

        // Retrieve application status of a particular user for a particular job
        private int? IsUserAlreadyApplied(int providerId, int? applicationProviderId)
        {
            Console.WriteLine("Checking application, Provider ID: " + applicationProviderId);

            // applicationProviderId can be number or null
            if (providerId == applicationProviderId)
            {
                return 1;
            }
            return null;
        }

        // GET api/joblocation/5
        //[HttpGet("{id}")]
        //public IEnumerable<string> Get(int id)
        //{
        //    var userName = _context.Users
        //        .Where(u => u.Id ==id )
        //        .Select(p => p.Name);

        //    return userName;
        //}

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


    public class JobSearch
    {
        public string Keywords { get; set; }
        //public decimal? RatePerHour { get; set; }
        //public decimal? RateFixed { get; set; }
        //public int? DurationDays { get; set; }
        //public int? DurationHours { get; set; }
        //public DateTime? PlannedStartDate { get; set; }
        //public DateTime? PlannedFinishDate { get; set; }
        public decimal? LocationLat { get; set; }
        public decimal? LocationLng { get; set; }
        //public int? Radius { get; set; }
        public int? Zoom { get; set; }
        public string Skill { get; set; }
        public int? Duration { get; set; }
    }

    public class JobSearchResult
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string JobTitle { get; set; }
        public decimal? RatePerHour { get; set; }
        public decimal? RateFixed { get; set; }
        public int? DurationDays { get; set; }
        public int? DurationHours { get; set; }
        public int? JobState { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedFinishDate { get; set; }
        public decimal? LocationLat { get; set; }
        public decimal? LocationLng { get; set; }

        public string Skill { get; set; }

        public string JobDescription { get; set; }

        public int? Applied { get; set; }

        public string[] Images { get; set; }
    }
}
