using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
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
        public long Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Info { get; set; }

        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<Record> Records { get; set; }
        public virtual ICollection<User> Users { get; set; }

    }
}
