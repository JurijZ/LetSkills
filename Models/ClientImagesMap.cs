using System;
using System.Collections.Generic;

namespace skillsBackend.Models
{
    public partial class ClientImagesMap
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ImageId { get; set; }

        public Users Client { get; set; }
    }
}
