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
    public class CreateOfferController : Controller
    {

        private readonly SkillsContext _context;

        public CreateOfferController(SkillsContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public string Post([FromBody] NewOffer value)
        {

            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            var user = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            Console.WriteLine("Authenticated user name is: " + user.Value); //it's in a {key: value} format
            var userName = user.Value;

            DateTime currentTime = DateTime.Now;

            var client = _context.Users.FirstOrDefault(u => u.Username == userName);
            var job = _context.Jobs.FirstOrDefault(j => j.Id == value.JobId);
            var currentBalance = _context.Wallet.FirstOrDefault(w => w.UserId == job.ClientId);

            // Insert into the Offers table and modify Job state (only if there is enough balance in the Wallet)
            if (client != null && currentBalance.AvailableAmmount >= 5)
            {
                var newOffer = new Offers
                {
                    JobId = value.JobId,
                    ClientId = client.Id,
                    ProviderId = value.ProviderId,
                    OfferExpiry = value.OfferExpiry,
                    OfferTimestamp = DateTime.Now
                };
                _context.Offers.Add(newOffer);

                // Changing job state to Offer Sent
                job.JobState = 20;

                // Record change to the WalletHistory table
                var walletHistory = new WalletHistory
                {
                    WalletId = currentBalance.Id,
                    UserId = currentBalance.UserId,
                    AvailableAmountBefore = currentBalance.AvailableAmmount,
                    BlockedAmountBefore = currentBalance.BlockedAmmount,
                    AvailableAmountAfter = currentBalance.AvailableAmmount - 5,
                    BlockedAmountAfter = currentBalance.BlockedAmmount + 5,
                    TimeStamp = currentTime
                };
                _context.WalletHistory.Add(walletHistory);

                // Modify Clients' Wallet balance
                currentBalance.AvailableAmmount = currentBalance.AvailableAmmount - 5;
                currentBalance.BlockedAmmount = currentBalance.BlockedAmmount + 5;

                _context.SaveChanges();

                //return jobid back to Angular (technically there is not much sense returning anything appart from Ok);
                return newOffer.Id.ToString();
            }

            // Return error
            Console.WriteLine("--Offer for the JobID " + value.JobId + " FAILED");
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

    public class NewOffer
    {
        public int JobId { get; set; }
        public int ProviderId { get; set; }
        public int OfferExpiry { get; set; }
    }
}
