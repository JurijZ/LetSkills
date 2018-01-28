using System;
using System.Collections.Generic;

namespace LetSkillsBackend.Models
{
    public partial class MapJobImages
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int ImageId { get; set; }
    }
}
