using System;
using System.Collections.Generic;

namespace skillsBackend.Models
{
    public partial class Offers
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int ClientId { get; set; }
        public int ProviderId { get; set; }
        public DateTime OfferTimestamp { get; set; }
        public int OfferExpiry { get; set; }
        public byte? AcceptanceStatus { get; set; }
        public DateTime? AcceptanceTimestamp { get; set; }

        public Users Client { get; set; }
        public Jobs Job { get; set; }
        public Users Provider { get; set; }
    }
}
