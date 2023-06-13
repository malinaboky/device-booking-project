using Database;
using Database.Models;
using DotNetEd.CoreAdmin.ViewModels.Device;
using DotNetEd.CoreAdmin.ViewModels.Record;
using DotNetEd.CoreAdmin.ViewModels.Report;
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

        public async Task<string> DeleteRecord(long id)
        {
            var record = await context.Records.FindAsync(id);

            if (record == null)
                return null;

            context.Records.Remove(record);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return "Error saving to database";
            }

            return null;
        }

        public async Task<string> DeleteRecords(RecordsList list)
        {
            var records = new List<Record>();
            if (list.Records == null)
                return null;
            foreach (var record in list.Records)
                if (record.Selected)
                {
                    var info = await context.Records.FindAsync(record.Id);
                    if (info == null)
                        continue;
                    records.Add(info);                 
                }          
            if (records.Count > 0)
                context.Records.RemoveRange(records);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return "Error saving to database";
            }

            return null;
        }
    }
}
