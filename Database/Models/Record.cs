using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
    public partial class Record
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public bool Booked { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly TimeFrom { get; set; }
        public TimeOnly TimeTo { get; set; }

        public long? DepartmentId { get; set; }

        public long DeviceId { get; set; }

        public long UserId { get; set; }

        public virtual Department? Department { get; set; }
        public virtual Device Device { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
