using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
    public partial class Image
    {
        public Image()
        {
            Devices = new HashSet<Device>();
            Users = new HashSet<User>();
            ImageInfos = new HashSet<ImageInfo>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Path { get; set; } = null!;

        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<Device> Qrs { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<ImageInfo> ImageInfos { get; set; }
    }
}
