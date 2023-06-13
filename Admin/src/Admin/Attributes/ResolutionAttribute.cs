using Admin.ViewModels.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Attributes
{
    public class ResolutionAttribute : ValidationAttribute
    {
        public string GetErrorMessage() =>
            $"Обе величины должны быть заполнены";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var resolution = value as Resolution;

            if (resolution.Height != null && resolution.Width != null)
                return ValidationResult.Success;

            if (!(resolution.Height == null && resolution.Width == null))
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }
    }
}
