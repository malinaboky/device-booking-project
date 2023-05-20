using Database.Models;
using Database.DTO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Booking.DTO;
using Microsoft.Data.SqlClient.Server;

namespace Database.Controllers
{
    [Route("api/records")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [EnableCors("CorsPolicy")]
    public class RecordsController : ControllerBase
    {
        private readonly DeviceBookingContext context;

        public RecordsController(DeviceBookingContext context)
        {
            this.context = context;
        }

        [HttpPost("calendar/device")]
        public async Task<IActionResult> GetRecordsOfDeviceForCalendar([FromBody] InfoOfRecordsForCalendar recordsInfo)
        {
            var device = await context.Devices.Include(d => d.Records).FirstOrDefaultAsync(d => d.Id == recordsInfo.DeviceId);

            var timeFrom = DateOnly.FromDateTime(DateTime.Parse(recordsInfo.Start, null,
                                       System.Globalization.DateTimeStyles.RoundtripKind));
            var timeTo = DateOnly.FromDateTime(DateTime.Parse(recordsInfo.End, null,
                                       System.Globalization.DateTimeStyles.RoundtripKind));

            if (device == null)
                return NotFound(new { error = true, message = "Device is not found" });

            return Ok(device.Records.Where(r => r.Date >= timeFrom && r.Date <= timeTo)
                                    .Select(r => 
                                        new {
                                            id = r.Id,
                                            title = device.Name,
                                            start = r.Date.ToDateTime(r.TimeFrom).ToString("yyyy-MM-ddThh:mm:ss.SSSZ"),
                                            end = r.Date.ToDateTime(r.TimeTo).ToString("yyyy-MM-ddThh:mm:ss.SSSZ"),
                                            booked = r.Booked,
                                            deviceId = device.Id
                                        }));
        }

        [HttpPost("calendar/user")]
        public async Task<IActionResult> GetRecordsOfUserForCalendar([FromBody] InfoOfRecordsCalendarUser recordsInfo)
        {
            if (HttpContext.User.Identity?.Name == null)
                return NotFound(new { error = true, message = "User is not found" });

            var userId = long.Parse(HttpContext.User.Identity.Name);
            var user = await context.Users.FindAsync(userId);

            if (user == null)
                return NotFound(new { error = true, message = "User is not found" });

            var records = await context.Records.Include(r => r.Device).Where(r => r.UserId == user.Id).ToListAsync();

            var timeFrom = DateOnly.FromDateTime(DateTime.Parse(recordsInfo.Start, null,
                                       System.Globalization.DateTimeStyles.RoundtripKind));
            var timeTo = DateOnly.FromDateTime(DateTime.Parse(recordsInfo.End, null,
                                       System.Globalization.DateTimeStyles.RoundtripKind));            

            return Ok(records.Where(r => r.Date >= timeFrom && r.Date <= timeTo)
                                    .Select(r =>
                                        new {
                                            id = r.Id,
                                            title = r.Device.Name,
                                            start = r.Date.ToDateTime(r.TimeFrom).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                            end = r.Date.ToDateTime(r.TimeTo).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                            booked = r.Booked,
                                            deviceId = r.DeviceId
                                        }));
        }

        [HttpPost("calendar/change")]
        public async Task<IActionResult> ChangeRecordOfUser([FromBody] ChangedRecordInfo recordInfo)
        {
            if (HttpContext.User.Identity?.Name == null)
                return NotFound(new { error = true, message = "User is not found" });

            var userId = long.Parse(HttpContext.User.Identity.Name);
            var user = await context.Users.FindAsync(userId);

            if (user == null)
                return NotFound(new { error = true, message = "User is not found" });

            var record = await context.Records.FindAsync(recordInfo.RecordId);

            if (record == null)
                return NotFound(new { error = true, message = "Record is not found" });

            var timeFrom = DateTime.Parse(recordInfo.Start, null,
                                       System.Globalization.DateTimeStyles.RoundtripKind);
            var timeTo = DateTime.Parse(recordInfo.End, null,
                                       System.Globalization.DateTimeStyles.RoundtripKind);

            if (timeFrom.Date != timeTo.Date)
                return BadRequest(new { error = true, message = "Device can be booked only for one day" });

            record.Date = DateOnly.FromDateTime(timeFrom);
            record.TimeFrom = TimeOnly.FromDateTime(timeFrom);
            record.TimeTo = TimeOnly.FromDateTime(timeTo);

            context.Entry(record).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new {error = true, message = "Error saving to database" });
            }
        }

        [HttpGet("history/{deviceId}")]
        public async Task<ActionResult<IEnumerable<RecordsDeviceCardDTO>>> GetRecordsOfDevice(int deviceId)
        {
            var records = await context.Records.Include(r => r.User)
                                                .Where(r => r.DeviceId == deviceId)
                                                .ToListAsync();

            if (records.Count() == 0)
                return NotFound(new { error = true, message = "Records are not found" });

            var list = records.GroupBy(r => r.Date.ToShortDateString())
                              .Select(group => new RecordsDeviceCardDTO
                              {
                                  Date = group.Key,
                                  Records = group.Select(r => new RecordCard
                                  {
                                      UserName = $"{r.User.Firstname} {r.User.Secondname}",
                                      TimeFrom = r.TimeFrom.ToLongTimeString(),
                                      TimeTo = r.TimeTo.ToLongTimeString()
                                  }).ToList(),                              
                              });                               
            return Ok(list);
        }

        [HttpGet("history/user")]
        public async Task<ActionResult<IEnumerable<UserRecordsDTO>>> GetHistoryRecordsOfUser()
        {
            if (HttpContext.User.Identity?.Name == null)
                return NotFound(new { error = true, message = "User is not found" });

            var userId = long.Parse(HttpContext.User.Identity.Name);

            var records = await context.Records.Include(r => r.Device)
                                                .Include(r => r.User)
                                                .Where(r => r.User.Id == userId)
                                                .ToListAsync();

            var list = records.Select(r => new UserRecordsDTO {
                Id = r.Id,
                Device = new DeviceInfo { Id = r.DeviceId, Name = r.Device.Name },
                Booked = r.Booked,
                TimeFrom = r.TimeFrom.ToLongTimeString(),
                TimeTo = r.TimeTo.ToLongTimeString(),
                Date = r.Date.ToShortDateString()
            });

            return Ok(list);

        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<UserRecordsDTO>>> GetRecordsOfUser()
        {
            var url = "/api/image/?filePath=";
            if (HttpContext.User.Identity?.Name == null)
                return NotFound(new { error = true, message = "User is not found" });

            var userId = long.Parse(HttpContext.User.Identity.Name);

            var records = await context.Records.Include(r => r.Device)
                                                .Include(r => r.User)
                                                .Include(r => r.Device.Image)
                                                .Where(r => r.User.Id == userId && r.Booked)
                                                .ToListAsync();

            var list = records.Select(r => new UserRecordsDTO
            {
                Id = r.Id,
                Device = new DeviceInfo { 
                    Id = r.DeviceId,
                    Name = r.Device.Name,
                    ImgPath = r.Device.Image == null ? null : $"{url}{r.Device.Image.Path}"
                },
                Booked = r.Booked,
                TimeFrom = r.TimeFrom.ToLongTimeString(),
                TimeTo = r.TimeTo.ToLongTimeString(),
                Date = r.Date.ToShortDateString(),
            });

            return Ok(list);

        }

        [HttpPost()]
        public async Task<ActionResult<Record>> PostRecord([FromBody] NewRecordDTO newRecord)
        {
            var convertRecord = new ConvertTimeRecordDTO
            {
                Date = DateOnly.FromDateTime(newRecord.Date),
                TimeFrom = TimeOnly.FromDateTime(newRecord.TimeFrom),
                TimeTo = TimeOnly.FromDateTime(newRecord.TimeTo)
            };

            if (HttpContext.User.Identity?.Name == null)
                return NotFound(new { error = true, message = "User is not found" });

            var userId = long.Parse(HttpContext.User.Identity.Name);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound(new { error = true, message = "User is not found" });

            var device = await context.Devices.Include(d => d.Department).FirstOrDefaultAsync(d => d.Id == newRecord.DeviceId);
            var oldRecord = await context.Records.Include(r => r.User)
                                                  .Where(r => r.Date == convertRecord.Date)
                                                  .FirstOrDefaultAsync(r => !(r.TimeTo < convertRecord.TimeFrom || r.TimeFrom > convertRecord.TimeTo) && r.Booked);

            if (oldRecord != null)
                return BadRequest(new
                {
                    error = true,
                    message = $"Time conflict with user {oldRecord.User.Firstname} {oldRecord.User.Secondname} " +
                    $"(his record: {oldRecord.TimeFrom.ToShortTimeString()}-{oldRecord.TimeTo.ToShortTimeString()})"
                });

            if (device == null)
                return NotFound(new { error = true, message = "Device is not found" });

            var record = new Record {
                Date = convertRecord.Date,
                TimeFrom = convertRecord.TimeFrom,
                TimeTo = convertRecord.TimeTo,
                Device = device,
                User = user,
                Booked = true,
                Department = device.Department
            };

            context.Records.Add(record);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }
            return Ok(new { Id = record.Id });
        }

        [HttpPut("pass")]
        public async Task<ActionResult> PassRecord([FromBody] RecordDTO recordInfo)
        {
            var record = await context.Records.Include(r => r.Device) 
                                               .FirstOrDefaultAsync(r => r.Id == recordInfo.RecordId);

            var department = await context.Departments.FindAsync(recordInfo.DepartmentId);

            if (record == null)
                return NotFound(new { error = true, message = "Record is not found" });

            if (department == null)
                return NotFound(new { error = true, message = "Department is not found" });

            record.Booked = false;
            record.Device.Department = department;

            context.Entry(record).State = EntityState.Modified;
            context.Entry(record.Device).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }
            return Ok($"Record updated; Don't forget to bring the device to department {department.Name}");
        }

        [HttpPost("cancel/{recordId}")]
        public async Task<ActionResult> CancelRecord(int recordId)
        {
            var record = await context.Records.FindAsync(recordId);

            if (record == null)
                return NotFound(new { error = true, message = "Record is not found" });

            record.Booked = false;

            context.Entry(record).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }
            return Ok(new { message = "Record cancelled successfully" });
        }

        [HttpDelete("delete/{recordId}")]
        public async Task<IActionResult> DeleteRecord(int recordId)
        {
            var record = await context.Records.FindAsync(recordId);

            if (record == null)
                return NotFound(new { error = true, message = "Record is not found" });

            context.Records.Remove(record);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }

            return Ok(new { message = "Record deleted successfully" });
        }
    }
}
