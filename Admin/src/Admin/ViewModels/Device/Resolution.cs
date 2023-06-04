using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.ViewModels.Device
{
    public class Resolution
    {
        [RegularExpression(@"^\d+$", ErrorMessage = "Допустимы только числовые значения")]
        public string Height { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Допустимы только числовые значения")]
        public string Width { get; set; }
    }
}
