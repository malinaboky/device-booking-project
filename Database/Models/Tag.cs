using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
    public partial class Tag
    {
        public Tag()
        {
            TagInfos = new HashSet<TagInfo>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<TagInfo> TagInfos { get; set; }
    }
}
