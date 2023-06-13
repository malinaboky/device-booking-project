using Admin.ViewModels.Device;
using Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Attributes
{
    public class UniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = (DeviceBookingContext)validationContext.GetService(typeof(DeviceBookingContext));
            var username = value?.ToString();
            try
            {
                if (context.Users.Any(u => u.Username == username))
                    return new ValidationResult("Никнейм должен быть уникальным.");
            }
            catch
            {
                return new ValidationResult("Ошибка соединения к базе данных.");
            }
          
            return ValidationResult.Success;
        }
    }
}
