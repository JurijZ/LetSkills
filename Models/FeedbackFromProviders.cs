using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace skillsBackend.Models
{
    public partial class FeedbackFromProviders
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int JobId { get; set; }
        public string FeedBackDescription { get; set; }
        public byte FeedBackRating { get; set; }

        // This column is automatically populated in the database therefore it's only readonly
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? FeedbackTimestamp { get; set; }

        public Jobs Job { get; set; }
        public Users User { get; set; }
    }
}
