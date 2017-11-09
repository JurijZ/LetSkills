using System;
using System.Collections.Generic;

namespace skillsBackend.Models
{
    public partial class WalletHistory
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public int UserId { get; set; }
        public decimal AvailableAmountBefore { get; set; }
        public decimal BlockedAmountBefore { get; set; }
        public decimal AvailableAmountAfter { get; set; }
        public decimal BlockedAmountAfter { get; set; }
        public DateTime TimeStamp { get; set; }

        public Users User { get; set; }
        public Wallet Wallet { get; set; }
    }
}
