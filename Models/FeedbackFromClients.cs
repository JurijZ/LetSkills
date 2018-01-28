using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetSkillsBackend.Models
{
    public partial class FeedbackFromClients
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public int JobId { get; set; }
        public string FeedBackDescription { get; set; }
        public byte Skills { get; set; }
        public byte Communication { get; set; }
        public byte Punctuality { get; set; }

        // This column is automatically populated in the database therefore it's only readonly
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? FeedbackTimestamp { get; set; }

        public Jobs Job { get; set; }
        public Users User { get; set; }
    }
}
