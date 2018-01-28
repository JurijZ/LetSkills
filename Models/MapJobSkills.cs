using System;
using System.Collections.Generic;

namespace LetSkillsBackend.Models
{
    public partial class MapJobSkills
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int SkillId { get; set; }
        public byte SkillProficiency { get; set; }
        public byte YearsOfExperience { get; set; }
    }
}
