using booking.DTO;
using booking.Entities;
using booking.Serializers;
using booking.Deserializers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace booking.Controllers
{
    [Route("api/filter")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [EnableCors("CorsPolicy")]
    public class FiltersController : ControllerBase
    {
        private readonly DeviceBookingContext _context;

        public FiltersController(DeviceBookingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<FiltersSerializer>> GetFilters()
        {
            var types = await _context.Types.Select(t => new TypeDTO { Id = t.Id, Name = t.Name }).ToListAsync();    
            var systems = await _context.Os.Select(o => new OsDTO { Id = o.Id, Name = o.Name }).ToListAsync();
            var dep = await _context.Departments.Select(d => new DepartmentDTO { Id = d.Id,Name = d.Name }).ToListAsync();
            var tags = await _context.Tags.Select(t => new TagDTO { Id = t.Id, Name = t.Name }).ToListAsync();

            return Ok(new FiltersSerializer { Types = types, Systems = systems, Departments = dep, Tags = tags });
        }

        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<ShortDevicesListDTO>>> SearchDevice([FromBody] SearchRootObject root)
        {
            var url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/image/?filePath=";
            var list = await _context.Devices.Where(d => string.IsNullOrEmpty(root.Name) || d.Name.ToLower().Replace(" ", "").Contains(root.Name.ToLower().Replace(" ", "")))
                                             .Where(d => root.Type == null || d.TypeId == root.Type)
                                             .Where(d => root.Os == null || d.OsId == root.Os)
                                             .Where(d => root.Department == null || d.DepartmentId == root.Department)
                                             .Select(d => new ShortDevicesListDTO
                                             {
                                                 Id = d.Id,
                                                 Name = d.Name,
                                                 Os = d.Os == null ? null : (d.Os.Name),
                                                 Diagonal = d.Diagonal,
                                                 Department = d.Department == null ? null : new DepartmentDTO { Id = d.Department.Id, Name = d.Department.Name},
                                                 Image = d.Img == null ? null : $"{url}{d.Img.Path}"
                                             })
                                             .ToListAsync();
           
            if (root.Tags != null)
            {
                var tags = await _context.Tags.Include(t => t.TagInfos).ToListAsync();
                var finalTags = tags.Where(t => root.Tags.Contains(t.Id))
                    .SelectMany(t => t.TagInfos.Select(info => info.DeviceId))
                    .GroupBy(t => t)
                    .ToDictionary(group => group.Key, group => group.Count());
                list = list.Where(d => finalTags.ContainsKey(d.Id))
                    .OrderBy(d => finalTags[d.Id])
                    .ToList();
            }         
            if (list.Count() == 0)
                return NotFound(new { error = true, message = "No content" });
            return Ok(list);
        }
    }
}
