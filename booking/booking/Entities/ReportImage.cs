using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace booking.Entities
{
    public partial class ReportImage
    {
        public ReportImage()
        {
            ImageInfos = new HashSet<ImageInfo>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Filename { get; set; }
        public string Path { get; set; } = null!;

        public virtual ICollection<ImageInfo> ImageInfos { get; set; }
    }
}
