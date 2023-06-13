using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.ViewModels.Department
{
    public class DepartmentToCreate
    {
        [Required(ErrorMessage = "Обязательное поле")]
        public string Name { get; set; }
    }
}
