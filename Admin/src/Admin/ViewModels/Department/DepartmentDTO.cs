using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.ViewModels.Department
{
    public class DepartmentDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}
