using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.ViewModels.Record
{
    public class RecordDTO
    {
        public long Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsBooked { get; set; }
        public long DeviceId { get; set; }
        public long UserId { get; set; }
        public bool Selected { get; set; }
    }
}
