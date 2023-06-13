using Admin;
using Admin.Attributes;
using Admin.ViewModels.Device;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.ViewModels.Device
{
    public class DeviceToEdit
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string Name { get; set; } = null!;

        [RegularExpression("^(?:-(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))|(?:0|(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))))(?:.\\d+|)$", ErrorMessage = "Допустимы только числовые значения")]
        public string Diagonal { get; set; }
        
        [ResolutionAttribute]        
        public Resolution Resolution { get; set; }

        public long? OsId { get; set; }
        public ClassOfDevice? ClassOfDevice { get; set; }
        public long? TypeId { get; set; }
        public long? DepartmentId { get; set; }
        public string ImagePath { get; set; }
        public IFormFile Image { get; set; }
    }
}
