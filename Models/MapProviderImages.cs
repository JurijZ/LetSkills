using System;
using System.Collections.Generic;

namespace skillsBackend.Models
{
    public partial class MapProviderImages
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public int ImageId { get; set; }
    }
}
