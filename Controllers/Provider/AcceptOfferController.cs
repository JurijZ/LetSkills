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
using skillsBackend.Utilities;

namespace skillsBackend.Controllers
{
    [Route("api/[controller]")]
    //[EnableCors("default")]
    public class AcceptOfferController : Controller
    {
        private readonly SkillsContext _context;

        public AcceptOfferController(SkillsContext context)
        {
            _context = context;
        }

        [HttpPost]
        public string Post([FromBody] AcceptOffer value)
        {

            Console.WriteLine("--Accepting an Offer for the JobID: " + value.JobId);

            // This is an account of the Provider
            string userName = "jz@gmail.com"; // This value will be taken from the JWT claim
            DateTime currentTime = DateTime.Now;

            var provider = _context.Users.FirstOrDefault(u => u.Username == userName);
            var applications = _context.Applications.Where(a => a.JobId == value.JobId);
            var job = _context.Jobs.FirstOrDefault(j => j.Id == value.JobId);
            var currentBalance = _context.Wallet.FirstOrDefault(w => w.UserId == job.ClientId);
            var offer = _context.Offers.Where(o => (o.JobId == value.JobId) 
                                                && (o.ProviderId == provider.Id)
                                                && (o.AcceptanceStatus != 0)).FirstOrDefault();

            // Update Offers, Jobs, Applications and Wallet tables
            if (provider != null && offer != null)
            {
                // Update offer to Accepted
                offer.AcceptanceStatus = 1;
                offer.AcceptanceTimestamp = currentTime;
                Console.WriteLine("Acceptance Time stamp " + currentTime);

                // Changing job state to Accepted
                job.JobState = 40;

                // Removing records from the Applications table
                foreach (var application in applications)
                {
                    Console.WriteLine("Deleting application: " + application.Id);
                    _context.Applications.Remove(application);
                }

                // Record Wallet changes to the WalletHistory table
                var walletHistory = new WalletHistory
                {
                    WalletId = currentBalance.Id,
                    UserId = currentBalance.UserId,
                    AvailableAmountBefore = currentBalance.AvailableAmmount,
                    BlockedAmountBefore = currentBalance.BlockedAmmount,
                    AvailableAmountAfter = currentBalance.AvailableAmmount,
                    BlockedAmountAfter = currentBalance.BlockedAmmount - 5,
                    TimeStamp = currentTime
                };
                _context.WalletHistory.Add(walletHistory);

                // Update Clients' Wallet balance
                currentBalance.BlockedAmmount = currentBalance.BlockedAmmount - 5;

                // Commit changes
                _context.SaveChanges();

                // Sending an SMS message to the Client informing that the Offer is Accepted
                Console.WriteLine("Sending an SMS to inform about Offer acception to the tel: " + job.ContactTelephone1);

                //return jobid back to Angular (technically there is not much sense returning anything appart from Ok);
                return offer.Id.ToString();
            }

            // Return error
            Console.WriteLine("--Offer Acceptance for the JobID " + value.JobId + " FAILED");
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

    public class AcceptOffer
    {
        public int JobId { get; set; }
    }
}
