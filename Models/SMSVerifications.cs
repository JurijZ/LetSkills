using System;
using System.Collections.Generic;

namespace LetSkillsBackend.Models
{
    public partial class SMSVerifications
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int ObjectId { get; set; }
        public int ObjectIdType { get; set; }
        public string TelephoneNumber { get; set; }
        public string SMSCode { get; set; }
        public DateTime? SMSSentTime { get; set; }
        public bool? SMSVerified { get; set; }
        public DateTime? SMSVerifiedTime { get; set; }
    }
}
