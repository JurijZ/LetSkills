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
    [Route("[controller]")]
    public class GetOfferProviderController : Controller
    {

        private readonly SkillsContext _context;

        public GetOfferProviderController(SkillsContext context)
        {
            _context = context;
        }

        // GET api/clientprofile
        [HttpGet("{job_id}")]
        //[Authorize]
        public OfferProvider Get(int job_id)
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            //var userName = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            //Console.WriteLine("Authenticated user name is: " + userName.Value); //it's in a JSON format - name: value
            
            //string userName = "jz@gmail.com"; // This value will be taken from the JWT claim
            Console.WriteLine("-- GetOfferProvider - for the JobID: " + job_id);

            // retrieve image urls
            //var imageUrls = (from i in _context.JobImages
            //                 where i.JobId == job_id
            //                 select i.Url).ToArray();


            var offerProvider = (from o in _context.Offers
                                 where o.JobId == job_id
                                 where o.AcceptanceStatus != 0  // Not expired jobs only
                                 select new OfferProvider
                                {
                                    providerId = o.ProviderId
                                }).SingleOrDefault();

            //foreach (var jobDetail in jobDetails)
            //{
            //    Console.WriteLine("Job Title: " + jobDetail.jobTitle);
            //}
            if (offerProvider != null)
            {
                Console.WriteLine("-- GetOfferProvider - Provider ID: " + offerProvider.providerId);
            }
            return offerProvider;
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

    public class OfferProvider
    {
        public int providerId { get; set; }
    }
}
