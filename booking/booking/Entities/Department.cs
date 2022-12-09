using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace booking.Entities
{
    public partial class Department
    {
        public Department()
        {
            Devices = new HashSet<Device>();
            Records = new HashSet<Record>();
            Users = new HashSet<User>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Info { get; set; }

        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<Record> Records { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is Department department) return Id == department.Id;
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
