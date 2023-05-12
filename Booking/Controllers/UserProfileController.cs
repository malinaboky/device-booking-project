using Database.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Database.Deserializers;
using Microsoft.AspNetCore.Cors;
using Database.DTO;

namespace Database.Controllers
{
    [Route("api/profile")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [EnableCors("CorsPolicy")]
    public class UserProfileController : ControllerBase
    {
        private readonly DeviceBookingContext _context;

        public UserProfileController(DeviceBookingContext context)
        {
            _context = context;
        }

        [HttpPut("update")]
        public async Task<ActionResult> ChangeProfileInfo([FromBody] ProfileRootObject profileInfo)
        {
            if (HttpContext.User.Identity?.Name == null)
                return NotFound(new { error = true, message = "User is not found" });

            var name = HttpContext.User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == name);

            if (user == null)
                return NotFound(new { error = true, message = "User is not found" });

            user.Firstname = profileInfo.Firstname ?? user.Firstname;
            user.Secondname = profileInfo.Secondname ?? user.Secondname;
            user.ConnectLink = profileInfo.ConnectLink ?? user.ConnectLink;

            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Changes saved" });
            }
            catch(DbUpdateException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }
        }


        [HttpGet]
        public async Task<ActionResult> GetProfileInfo()
        {
            var url = "/api/image/?filePath=";

            if (HttpContext.User.Identity?.Name == null)
                return NotFound(new { error = true, message = "User is not found" });

            var name = HttpContext.User.Identity.Name;
            var user = await _context.Users.Include(u => u.Image).FirstOrDefaultAsync(u => u.Username == name);

            if (user == null)
            {
                return NotFound(new { error = true, message = "User is not found" });
            }

            var info = new UserProfileDTO() {
                Id = user.Id,
                Firstname = user.Firstname,
                Secondname = user.Secondname,
                Username = user.Username,
                ConnectLink = user.ConnectLink,
                Status = user.Status,
                Image = user.Image == null ? null : $"{url}{user.Image.Path}"
            };

            return Ok(info);
        }
    }
}
