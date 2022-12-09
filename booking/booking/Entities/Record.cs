using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace booking.Entities
{
    public partial class Record
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool Booked { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly TimeFrom { get; set; }
        public TimeOnly TimeTo { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        [ForeignKey("Device")]
        public int DeviceId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual Department Department { get; set; } = null!;
        public virtual Device Device { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
