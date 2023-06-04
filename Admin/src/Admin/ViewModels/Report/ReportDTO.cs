using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.ViewModels.Report
{
    public class ReportDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }
        public bool Selected { get; set; }
    }
}
