using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
    public partial class ImageInfo
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long ReportId { get; set; }

        public long ImageId { get; set; }

        public virtual Image Image { get; set; } = null!;
        public virtual Report Report { get; set; } = null!;
    }
}
