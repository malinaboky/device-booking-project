using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace booking.Entities
{
    public partial class ImageInfo
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Report")]
        public int ReportId { get; set; }

        [ForeignKey("Image")]
        public int ImageId { get; set; }

        public virtual ReportImage Image { get; set; } = null!;
        public virtual Report Report { get; set; } = null!;
    }
}
