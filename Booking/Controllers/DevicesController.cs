using Database.DTO;
using Database.Models;
using Type = Database.Models.Type;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http.Extensions;
using System.Security.Policy;
using Booking.DTO.Device;

namespace Database.Controllers
{
    [Route("api/device")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
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
            var url = "/api/image/?filePath=";

            var list = await _context.Devices.Include(d => d.Os)
                                             .Include(d => d.Image)
                                             .Include(d => d.Department)
                                             .Select(
                                                  s => new ShortDevicesListDTO
                                                  {
                                                      Id = s.Id,
                                                      Name = s.Name,
                                                      Os = s.Os == null ? null : s.Os.Name,
                                                      Diagonal = s.Diagonal,
                                                      Department = s.Department == null ? null : new DepartmentDTO { Id = s.Department.Id, Name = s.Department.Name},
                                                      Image = s.Image == null ? null : $"{url}{s.Image.Path}"
                                                  }
            ).ToListAsync();

            if (list.Count < 0)
                return NotFound(new { error = true, message = "No content" });
            return Ok(list);
        }

        [HttpGet("info/short/{id}")]
        public async Task<ActionResult<ShortDeviceCardDTO>> GetDeviceShort(long id)
        {
            var url = "/api/image/?filePath=";

            var device = await _context.Devices.Include(d => d.Os)
                                               .Include(d => d.Image)
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
                Image = device.Image == null ? null : $"{url}{device.Image.Path}"
            });
        }

        [HttpGet("info/full/{id}")]
        public async Task<ActionResult<FullDeviceCardDTO>> GetDeviceFull(long id)
        {
            var url = "/api/image/?filePath=";

            var device = await _context.Devices.Include(d => d.Os)
                                               .Include(d => d.Image)
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
                Image = device.Image == null ? null : $"{url}{device.Image.Path}"
            });
        }


    }
}
