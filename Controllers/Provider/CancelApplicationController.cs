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
using Microsoft.AspNetCore.Cors;
using System.Net.Http.Headers;
using System.Net;

namespace LetSkillsBackend.Controllers
{
    [Route("[controller]")]
    //[EnableCors("default")]
    public class CancelApplicationController : Controller
    {
        private readonly SkillsContext _context;

        public CancelApplicationController(SkillsContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public string Post([FromBody] CancelApplication value)
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            var jwtuser = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            Console.WriteLine("Authenticated user name is: " + jwtuser.Value); //it's in a {key: value} format
            var userName = jwtuser.Value;
            
            Console.WriteLine("--Canceling an Application for the JobID: " + value.JobId);
            
            var provider = _context.Users.FirstOrDefault(u => u.Username == userName);
            var application = _context.Applications.Where(a => ( a.ProviderId == provider.Id )
                                                            && ( a.JobId == value.JobId )).FirstOrDefault();

            // Delete from the Applications table 
            if (provider != null && application != null)
            {
                _context.Applications.Remove(application);
                _context.SaveChanges();

                //return jobid back to Angular (technically there is not much sense returning anything appart from Ok);
                return application.JobId.ToString();
            }

            // Return error
            Console.WriteLine("--Application Cancelation for the JobID " + value.JobId + " FAILED");
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

    public class CancelApplication
    {
        public int JobId { get; set; }
        //public int ProviderId { get; set; }
    }
}
