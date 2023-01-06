using booking.DTO;
using booking.Entities;
using Type = booking.Entities.Type;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http.Extensions;
using System.Security.Policy;

namespace booking.Controllers
{
    [Route("api/device")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [EnableCors("CorsPolicy")]
    public class DevicesController : ControllerBase
    {
        private readonly DeviceBookingContext _context;

        public DevicesController(DeviceBookingContext context)
        {
            _context = context;
        }

        [HttpGet("info/short")]
        public async Task<ActionResult<IEnumerable<ShortDevicesListDTO>>> GetDevices()
        {
            var url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/image/?filePath=";

            var list = await _context.Devices.Include(d => d.Os)
                                             .Include(d => d.Img)
                                             .Include(d => d.Department)
                                             .Select(
                                                  s => new ShortDevicesListDTO
                                                  {
                                                      Id = s.Id,
                                                      Name = s.Name,
                                                      Os = s.Os == null ? null : s.Os.Name,
                                                      Diagonal = s.Diagonal,
                                                      Department = s.Department == null ? null : new DepartmentDTO { Id = s.Department.Id, Name = s.Department.Name},
                                                      Image = s.Img == null ? null : $"{url}{s.Img.Path}"
                                                  }
            ).ToListAsync();

            if (list.Count < 0)
                return NotFound(new { error = true, message = "No content" });
            return Ok(list);
        }

        [HttpGet("info/short/{id}")]
        public async Task<ActionResult<ShortDeviceCardDTO>> GetDeviceShort(int id)
        {
            var url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/image/?filePath=";

            var device = await _context.Devices.Include(d => d.Os)
                                               .Include(d => d.Img)
                                               .Include(d => d.Department)
                                               .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
                return NotFound(new { error = true, message = "Device is not found" });

            return Ok(new ShortDeviceCardDTO
            {
                Id = device.Id,
                Name = device.Name,
                Os = device.Os == null ? null : (device.Os.Name),
                Diagonal = device.Diagonal,
                Department = device.Department == null ? null : new DepartmentDTO { Id = device.Department.Id, Name = device.Department.Name },
                Image =$"{url}{device.Img?.Path}"
            });
        }

        [HttpGet("info/full/{id}")]
        public async Task<ActionResult<FullDeviceCardDTO>> GetDeviceFull(int id)
        {
            var url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/image/?filePath=";

            var device = await _context.Devices.Include(d => d.Os)
                                               .Include(d => d.Img)
                                               .Include(d => d.Type)
                                               .Include(d => d.Department)
                                               .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
                return NotFound(new { error = true, message = "Device is not found" });

            return Ok(new FullDeviceCardDTO
            {
                Id = device.Id,
                Name = device.Name,
                Os = device.Os?.Name,
                Diagonal = device.Diagonal,
                Type = device.Type?.Name,
                Department = device.Department == null ? null : new DepartmentDTO { Id = device.Department.Id, Name = device.Department.Name },
                Info = device.Info,
                Image = $"{url}{device.Img?.Path}"
            });
        }

        [Authorize(Policy = "OnlyForAdmin")]
        [HttpPost("add")]
        public async Task<ActionResult> PostDevice([FromBody] PostDeviceDTO device)
        {
            var type = await _context.Types.FirstOrDefaultAsync(t => device.Type == null || t.Name.ToLower() == device.Type.ToLower());
            var system = await _context.Os.FirstOrDefaultAsync(os => device.Os == null || os.Name.ToLower() == device.Os.ToLower());
            var department = await _context.Departments.FirstOrDefaultAsync(d => device.Department == null || d.Name.ToLower() == device.Department.ToLower());

            if (type == null && device.Type != null)
            {
                type = new Type { Name = device.Type };
                _context.Types.Add(type);
            }

            if (system == null && device.Os != null)
            {
                system = new Os { Name = device.Os };
                _context.Os.Add(system);
            }

            if (department == null && device.Department != null)
            {
                department = new Department { Name = device.Department };
                _context.Departments.Add(department);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }
            var newDevice = new Device
            {
                Name = device.Name,
                Diagonal = device.Diagonal,
                Resolution = device.Resolution,
                Class = device.Class,
                Type = type,
                Department = department,
                Os = system,
                Info = device.Info
            };

            _context.Devices.Add(newDevice);
            try
            {         
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }
            return Ok(new {id = newDevice.Id}); 
        }

        [Authorize(Policy = "OnlyForAdmin")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteDevice(int id)
        {
            var device = await _context.Devices.Include(d => d.Records).FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
                return NotFound(new { error = true, message = "Device is not found" });

            var isBooked = device.Records.Any(r => r.Booked);
            if (isBooked)
                return BadRequest(new { error = true, message = "Device is booked now" });
           
            _context.Records.RemoveRange(_context.Records.Where(r => r.DeviceId == device.Id));
            _context.TagInfos.RemoveRange(_context.TagInfos.Where(r => r.DeviceId == device.Id));
            _context.Devices.Remove(device);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }

            return Ok(new {message = "Device deleted"});
        }
    }
}
