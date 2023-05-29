using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.ViewModels.Device
{
    public class DeviceToCreate
    {
        [Required(ErrorMessage = "Обязательное поле")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression("^(?:-(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))|(?:0|(?:[1-9](?:\\d{0,2}(?:,\\d{3})+|\\d*))))(?:.\\d+|)$",
            ErrorMessage = "Допустимы только числа")]
        public string Diagonal { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression("^\\d+$", ErrorMessage = "Допустимы только числа")]
        public string Hight { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression("^\\d+$", ErrorMessage = "Допустимы только числа")]
        public string Width { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public long OsId { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public ClassOfDevice ClassOfDevice { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public long TypeId { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public long DepartmentId { get; set; }
        public IFormFile Image { get; set; }
        public bool Stay { get; set; }
    }
}
