using Database;
using DotNetEd.CoreAdmin.ViewModels.Device;
using DotNetEd.CoreAdmin.ViewModels.Record;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.Service
{
    public class RecordService
    {
        private readonly DeviceBookingContext context;

        public RecordService(DeviceBookingContext context)
        {
            this.context = context;
        }

        public async Task<List<RecordDTO>> GetRecordsFromDB()
        {
            var list = await context.Records
                .Select(r => new RecordDTO
                {
                    Id = r.Id,
                    Date = r.Date,
                    StartTime = r.TimeFrom,
                    EndTime = r.TimeTo,
                    IsBooked = r.Booked,
                    UserId = r.UserId,
                    DeviceId = r.DeviceId
                })
                .ToListAsync();
            return list;
        }
    }
}
