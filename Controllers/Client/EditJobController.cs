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

namespace skillsBackend.Controllers
{
    [Route("api/[controller]")]
    //[EnableCors("default")]
    public class EditJobController : Controller
    {
        private readonly SkillsContext _context;

        public EditJobController(SkillsContext context)
        {
            _context = context;
        }

        // PUT api/editjob/5
        [HttpPut("{id}")]
        //[Authorize]
        public ActionResult Put(int id, [FromBody] JobUpdate value)
        {

            Console.WriteLine("Updating existing Job: " + id);
            
            // Retrieve objects related to the received id
            var job = _context.Jobs.FirstOrDefault(j => j.Id == id);
            var jobDetails = _context.JobDetails.FirstOrDefault(jd => jd.JobId == id);
            var oldImages = _context.JobImages.Where(i => i.JobId == id); // this needs to consider 2 things: update new images and remove deleted, skipping this for now

            Console.WriteLine("New job title: " + value.jobTitle);

            // Procede with update if the job id exist
            if (job != null)
            {
                // Update Job record fields 
                job.JobTitle = value.jobTitle;
                job.RatePerHour = value.ratePerHour;
                job.RateFixed = value.rateFixed;
                job.DurationDays = value.durationDays;
                job.DurationHours = value.durationHours;
                //job.LocationPostCode = value.locationPostCode;
                job.ContactTelephone1 = value.contactTelephone1;
                job.ContactTelephone2 = value.contactTelephone2;
                //job.ContactEmail = value.contactEmail;
                job.JobState = value.jobState;
                job.PlannedStartDate = value.plannedStartDate;
                job.PlannedFinishDate = value.plannedFinishDate;
                job.LocationLat = value.locationLat;
                job.LocationLng = value.locationLng;
                job.SkillId = ConvertSkillNameToSkillID(value.skill);
                _context.SaveChanges();

                // Update JobDetails record fields
                jobDetails.JobDescription = value.jobDescription;
                _context.SaveChanges();

                // Update Images records
                if (value.images.Length != 0)
                {
                    Console.WriteLine("Number of images in the array: " + value.images.Length);

                    // Delete existing links from the database
                    if (oldImages != null) {
                        foreach (var oldImage in oldImages)
                        {
                            _context.JobImages.Remove(oldImage);
                        }
                        _context.SaveChanges();
                    }

                    // Create new links of the latest images
                    foreach (var image in value.images)
                    {
                        Console.WriteLine("-- New image: " + image);
                        var newImages = new JobImages
                        {
                            JobId = job.Id,  // Use Identity value of the inserted record to the Jobs table
                            Url = image
                        };
                        _context.JobImages.Add(newImages);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    Console.WriteLine("No images were sent in the POST");
                }
            }
            else
            {
                Console.WriteLine("Job ID: " + id + " does not exist");
                return NotFound();
            }
            //return new job id back to Angular;
            //return existingJob.Id.ToString();
            return Ok();
        }

        // Retrieves Skill ID by it's Name
        private int ConvertSkillNameToSkillID(string skill)
        {
            var jobSkill = _context.Skills.FirstOrDefault(s => s.Name == skill);

            if (jobSkill != null)
            {
                return jobSkill.Id;
            }
            return 10; // 10 - General Skill
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


        // DELETE api/joblocation/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }

    public class JobUpdate
    {
        //public int id { get; set; }
        //public int clientid { get; set; }
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
