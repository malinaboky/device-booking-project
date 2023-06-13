using Database;
using Database.Models;
using DotNetEd.CoreAdmin.Service;
using DotNetEd.CoreAdmin.ViewModels.Report;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ReportPageController : Controller
    {
        private readonly ReportService reportService;
        private readonly DeviceBookingContext context;

        public ReportPageController(ReportService reportService,
            DeviceBookingContext context)
        {
            this.reportService = reportService;
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Message = TempData["shortMessage"]?.ToString();
            return View("Index", new ReportsList { Reports = await reportService.GetReportsFromDB() });
        }

        [HttpGet]
        public async Task<IActionResult> DeletePost(long id)
        {
            var message = await reportService.DeleteReport(id);

            if (message != null)
                return new JsonResult(new { error = true, message });

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> DeleteAmountPost(ReportsList reportList)
        {
            var message = await reportService.DeleteReports(reportList);

            if (message != null)
                return new JsonResult(new { error = true, message });

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var report = await reportService.GetReportToEdit(id);

            if (report == null)
            {
                TempData["shortMessage"] = "Report is not found";
                return RedirectToAction("Index");
            }

            var reviewers = context.Users.Where(u => !u.IsBlocked)
             .Select(u => new SelectListItem
             {
                 Text = $"ID: {u.Id}, Username: {u.Firstname} {u.Secondname}",
                 Value = u.Id.ToString(),
                 Selected = report.ReviewerId == u.Id
             })
             .ToList();
            reviewers.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = report.ReviewerId == null });

            ViewBag.ReviewerId = reviewers;
            ViewBag.Address = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            return View("Edit", report);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] ReportToEdit formData)
        {
            var report = await context.Reports.FindAsync(formData.Id);

            if (report == null)
            {
                TempData["shortMessage"] = "Report is not found";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                var message = await reportService.EditDevice(report, formData);

                if (message != null)
                    return new JsonResult(new { error = true, message });

                return RedirectToAction("Index");
            }

            formData = await reportService.GetReportToEdit(formData.Id);

            if (formData == null)
            {
                TempData["shortMessage"] = "Report is not found";
                return RedirectToAction("Index");
            }

            var reviewers = context.Users.Where(u => !u.IsBlocked)
             .Select(u => new SelectListItem
             {
                 Text = $"ID: {u.Id}, Username: {u.Firstname} {u.Secondname}",
                 Value = u.Id.ToString(),
                 Selected = report.ReviewerId == u.Id
             })
             .ToList();
            reviewers.Add(new SelectListItem { Text = "Не выбран", Value = "", Selected = report.ReviewerId == null });

            ViewBag.ReviewerId = reviewers;
            ViewBag.Address = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            return View("Edit", formData);
        }
    }
}
