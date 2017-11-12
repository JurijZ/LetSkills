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
    [Route("[controller]")]
    //[EnableCors("default")]
    public class UpdateJobStateController : Controller
    {

        private readonly SkillsContext _context;

        public UpdateJobStateController(SkillsContext context)
        {
            _context = context;
        }

        // PUT api/editjob/5
        [HttpPut("{job_id}")]
        //[Authorize]
        public ActionResult Put(int job_id, [FromBody] JobStateUpdate value)
        {
            Console.WriteLine("Updating Job State, Job ID: " + job_id);

            string userName = "jz@gmail.com"; // This value will be taken from the JWT claim

            // Retrieve job object related to the received id
            var job = (from p in _context.Jobs
                       join e in _context.Users on p.ClientId equals e.Id
                       where e.Username == userName && p.Id == job_id
                       select p).SingleOrDefault();
            
            //var job = _context.Jobs.FirstOrDefault(j => j.Id == id);            

            Console.WriteLine("New job state: " + value.jobState);

            // Procede with update if the job id exist
            if (job != null)
            {
                // Update JobState fields 
                job.JobState = value.jobState;
                _context.SaveChanges();
                
            }
            else
            {
                Console.WriteLine("Job ID: " + job_id + " does not exist");
                return NotFound();
            }

            // Now we need to delete all applications related to this job
            var currentApplications = _context.Applications.Where(a => a.JobId == job_id);

            foreach (var application in currentApplications)
            {
                _context.Applications.Remove(application);
            }
            _context.SaveChanges();


            //return new job id back to Angular;
            //return existingJob.Id.ToString();
            return Ok();
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

    public class JobStateUpdate
    {
        public int? jobState { get; set; }
    }
}
