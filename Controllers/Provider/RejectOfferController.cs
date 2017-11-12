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
    public class RejectOfferController : Controller
    {

        private readonly SkillsContext _context;

        public RejectOfferController(SkillsContext context)
        {
            _context = context;
        }

        [HttpPost]
        public string Post([FromBody] RejectOffer value)
        {

            Console.WriteLine("--Rejecting an Offer for the JobID: " + value.JobId);
            string userName = "jz@gmail.com"; // This value will be taken from the JWT claim
            DateTime currentTime = DateTime.Now;

            var provider = _context.Users.FirstOrDefault(u => u.Username == userName);
            var applications = _context.Applications.Where(a => a.JobId == value.JobId);
            var job = _context.Jobs.FirstOrDefault(j => j.Id == value.JobId);
            var currentBalance = _context.Wallet.FirstOrDefault(w => w.UserId == job.ClientId);
            var offer = _context.Offers.Where(o => (o.JobId == value.JobId) 
                                                && (o.ProviderId == provider.Id)
                                                && (o.AcceptanceStatus != 0)).FirstOrDefault();

            // Update Offers, Jobs and Applications table
            if (provider != null && offer != null)
            {
                // Update offer to Rejected
                offer.AcceptanceStatus = 0;
                offer.AcceptanceTimestamp = currentTime;
                Console.WriteLine("Rejection Time stamp " + currentTime);

                // Changing job state back to Published
                job.JobState = 10;

                // Removing records from the Applications table
                foreach (var application in applications)
                {
                    Console.WriteLine("Deleting application: " + application.Id);
                    _context.Applications.Remove(application);
                }

                // Record change to the WalletHistory table
                var walletHistory = new WalletHistory
                {
                    WalletId = currentBalance.Id,
                    UserId = currentBalance.UserId,
                    AvailableAmountBefore = currentBalance.AvailableAmmount,
                    BlockedAmountBefore = currentBalance.BlockedAmmount,
                    AvailableAmountAfter = currentBalance.AvailableAmmount + 5,
                    BlockedAmountAfter = currentBalance.BlockedAmmount - 5,
                    TimeStamp = currentTime
                };
                _context.WalletHistory.Add(walletHistory);

                // Update Clients' Wallet balance
                currentBalance.AvailableAmmount = currentBalance.AvailableAmmount + 5;
                currentBalance.BlockedAmmount = currentBalance.BlockedAmmount - 5;

                // Commit changes
                _context.SaveChanges();

                // Sending an SMS message to the Client informing that the Offer has been rejected
                Console.WriteLine("Sending an SMS to inform about Offer Rejection to the tel: " + job.ContactTelephone1);

                //return jobid back to Angular (technically there is not much sense returning anything appart from Ok);
                return offer.Id.ToString();
            }

            // Return error
            Console.WriteLine("--Offer Rejectino for the JobID " + value.JobId + " FAILED");
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

    public class RejectOffer
    {
        public int JobId { get; set; }
    }
}
