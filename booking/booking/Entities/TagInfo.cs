using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace booking.Entities
{
    public partial class TagInfo
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Device")]
        public int DeviceId { get; set; }

        [ForeignKey("Tag")]
        public int TagId { get; set; }

        public virtual Device Device { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
