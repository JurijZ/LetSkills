using System;
using System.Collections.Generic;

namespace skillsBackend.Models
{
    public partial class UserProviderDetails
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ContactTelephone1 { get; set; }
        public string ContactTelephone2 { get; set; }
        public string ContactEmail { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool? HaveAcar { get; set; }
        public decimal? LocationLat { get; set; }
        public decimal? LocationLng { get; set; }
        public bool? Searchable { get; set; }
        public byte? Rating { get; set; }    // tinyint in SQL
        public string Telephone1Verified { get; set; }

        public Users User { get; set; }
    }
}
