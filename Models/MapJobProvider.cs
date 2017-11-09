using System;
using System.Collections.Generic;

namespace skillsBackend.Models
{
    public partial class MapJobProvider
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ProviderId { get; set; }
    }
}
