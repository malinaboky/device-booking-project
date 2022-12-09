using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace booking.Entities
{
    public partial class Image
    {
        public Image()
        {
            Devices = new HashSet<Device>();
            Users = new HashSet<User>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Path { get; set; } = null!;

        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
