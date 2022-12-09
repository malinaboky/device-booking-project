using booking.Entities;
using booking.DTO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace booking.Controllers
{
    [Route("api/tag")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class TagsController : ControllerBase
    {
        private readonly DeviceBookingContext _context;

        public TagsController(DeviceBookingContext context)
        {
            _context = context;
        }

        [HttpGet("{deviceId}")]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetTags(int deviceId)
        {
            var list = await _context.TagInfos.Where(tag => tag.DeviceId == deviceId)
                                              .Include(tag => tag.Tag)
                                              .Select(
                                                 s => new TagDTO
                                                 {
                                                     Id = s.Tag.Id,
                                                     Name = s.Tag.Name
                                                 }
            ).ToListAsync();    

            if (list.Count == 0)
                return NotFound(new { error = true, message = "Tags are not found" });

            return Ok(list);
        }

        [HttpPut("{deviceId}/{tagId}")]
        public async Task<ActionResult> PutTagToDevice(int deviceId, int tagId)
        {
            var device = await _context.Devices.FindAsync(deviceId);
            var tag = await _context.Tags.FindAsync(tagId);

            if (device == null)
                return NotFound(new { error = true, message = "Device is not found" });

            if (tag == null)
                return NotFound(new { error = true, message = "Tag is not found" });

            var tagInfo = new TagInfo { Device = device, Tag = tag };
            _context.TagInfos.Add(tagInfo);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new {TagInfoId = tagInfo.Id});
            }
            catch(DbUpdateException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }
        }

        [HttpPost("{deviceId}/{name}")]
        public async Task<ActionResult> PostToDeviceTag(int deviceId, string name)
        {
            var device = await _context.Devices.FirstOrDefaultAsync(d => d.Id == deviceId);

            if (device == null) 
                return NotFound(new { error = true, message = "Device is not found" });

            try
            {
                var tag = new Tag { Name = name };
                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();

                var infoTag = new TagInfo { DeviceId = device.Id, TagId = tag.Id };
                _context.TagInfos.Add(infoTag);
                await _context.SaveChangesAsync();

                return Ok(new { tagId = tag.Id, InfoTagId = infoTag.Id});
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }
        }

        [HttpPost("{name}")]
        public async Task<ActionResult> PostTag(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest(new { error = true, message = "Tag is empty" });

            var tag = new Tag { Name = name };
            _context.Tags.Add(tag);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }

            return Ok(new { id = tag.Id });
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
                return NotFound(new { error = true, message = "Tag is not found" });

            _context.TagInfos.RemoveRange(_context.TagInfos.Where(info => info.TagId == tag.Id));
            _context.Tags.Remove(tag);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }

            return Ok(new { message = "Tag deleted successfully" });
        }

        [HttpDelete("delete/{deviceId}/{tagId}")]
        public async Task<ActionResult> DeleteTagFromDevice(int deviceId, int tagId)
        {
            var device = await _context.Devices.FindAsync(deviceId);
            var tag = await _context.Devices.FindAsync(tagId);

            if (device == null)
                return NotFound(new { error = true, message = "Device is not found" });

            if (tag == null)
                return NotFound(new { error = true, message = "Tag is not found" });

            _context.TagInfos.RemoveRange(_context.TagInfos.Where(info => info.TagId == tagId && info.DeviceId == deviceId));
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }

            return Ok(new { message = "Tag deleted successfully" });
        }
    }
}
