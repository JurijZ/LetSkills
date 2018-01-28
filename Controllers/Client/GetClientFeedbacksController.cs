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
        [Authorize]
        public IEnumerable<ClientFeedbacks> Get(int provider_id)
        {
            // Note - userName is not used in this Controller, any client can look for the rovider feedbacks
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            //var jwtuser = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            //Console.WriteLine("Authenticated user name is: " + jwtuser.Value); //it's in a {key: value} format
            //var userName = jwtuser.Value;

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
