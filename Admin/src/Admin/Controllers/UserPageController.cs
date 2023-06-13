using Database;
using Database.Models;
using DotNetEd.CoreAdmin.Service;
using DotNetEd.CoreAdmin.ViewModels;
using DotNetEd.CoreAdmin.ViewModels.Device;
using DotNetEd.CoreAdmin.ViewModels.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class UserPageController : Controller
    {
        private readonly DeviceBookingContext context;
        private readonly UserService userService;

        public UserPageController(DeviceBookingContext context,
            UserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Message = TempData["shortMessage"]?.ToString();
            return View(new UserList { Users = await userService.GetUsersFromDB()});
        }

        [HttpGet]
        public async Task<IActionResult> BlockPost(long id)
        {
            var message = await userService.BlockUser(id);

            if (message != null)
            {
                TempData["shortMessage"] = message;
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> BlockAmountPost(UserList userList)
        {
            var message = await userService.BlockUsers(userList);

            if (message != null)
                return new JsonResult(new { error = true, message });

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var departments = await context.Departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name,
                Selected = false
            }).ToListAsync();
            var status = (Enum.GetValues(typeof(StatusOfUser)).Cast<int>()
                                                          .Select(e =>
                                                          new SelectListItem()
                                                          {
                                                              Text = Enum.GetName(typeof(StatusOfUser), e),
                                                              Value = e.ToString(),
                                                              Selected = false
                                                          }))
                                                          .ToList();
            departments.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = true });
            status.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = true });

            ViewBag.DepartmentId = departments;
            ViewBag.Status = status;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] UserToCreate formData)
        {
            if (ModelState.IsValid)
            {
                await userService.CreateUser(formData);
                if (!formData.Stay)
                {
                    return RedirectToAction("Index");
                }
                ModelState.Clear();
            }

            var departments = await context.Departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name,
                Selected = formData.DepartmentId == d.Id
            }).ToListAsync();
            var status = (Enum.GetValues(typeof(StatusOfUser)).Cast<int>()
                                                          .Select(e =>
                                                          new SelectListItem()
                                                          {
                                                              Text = Enum.GetName(typeof(StatusOfUser), e),
                                                              Value = e.ToString(),
                                                              Selected = ((int)formData.Status) == e
                                                          }))
                                                          .ToList();
            departments.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = formData.DepartmentId == null });
            status.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = false });

            ViewBag.DepartmentId = departments;
            ViewBag.Status = status;

            return View("Create");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var user = await userService.GetUserToEdit(id);

            if (user == null)
            {
                TempData["shortMessage"] = "User is not found";
                return RedirectToAction("Index");
            }

            var departments = await context.Departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name,
                Selected = user.DepartmentId != null && d.Id == user.DepartmentId
            }).ToListAsync();
            departments.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = user.DepartmentId == null });

            ViewBag.DepartmentId = departments;
            ViewBag.Address = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            return View("Edit", user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] UserToEdit formData)
        {
            var user = await context.Users.FindAsync(formData.Id);

            if (user == null)
            {
                TempData["shortMessage"] = "User is not found";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                var message = await userService.EditUser(user, formData);

                if (message != null)
                    return new JsonResult(new { error = true, message });

                return RedirectToAction("Index");
            }

            var departments = await context.Departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name,
                Selected = formData.DepartmentId != null &&  formData.DepartmentId == d.Id
            }).ToListAsync();
            departments.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = formData.DepartmentId == null });

            ViewBag.DepartmentId = departments;
            ViewBag.Address = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            formData.ImagePath ??= "/default.png";

            return View("Edit", formData);
        }
    }
}

