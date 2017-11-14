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
    public class CreateJobController : Controller
    {

        private readonly SkillsContext _context;

        public CreateJobController(SkillsContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public string Post([FromBody] NewJob value)
        {

            Console.WriteLine("--" + value.PlannedStartDate);
            Console.WriteLine("--" + value.PlannedFinishDate);

            // !!!!! NOTE - all the inserts must be in one SQL transaction
            // Insert into the Jobs table 
            var newJob = new Jobs
            {
                ClientId = 1,
                JobTitle = value.JobTitle,
                RatePerHour = value.RatePerHour,
                RateFixed = value.RateFixed,
                DurationDays = value.DurationDays,
                DurationHours = value.DurationHours,
                //LocationPostCode = value.LocationPostCode,
                ContactTelephone1 = value.ContactTelephone1,
                ContactTelephone2 = value.ContactTelephone2,
                //ContactEmail = value.ContactEmail,
                JobState = value.JobState, // Initial state just after creation
                PlannedStartDate = value.PlannedStartDate,
                PlannedFinishDate = value.PlannedFinishDate,
                LocationLat = value.LocationLat,
                LocationLng = value.LocationLng,
                SkillId = ConvertSkillNameToSkillID(value.Skill)
            };
            _context.Jobs.Add(newJob);
            _context.SaveChanges();

            // Insert into the JobDetails table
            var newJobDetails = new JobDetails
            {
                JobId = newJob.Id,  // Use Identity value of the inserted record to the Jobs table
                JobDescription = value.JobDescription
            };
            _context.JobDetails.Add(newJobDetails);
            _context.SaveChanges();

            // Insert into the Images table (not the most efficient way, fix later)
            if (value.Images != null)
            { 
                foreach (var image in value.Images)
                {
                    Console.WriteLine("--" + image);
                    var newImages = new JobImages
                    {
                        JobId = newJob.Id,  // Use Identity value of the inserted record to the Jobs table
                        Url = image
                    };
                    _context.JobImages.Add(newImages);
                    _context.SaveChanges();
                }
            }

            //return new job id back to Angular;
            return newJob.Id.ToString();

            // Return error
            //Console.WriteLine("--New Job creation FAILED");
            //this.HttpContext.Response.StatusCode = 400;
            //return null;
        }

        // Retrieves Skill ID by it's Name
        private int ConvertSkillNameToSkillID(string skill)
        {
            var jobSkill = _context.Skills.FirstOrDefault(s => s.Name == skill);

            if (jobSkill != null)
            {
                return jobSkill.Id;
            }
            return 16; // 16 - General
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


    public class NewJob
    {
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public decimal? RatePerHour { get; set; }
        public decimal? RateFixed { get; set; }
        public int? DurationDays { get; set; }
        public int? DurationHours { get; set; }
        public string LocationPostCode { get; set; }
        public string ContactTelephone1 { get; set; }
        public string ContactTelephone2 { get; set; }
        public string ContactEmail { get; set; }
        public int JobState { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedFinishDate { get; set; }
        public decimal? LocationLat { get; set; }
        public decimal? LocationLng { get; set; }
        public string Skill { get; set; }

        public string[] Images { get; set; }
    }
}
