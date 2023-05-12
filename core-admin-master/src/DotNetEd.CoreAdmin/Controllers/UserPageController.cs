using Database;
using DotNetEd.CoreAdmin.Service;
using DotNetEd.CoreAdmin.ViewModels.Device;
using DotNetEd.CoreAdmin.ViewModels.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.Controllers
{
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
            return View(new UserList { Users = await userService.GetUsersFromDB()});
        }
        [HttpGet]
        public async Task<IActionResult> Block(long id)
        {
            var user = await userService.GetUserToDelete(id);

            if (user == null)
                return NotFound(new { error = true, message = "User is not found" });

            return View(user);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> BlockPost(long id)
        {
            var message = await userService.BlockUser(id);

            if (message != null)
                return new JsonResult(new { error = true, message });

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
        public IActionResult Create()
        {
            ViewBag.Department = new SelectList(context.Departments, "Id", "Name");

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

            ViewBag.Department = new SelectList(context.Departments, "Id", "Name");

            return View("Create");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var user = await userService.GetUserToEdit(id);

            if (user == null)
                return NotFound();

            ViewBag.Department = new SelectList(context.Departments, "Id", "Name");

            return View("Edit", user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] UserToEdit formData)
        {
            var user = await context.Users.FindAsync(formData.Id);

            if (user == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                var message = await userService.EditUser(user, formData);

                if (message != null)
                    return new JsonResult(new { error = true, message });

                return RedirectToAction("Index");
            }

            ViewBag.Department = new SelectList(context.Departments, "Id", "Name");

            return View("Edit", formData);
        }
    }
}

