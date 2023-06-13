using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models
{
    public enum StatusOfReports
    {
        [Display(Name = "Открыт")]
        Open,
        [Display(Name = "В рассмотрении")]
        UnderConsider,
        [Display(Name = "В процессе")]
        InProgress,
        [Display(Name = "Закрыт")]
        Close
    }
}
