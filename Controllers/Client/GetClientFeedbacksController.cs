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
    public class GetClientFeedbacksController : Controller
    {
        private readonly SkillsContext _context;

        public GetClientFeedbacksController(SkillsContext context)
        {
            _context = context;
        }

        // Retrieve all feedbacks of a specific Provider
        // GET api/getclientfeedbacks/3
        [HttpGet("{provider_id}")]
        //[Authorize]
        public IEnumerable<ClientFeedbacks> Get(int provider_id)
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            //var userName = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            //Console.WriteLine("Authenticated user name is: " + userName.Value); //it's in a JSON format - name: value
            
            //string userName = "jz@gmail.com"; // This value will be taken from the JWT claim
            Console.WriteLine("-- GetClientFeedbacks for Provider ID: " + provider_id);            

            var clientFeedbacks = (from f in _context.FeedbackFromClients 
                                 join j in _context.Jobs on f.JobId equals j.Id
                                 where f.ProviderId == provider_id
                                 select new ClientFeedbacks
                                 {
                                     // from the FeedbackFromClients table
                                     feedBackDescription = f.FeedBackDescription,
                                     skills = f.Skills,
                                     communication = f.Communication,
                                     punctuality = f.Punctuality,
                                     feedbackTimestamp = f.FeedbackTimestamp,

                                     // from the Jobs table
                                     jobTitle = j.JobTitle
                                 });        

            return clientFeedbacks;
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

    public class ClientFeedbacks
    {
        // from the FeedbackFromClients table
        public string feedBackDescription { get; set; }
        public byte? skills { get; set; }
        public byte? communication { get; set; }
        public byte? punctuality { get; set; }
        public DateTime? feedbackTimestamp { get; set; }

        // from the Jobs table
        public string jobTitle { get; set; }
        
    }
}
