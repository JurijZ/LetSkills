using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace skillsBackend.Models
{
    public partial class Address
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostCode { get; set; }

        // This column is automatically populated in the database therefore it's only readonly
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ModifiedDate { get; set; }

        public Users User { get; set; }
    }
}
