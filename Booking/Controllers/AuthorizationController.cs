using Database.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Visus.LdapAuthentication;
using System.Security.Claims;
using Database.DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;

namespace Database.Controllers
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

                if (retval == null)
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return Unauthorized(new { error = true, message = "User is not found" });
                }

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
                await Authenticate(user, data.RememberMe);

                return Ok(new { message = "User successfully logged in" });
            }
            catch
            {
                return Unauthorized(new { error = true, message = "User is not found" });
            }
        }

        [HttpGet("authorize")]
        [AllowAnonymous]
        public ActionResult Check()
        {
            var authorize = IsAuthorized();
            if (authorize)
                return Ok(new { authorize = authorize });
            return Unauthorized(new { authorize = authorize });
        }

        [HttpGet("logout")]
        public async Task<ActionResult> Logout()
        {
            if (HttpContext.Request.Cookies.Count > 0)
            {
                var siteCookies = HttpContext.Request.Cookies.Where(c => c.Key.Contains(".AspNetCore.") || c.Key.Contains("Microsoft.Authentication"));
                foreach (var cookie in siteCookies)
                {
                    Response.Cookies.Delete(cookie.Key);
                }
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok(new { message = "User successfully logged out" });
        }

        private async Task Authenticate(User user, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Status)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            if (rememberMe)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id), new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMonths(1),
                    AllowRefresh = true
                });
            }
            else
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id), new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    AllowRefresh = true
                });
            } 
        }

        public bool IsAuthorized()
        {
            if (HttpContext.User.Identity == null)
                return false;
            return HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
