using System;
using System.Collections.Generic;

namespace skillsBackend.Models
{
    public partial class Images
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public DateTime UploadTimestamp { get; set; }
        public bool Deleted { get; set; }
    }
}
