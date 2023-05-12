using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.ViewModels.User
{
    public class UserDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string ImagePath { get; set; }
        public bool isBlocked { get; set; }
        public bool Selected { get; set; }
    }
}
