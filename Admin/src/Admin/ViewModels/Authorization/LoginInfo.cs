using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.ViewModels.Authorization
{
    public class LoginInfo
    {
        [Required(ErrorMessage = "Обязательное поле")]
        public string UserName { get; set; } = null!;
        [Required(ErrorMessage = "Обязательное поле")]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }
}
