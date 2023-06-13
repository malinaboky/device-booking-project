using Admin.Service;
using Admin.ViewModels.Employee;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class EmployeePageController : Controller
    {
        private readonly EmployeeService employeeService;

        public EmployeePageController(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.Identity?.Name == null)
                return NotFound(new { error = true, message = "User is not found" });

            var id = long.Parse(HttpContext.User.Identity.Name);

            var user = await employeeService.GetEmployeeFromDB(id);

            if (user == null)
                return NotFound(new { error = true, message = "User is not found" });

            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> Index(EmployeeDTO info)
        {
            if (HttpContext.User.Identity?.Name == null)
                return NotFound(new { error = true, message = "User is not found" });

            var id = long.Parse(HttpContext.User.Identity.Name);
            var user = await employeeService.GetEmployeeFromDB(id);

            if (user == null)
                return NotFound(new { error = true, message = "User is not found" });

            if (ModelState.IsValid)
            {
                var messange = await employeeService.SavePassword(id, info.Password);

                if (messange != null)
                    return new JsonResult(new { messange });
            }

            return View(user);
        }
    }
}
