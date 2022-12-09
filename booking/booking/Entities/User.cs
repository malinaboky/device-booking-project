using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace booking.Entities
{
    public partial class User
    {
        public User()
        {
            Records = new HashSet<Record>();
            Reports = new HashSet<Report>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Firstname { get; set; }
        public string? Secondname { get; set; }
        public string? ConnectLink { get; set; }
        public string Username { get; set; } = null!;
        public string Status { get; set; } = null!;

        [ForeignKey("Department")]
        public int? DepartmentId { get; set; }

        [ForeignKey("Img")]
        public int? ImgId { get; set; }

        public virtual Department? Department { get; set; }
        public virtual Image? Img { get; set; }
        public virtual ICollection<Record> Records { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
