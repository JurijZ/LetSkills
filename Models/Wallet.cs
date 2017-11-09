using System;
using System.Collections.Generic;

namespace skillsBackend.Models
{
    public partial class Wallet
    {
        public Wallet()
        {
            WalletHistory = new HashSet<WalletHistory>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal AvailableAmmount { get; set; }
        public decimal BlockedAmmount { get; set; }
        public DateTime? LastChangeTime { get; set; }

        public Users User { get; set; }
        public ICollection<WalletHistory> WalletHistory { get; set; }
    }
}
