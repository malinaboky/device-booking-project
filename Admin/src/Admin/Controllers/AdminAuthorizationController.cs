using Admin.ViewModels.Authorization;
using Database.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Admin.Service;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Admin.Controllers
{
    public class AdminAuthorizationController : Controller
    {
        private readonly AuthorizationService authorizationService;

        public AdminAuthorizationController(AuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginInfo info)
        {
            if (ModelState.IsValid)
            {
                var user = await authorizationService.CheckUser(info.UserName, info.Password);
                if (user == null)
                    return View(info);
                await Authenticate(user, info.RememberMe);
                return RedirectToAction("Index", "EmployeePage");
            }
            return View(info);
        }

        [HttpGet]
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

            return RedirectToAction("Login");
        }

        private async Task Authenticate(Employee user, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Status),
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
    }
}
