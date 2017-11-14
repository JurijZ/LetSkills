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
using System.Net;

namespace skillsBackend.Controllers
{
    [Route("[controller]")]
    //[EnableCors("default")]
    public class CreateClientFeedbackController : Controller
    {

        private readonly SkillsContext _context;

        public CreateClientFeedbackController(SkillsContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public string Post([FromBody] NewClientFeedback value)
        {
            Console.WriteLine("--Creating new Feedback for the provider: " + value.ProviderId);
            string userName = "jz@gmail.com"; // This value will be taken from the JWT claim

            var client = _context.Users.FirstOrDefault(u => u.Username == userName);

            // Insert into the FedbackFromClients table 
            if (client != null)
            {
                var newClientFeedback = new FeedbackFromClients
                {
                    JobId = value.JobId,
                    ProviderId = value.ProviderId,
                    FeedBackDescription = value.FeedBackDescription,
                    Skills = value.Skills,
                    Communication = value.Communication,
                    Punctuality = value.Punctuality
                };
                _context.FeedbackFromClients.Add(newClientFeedback);
                _context.SaveChanges();

                //return jobid back to Angular (technically there is not much sense returning anything appart from Ok);
                return newClientFeedback.JobId.ToString();
            }

            // Return error
            Console.WriteLine("--Feedback creation for the Provider ID " + value.ProviderId + " FAILED");
            this.HttpContext.Response.StatusCode = 404;
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

    public class NewClientFeedback
    {
        public int ProviderId { get; set; }
        public int JobId { get; set; }
        public string FeedBackDescription { get; set; }
        public byte Skills { get; set; }
        public byte Communication { get; set; }
        public byte Punctuality { get; set; }
    }
}
