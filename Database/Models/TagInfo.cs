using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
    public partial class TagInfo
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public long DeviceId { get; set; }

        public long TagId { get; set; }

        public virtual Device Device { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
