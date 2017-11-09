using System;
using System.Collections.Generic;

namespace skillsBackend.Models
{
    public partial class MapProviderSkills
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public int SkillId { get; set; }
        public byte? SkillProficiency { get; set; }
        public byte? YearsOfExperience { get; set; }
    }
}
