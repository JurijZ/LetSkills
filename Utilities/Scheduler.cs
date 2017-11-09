using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using skillsBackend.Models;

namespace skillsBackend.Utilities
{
    public class Scheduler : IMyScheduler
    {
        private readonly SkillsContext _context;

        public Scheduler(SkillsContext context)
        {
            _context = context;
        }

        public void ProcessExpiredOffers()
        {
            //Console.WriteLine("Scheduled Processing of the Offers (Scheduler class)");

            DateTime currentTime = DateTime.Now;

            // Retrieve all offers that are not yet accepted
            var sentOffers = _context.Offers.Where(o => o.AcceptanceStatus == null);

            foreach (var offer in sentOffers)
            {
                //Console.WriteLine("Expiry Time: " + offer.OfferTimestamp.AddMinutes(offer.OfferExpiry));
                if (offer.OfferTimestamp.AddMinutes(offer.OfferExpiry) < currentTime)
                {
                    Console.WriteLine("Offer ID: " + offer.Id + " expired. Marking as expired");
                    offer.AcceptanceStatus = 0; // 0 - expired, 1 - accepted.
                    offer.AcceptanceTimestamp = currentTime;

                    var job = _context.Jobs.Where(j => j.Id == offer.JobId).SingleOrDefault();
                    job.JobState = 10;
                }
            }
            _context.SaveChanges();
        }
    }

    public interface IMyScheduler
    {
        void ProcessExpiredOffers();
    }
}
