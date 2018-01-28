using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetSkillsBackend.Models
{
    public partial class Applications
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int ProviderId { get; set; }
        public bool WillingToPayForTheOffer { get; set; }
        
        // This column is automatically populated in the database therefore it's only readonly
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ApplicationTimestamp { get; set; }
        

        public Jobs Job { get; set; }
    }
}
