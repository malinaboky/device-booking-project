using DotNetEd.CoreAdmin.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.ViewModels.Employee
{
    public class EmployeeDTO
    {
        public string Status { get; set; }

        [MinLength(8)]
        public string Password { get; set; }

        public string Login { get; set; }
    }
}
