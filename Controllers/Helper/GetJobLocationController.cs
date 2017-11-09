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
    public class GetJobLocationController : Controller
    {

        private readonly SkillsContext _context;

        public GetJobLocationController(SkillsContext context)
        {
            _context = context;
        }
 
        // GET api/joblocation
        [HttpGet]
        [Authorize]
        public IEnumerable<JobLocation> Get()
        {
            
            Console.WriteLine("--- Get() method was called from the JobLocation Controller ---");

            // to get the Access_token manually
            //var h = Request.Headers["Authorization"];
            //Console.WriteLine(h);

            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            var userName = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            Console.WriteLine("Authenticated user name is: " + userName.Value); //it's in a JSON format - name: value

            // get jobs
            var jobLocation = _context.Jobs.Select(c => new JobLocation { LocationLat = c.LocationLat, LocationLng = c.LocationLng });

            // get jobs that are linked to the user requesting this information
            var jobLocation2 = (from p in _context.Jobs
                                join e in _context.Users
                                on p.ClientId equals e.Id
                                where e.Username == userName.Value
                                select new JobLocation { LocationLat = p.LocationLat, LocationLng = p.LocationLng });


            // if empty
            //if (!jobLocation2.Any())
            //{
            //    Console.WriteLine("Getting the Default value");

            //    // Default value
            //    List <JobLocation> jobLocation3 = new List<JobLocation> {
            //        new JobLocation { LocationLat = 51.544077m, LocationLng = 0.0014013m }
            //    };
            //    return jobLocation3;
            //}

            return jobLocation2;
        }

        // GET api/joblocation/5
        [HttpGet("{id}")]
        public IEnumerable<string> Get(int id)
        {
            var userName = _context.Users
                .Where(u => u.Id ==id )
                .Select(p => p.Name);

            return userName;
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

    public class JobLocation
    {
        public decimal? LocationLat { get; set; }
        public decimal? LocationLng { get; set; }
    }
}
