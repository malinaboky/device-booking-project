using Admin.Service;
using Admin.ViewModels.Department;
using DotNetEd.CoreAdmin.Service;
using DotNetEd.CoreAdmin.ViewModels.Report;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DepartmentPageController : Controller
    {
        private readonly DepartmentService departmentService;

        public DepartmentPageController(DepartmentService departmentService)
        {
            this.departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Message = TempData["shortMessage"]?.ToString();
            return View(new DepartmentsList { Departments = await departmentService.GetDepartmentsFromDB() });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] DepartmentToCreate info)
        {
            if (ModelState.IsValid)
            {
                var messange = await departmentService.CreateDepartment(info);

                if (messange != null)
                    return new JsonResult(new { error = true, messange });

                return RedirectToAction("Index");
            }
            return View(info);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var department = await departmentService.GetDepartmentToEdit(id);

            if (department == null)
            {
                TempData["shortMessage"] = "Department is not found";
                return RedirectToAction("Index");
            }

            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] DepartmentToEdit info)
        {
            if (ModelState.IsValid)
            {
                var message = await departmentService.EditDepartment(info);

                if (message != null)
                {
                    TempData["shortMessage"] = message;
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index");
            }
            return View(info);
        }

        [HttpGet]
        public async Task<IActionResult> DeletePost(long id)
        {
            var message = await departmentService.DeleteDepartment(id);

            if (message != null)
                return new JsonResult(new { error = true, message });

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> DeleteAmountPost(DepartmentsList departmentList)
        {
            var message = await departmentService.DeleteDepartments(departmentList);

            if (message != null)
                return new JsonResult(new { error = true, message });

            return RedirectToAction("Index");
        }

    }
}
