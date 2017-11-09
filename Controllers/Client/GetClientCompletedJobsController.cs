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
    public class GetClientCompletedJobsController : Controller
    {
        private readonly SkillsContext _context;

        public GetClientCompletedJobsController(SkillsContext context)
        {
            _context = context;
        }

        // Retrieve all feedbacks of a specific Provider
        // GET api/getclientfeedbacks/3
        [HttpGet]
        //[Authorize]
        public IEnumerable<ClientCompletedJobs> Get()
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            //var userName = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            //Console.WriteLine("Authenticated user name is: " + userName.Value); //it's in a JSON format - name: value
            
            string userName = "jz@gmail.com"; // This value will be taken from the JWT claim
            var user = _context.Users.FirstOrDefault(j => j.Username == userName);

            Console.WriteLine("-- GetClientCompletedJobs of the client ID: " + user.Id);            
            
            var clientCompletedJobs = from j in _context.Jobs
                                      join o in _context.Offers on j.Id equals o.JobId
                                      join u in _context.Users on o.ProviderId equals u.Id
                                      join upd in _context.UserProviderDetails on o.ProviderId equals upd.UserId
                                      join ffc in _context.FeedbackFromClients on o.JobId equals ffc.JobId into jffc // this into jucd is needed to do the LEFT JOIN
                                        from jjffc in jffc.DefaultIfEmpty() // to do the LEFT JOIN DefaultIfEmpty() method is used
                                      where j.ClientId == user.Id && j.JobState == 50 && o.AcceptanceStatus == 1
                                        select new ClientCompletedJobs
                                        {
                                            providerId = u.Id,
                                            providerName = u.Name,
                                            providerProfileImageUrl = upd.ProfileImageUrl,
                                            clientId = j.ClientId,
                                            jobId = j.Id,
                                            jobTitle = j.JobTitle,
                                            jobState = j.JobState,
                                            plannedStartDate = j.PlannedStartDate,
                                            plannedFinishDate = j.PlannedFinishDate,
                                            feedbackSubmited = jjffc.JobId
                                        };
            
            return clientCompletedJobs;
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

    public class ClientCompletedJobs
    {
        // from the FeedbackFromClients table
        public int? providerId { get; set; }
        public string providerName { get; set; }
        public string providerProfileImageUrl { get; set; }
        public int? clientId { get; set; }
        public int? jobId { get; set; }
        public string jobTitle { get; set; }
        public int? jobState { get; set; }
        public DateTime? plannedStartDate { get; set; }
        public DateTime? plannedFinishDate { get; set; }
        public int? feedbackSubmited { get; set; }
    }
}
