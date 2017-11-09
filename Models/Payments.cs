using System;
using System.Collections.Generic;

namespace skillsBackend.Models
{
    public partial class Payments
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int ClientId { get; set; }
        public int ProviderId { get; set; }
        public decimal Ammount { get; set; }
        public int PayerUserId { get; set; }
        public int PayerType { get; set; }
        public byte PaymentType { get; set; }
        public DateTime TimeStamp { get; set; }

        public Users Client { get; set; }
        public Jobs Job { get; set; }
        public Users Provider { get; set; }
    }
}
