using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
    public partial class Report
    {
        public Report()
        {
            ImageInfos = new HashSet<ImageInfo>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? Reason { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        public long UserId { get; set; }
        public long? ReviewerId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual User? Reviewer { get; set; }
        public virtual ICollection<ImageInfo> ImageInfos { get; set; }
    }
}
