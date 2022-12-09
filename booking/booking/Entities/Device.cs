using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace booking.Entities
{
    public partial class Device
    {
        public Device()
        {
            Records = new HashSet<Record>();
            TagInfos = new HashSet<TagInfo>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public double? Diagonal { get; set; }
        public string? Resolution { get; set; }
        public string? Class { get; set; }
        public string? Info { get; set; }

        [ForeignKey("Type")]
        public int? TypeId { get; set; }

        [ForeignKey("Os")]
        public int? OsId { get; set; }

        [ForeignKey("Department")]
        public int? DepartmentId { get; set; }

        [ForeignKey("Img")]
        public int? ImgId { get; set; }

        public virtual Department? Department { get; set; }
        public virtual Image? Img { get; set; }
        public virtual Os? Os { get; set; }
        public virtual Type? Type { get; set; }
        public virtual ICollection<Record> Records { get; set; }
        public virtual ICollection<TagInfo> TagInfos { get; set; }
    }
}
