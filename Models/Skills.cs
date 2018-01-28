using System;
using System.Collections.Generic;

namespace LetSkillsBackend.Models
{
    public partial class Skills
    {
        public Skills()
        {
            Jobs = new HashSet<Jobs>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Jobs> Jobs { get; set; }
    }
}
