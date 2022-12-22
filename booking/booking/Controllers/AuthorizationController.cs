using booking.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Visus.LdapAuthentication;
using System.Security.Claims;
using booking.DTO;
using Microsoft.AspNetCore.Cors;

namespace booking.Controllers
{
    [Route("api/user")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class AuthorizationController : ControllerBase
    {
        private readonly DeviceBookingContext _context;
        private readonly ILdapAuthenticationService _authService;

        public AuthorizationController(DeviceBookingContext context, ILdapAuthenticationService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ILdapUser>> Login([FromBody] LoginDTO data)
        {
            try
            {
                var retval = _authService.Login(data.Username, data.Password);
                var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == retval.Identity);

                if (user == null)
                    try 
                    {
                        user = new User { Username = retval.Identity, Status = "User" };
                        _context.Users.Add(user);
                        await _context.SaveChangesAsync();
                    } 
                    catch(DbUpdateException)
                    {
                        return BadRequest(new { error = true, message = "Error saving to database" });
                    }
                await Authenticate(user);

                return Ok(new { message = "User successfully logged in" });
            }
            catch
            {
                return Unauthorized(new { error = true, message = "User is not found" });
            }
        }

        [HttpGet("logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");

            return Ok(new { message = "User successfully logged out" });
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Status)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id), new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
            });
        }
    }
}
