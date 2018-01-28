using System;
using System.Collections.Generic;

namespace LetSkillsBackend.Models
{
    public partial class UserClientDetails
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ContactTelephone1 { get; set; }
        public string ContactTelephone2 { get; set; }
        public string ContactEmail { get; set; }
        public string ProfileImageUrl { get; set; }
        public byte? Rating { get; set; }    // tinyint in SQL
        public int MaxJobsAllowed { get; set; }

        public Users User { get; set; }
    }
}
