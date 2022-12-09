using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace booking.Entities 
{ 

    public partial class Type
    {
        public Type()
        {
            Devices = new HashSet<Device>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Device> Devices { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is Type type) return Id == type.Id;
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
