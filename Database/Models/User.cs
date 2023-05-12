using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
    public partial class User
    {
        public User()
        {
            Records = new HashSet<Record>();
            Reports = new HashSet<Report>();
            ConsideredReports = new HashSet<Report>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? Firstname { get; set; }
        public string? Secondname { get; set; }
        public string? ConnectLink { get; set; }
        public string Username { get; set; } = null!;
        public string Status { get; set; } = null!;
        public bool IsBlocked { get; set; }

        public long? DepartmentId { get; set; }

        public long? ImageId { get; set; }

        public virtual Department? Department { get; set; }
        public virtual Image? Image { get; set; }
        public virtual ICollection<Record> Records { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<Report> ConsideredReports { get; set; }
    }
}
