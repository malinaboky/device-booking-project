using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.ViewModels.User
{
    public class UserToEdit
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я''-'\s]{1,41}$", ErrorMessage = "Допустимы только буквы")]
        [StringLength(40, ErrorMessage = "Слишком длинное имя")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я''-'\s]{1,41}$", ErrorMessage = "Допустимы только буквы")]
        [StringLength(40, ErrorMessage = "Слишком длинная фамилия")]
        public string Secondname { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [StringLength(40, ErrorMessage = "Слишком длинный никнейм")]
        public string Username { get; set; }

        public string ConnectLink { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public StatusOfUser Status { get; set; }

        public long DepartmentId { get; set; }

        public IFormFile Image { get; set; }

        public bool IsBlocked { get; set; }
    }
}
