using Booking.DTO.Record;
using Database;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;

namespace Booking.Services
{
    public class RecordAPIService
    {
        private readonly DeviceBookingContext context;
        private readonly string timeFormat;

        public RecordAPIService(DeviceBookingContext context, IConfiguration configuration)
        {
            this.context = context;
            timeFormat = configuration["TimeFormat"];
        }

        public string GetTimeZone(HttpContext context)
        {
            var timeInfo = context.User.Claims.FirstOrDefault(c => c.Type == "TimeZone")?.Value ?? "0";

            if (!timeInfo.Contains('-'))
                timeInfo = "+" + timeInfo;

            return timeInfo;
        }

        public DateOnly ParseTimeFromString(string timeInfo)
        {
            return DateOnly.FromDateTime(DateTime.Parse(timeInfo, null, DateTimeStyles.RoundtripKind));
        }


        public async Task<Device?> GetDeviceById(long id)
        {
            return await context.Devices.Include(d => d.Records).FirstOrDefaultAsync(d => d.Id == id);
        }

        public IEnumerable<CalendarRecordsOfDevice> GetRecords(IEnumerable<Record> records, DateOnly timeFrom, DateOnly timeTo, string timeZone)
        {
            return records.Where(r => r.Date >= timeFrom && r.Date <= timeTo)
                                 .Select(r =>
                                    new CalendarRecordsOfDevice
                                    {
                                        Id = r.Id,
                                        Title = r.Device.Name,
                                        Start = ConvertTimeToString(r.Date, r.TimeFrom, timeZone),
                                        End = ConvertTimeToString(r.Date, r.TimeTo, timeZone),
                                        Booked = r.Booked,
                                        DeviceId = r.DeviceId
                                    });
        }

        public async Task<List<Record>> GetRecordsOfUser(long userId)
        {
            return await context.Records.Include(r => r.Device).Where(r => r.UserId == userId).ToListAsync();
        }

        public bool GetCollision(DateOnly date, TimeOnly timeFrom, TimeOnly timeTo)
        {
            return context.Records.Where(r => r.Date == date).Any(r => !(r.TimeFrom > timeTo || r.TimeTo < timeFrom));
        }

        private string ConvertTimeToString(DateOnly date, TimeOnly time, string timeZone)
        {
            return DateTimeOffset.Parse(date.ToDateTime(time).ToString() + timeZone).ToString(timeFormat);
        }
    }
}
