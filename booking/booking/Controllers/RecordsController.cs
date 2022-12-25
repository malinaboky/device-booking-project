using booking.Entities;
using booking.DTO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;


namespace booking.Controllers
{
    [Route("api/records")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [EnableCors("CorsPolicy")]
    public class RecordsController : ControllerBase
    {
        private readonly DeviceBookingContext _context;

        public RecordsController(DeviceBookingContext context)
        {
            _context = context;
        }

        [HttpGet("history/{deviceId}")]
        public async Task<ActionResult<IEnumerable<RecordsDeviceCardDTO>>> GetRecordsOfDevice(int deviceId)
        {
            var records = await _context.Records.Include(r => r.User)
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

            var user = HttpContext.User.Identity.Name;

            var records = await _context.Records.Include(r => r.Device)
                                                .Include(r => r.User)
                                                .Where(r => r.User.Username == user)
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
            if (HttpContext.User.Identity?.Name == null)
                return NotFound(new { error = true, message = "User is not found" });

            var user = HttpContext.User.Identity.Name;

            var records = await _context.Records.Include(r => r.Device)
                                                .Include(r => r.User)
                                                .Include(r => r.Device.Img)
                                                .Where(r => r.User.Username == user && r.Booked)
                                                .ToListAsync();

            var list = records.Select(r => new UserRecordsDTO
            {
                Id = r.Id,
                Device = new DeviceInfo { 
                    Id = r.DeviceId,
                    Name = r.Device.Name,
                    ImgPath = r.Device.Img?.Path
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

            var userName = HttpContext.User.Identity.Name;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);

            if (user == null)
                return NotFound(new { error = true, message = "User is not found" });

            var device = await _context.Devices.Include(d => d.Department).FirstOrDefaultAsync(d => d.Id == newRecord.DeviceId);
            var oldRecord = await _context.Records.Include(r => r.User)
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

            _context.Records.Add(record);
            try
            {
                await _context.SaveChangesAsync();
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
            var record = await _context.Records.Include(r => r.Device) 
                                               .FirstOrDefaultAsync(r => r.Id == recordInfo.RecordId);

            var department = await _context.Departments.FindAsync(recordInfo.DepartmentId);

            if (record == null)
                return NotFound(new { error = true, message = "Record is not found" });

            if (department == null)
                return NotFound(new { error = true, message = "Department is not found" });

            record.Booked = false;
            record.Device.Department = department;

            _context.Entry(record).State = EntityState.Modified;
            _context.Entry(record.Device).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }
            return Ok($"Record updated; Don't forget to bring the device to department {department}");
        }

        [HttpPost("cancel/{recordId}")]
        public async Task<ActionResult> CancelRecord(int recordId)
        {
            var record = await _context.Records.FindAsync(recordId);

            if (record == null)
                return NotFound(new { error = true, message = "Record is not found" });

            record.Booked = false;

            _context.Entry(record).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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
            var record = await _context.Records.FindAsync(recordId);

            if (record == null)
                return NotFound(new { error = true, message = "Record is not found" });

            _context.Records.Remove(record);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }

            return Ok(new { message = "Record deleted successfully" });
        }
    }
}
