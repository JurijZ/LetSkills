using System;
using System.Collections.Generic;

namespace LetSkillsBackend.Models
{
    public partial class JobDetails
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobDescription { get; set; }
        public int? JobImageId { get; set; }

        public Jobs Job { get; set; }
    }
}
