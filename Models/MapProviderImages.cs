using System;
using System.Collections.Generic;

namespace LetSkillsBackend.Models
{
    public partial class MapProviderImages
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public int ImageId { get; set; }
    }
}
