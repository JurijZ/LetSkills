using System;
using System.Collections.Generic;

namespace skillsBackend.Models
{
    public partial class CustomerPayments
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Ammount { get; set; }
        public byte PaymentMethod { get; set; }
        public string PaymentDetails { get; set; }
        public DateTime TimeStamp { get; set; }

        public Users User { get; set; }
    }
}
