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

        // GET api/offerprovider/job_id
        [HttpGet("{job_id}")]
        [Authorize]
        public OfferProvider Get(int job_id)
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            //var jwtuser = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            //Console.WriteLine("Authenticated user name is: " + jwtuser.Value); //it's in a {key: value} format
            //var userName = jwtuser.Value;

            Console.WriteLine("-- GetOfferProvider - for the JobID: " + job_id);
            
            var offerProvider = (from o in _context.Offers
                                 where o.JobId == job_id
                                 where o.AcceptanceStatus != 0  // Not expired jobs only
                                 select new OfferProvider
                                {
                                    providerId = o.ProviderId
                                }).SingleOrDefault();

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
