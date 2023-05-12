using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
    public partial class Device
    {
        public Device()
        {
            Records = new HashSet<Record>();
            TagInfos = new HashSet<TagInfo>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; } = null!;
        public double? Diagonal { get; set; }
        public string? Resolution { get; set; }
        public string? Class { get; set; }

        public long? TypeId { get; set; }

        public long? OsId { get; set; }

        public long? DepartmentId { get; set; }

        public long? ImageId { get; set; }

        public long? QrId { get; set; }

        public virtual Department? Department { get; set; }
        public virtual Image? Image { get; set; }
        public virtual Image? Qr { get; set; }

        public virtual Os? Os { get; set; }
        public virtual Type? Type { get; set; }

        public virtual ICollection<Record> Records { get; set; }
        public virtual ICollection<TagInfo> TagInfos { get; set; }
    }
}
