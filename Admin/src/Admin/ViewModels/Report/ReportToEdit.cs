using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.ViewModels.Report
{
    public class ReportToEdit
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public StatusOfReports Status { get; set; }

        public long UserId { get; set; }
        public long? ReviewerId { get; set; }
        public string UserName { get; set; }
        public List<string> Images { get; set; }
    }
}
