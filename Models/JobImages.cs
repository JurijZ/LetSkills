using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetSkillsBackend.Models
{
    public partial class JobImages
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        //public DateTime? UploadTimestamp { get; set; }
        public bool Deleted { get; set; } // Check that this one is configured correctly in the context

        public Jobs Job { get; set; }
    }
}
