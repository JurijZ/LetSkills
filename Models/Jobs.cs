using System;
using System.Collections.Generic;

namespace skillsBackend.Models
{
    public partial class Jobs
    {
        public Jobs()
        {
            Applications = new HashSet<Applications>();
            FeedbackFromClients = new HashSet<FeedbackFromClients>();
            FeedbackFromProviders = new HashSet<FeedbackFromProviders>();
            JobDetails = new HashSet<JobDetails>();
            JobImages = new HashSet<JobImages>();
            Offers = new HashSet<Offers>();
            Payments = new HashSet<Payments>();
        }

        public int Id { get; set; }
        public int ClientId { get; set; }
        public string JobTitle { get; set; }
        public decimal? RatePerHour { get; set; }
        public decimal? RateFixed { get; set; }
        public int? DurationDays { get; set; }
        public int? DurationHours { get; set; }
        public string LocationPostCode { get; set; }
        public string ContactTelephone1 { get; set; }
        public string ContactTelephone2 { get; set; }
        public string ContactEmail { get; set; }
        public int? JobState { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedFinishDate { get; set; }
        public decimal? LocationLat { get; set; }
        public decimal? LocationLng { get; set; }
        public int? SkillId { get; set; }
        public string Telephone1Verified { get; set; }

        public Users Client { get; set; }
        public ICollection<Applications> Applications { get; set; }
        public ICollection<FeedbackFromClients> FeedbackFromClients { get; set; }
        public ICollection<FeedbackFromProviders> FeedbackFromProviders { get; set; }
        public ICollection<JobDetails> JobDetails { get; set; }
        public ICollection<JobImages> JobImages { get; set; }
        public ICollection<Offers> Offers { get; set; }
        public ICollection<Payments> Payments { get; set; }
        public Skills Skills { get; set; }
    }
}
