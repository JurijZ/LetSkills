using System;
using System.Collections.Generic;

namespace skillsBackend.Models
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
