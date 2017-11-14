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
    public class CreateApplicationController : Controller
    {

        private readonly SkillsContext _context;

        public CreateApplicationController(SkillsContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public string Post([FromBody] NewApplication value)
        {

            Console.WriteLine("--Creating new Application for the JobID: " + value.JobId);
            string userName = "jz@gmail.com"; // This value will be taken from the JWT claim

            //var provider = _context.Users.FirstOrDefault(u => u.Username == userName);

            var provider = (from u in _context.Users
                             join upd in _context.UserProviderDetails on u.Id equals upd.UserId into jupd // this into jucd is needed to do the LEFT JOIN
                             from jjupd in jupd.DefaultIfEmpty()
                             where u.Username == userName
                             select new ProviderVerification
                             {
                                 // from Users
                                 Id = u.Id,
                                 UserName = u.Username,

                                 // from UserProviderDetails
                                 ContactTelephone1 = jjupd.ContactTelephone1,
                                 Telephone1Verified = jjupd.Telephone1Verified
                             }).SingleOrDefault();

            // Insert into the Applications table 
            if (provider != null)
            {
                // If provifers phone number is not verified then return an error
                if (provider.ContactTelephone1 != provider.Telephone1Verified) {
                    // Return error
                    Console.WriteLine("--Application is not possible hence Providers phone is not verified");
                    this.HttpContext.Response.StatusCode = 404;
                    return null;
                }

                var newApplication = new Applications
                {
                    JobId = value.JobId,  // Use Id value of the inserted record to the Jobs table
                    ProviderId = provider.Id
                };
                _context.Applications.Add(newApplication);
                _context.SaveChanges();

                //return jobid back to Angular (technically there is not much sense returning anything appart from Ok);
                return newApplication.JobId.ToString();
            }

            // Return error
            Console.WriteLine("--Application for the JobID " + value.JobId + " FAILED");
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

    public class NewApplication
    {
        public int JobId { get; set; }
        //public int ProviderId { get; set; }
    }

    public class ProviderVerification
    {
        public int Id;
        public string UserName;
        public string ContactTelephone1;
        public string Telephone1Verified;
    }
}
