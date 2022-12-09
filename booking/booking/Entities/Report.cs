using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace booking.Entities
{
    public partial class Report
    {
        public Report()
        {
            ImageInfos = new HashSet<ImageInfo>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Reason { get; set; }
        public string? Description { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<ImageInfo> ImageInfos { get; set; }
    }
}
