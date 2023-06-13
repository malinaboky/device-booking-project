using Database;
using DotNetEd.CoreAdmin.Service;
using DotNetEd.CoreAdmin.ViewModels.Record;
using DotNetEd.CoreAdmin.ViewModels.Report;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class RecordPageController : Controller
    {
        private readonly RecordService recordService;

        public RecordPageController(RecordService service)
        {
            recordService = service;
        } 

        public async Task<IActionResult> Index()
        {
            return View("Index", new RecordsList { Records = await recordService.GetRecordsFromDB()});
        }

        [HttpGet]
        public async Task<IActionResult> DeletePost(long id)
        {
            var message = await recordService.DeleteRecord(id);

            if (message != null)
                return new JsonResult(new { error = true, message });

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> DeleteAmountPost(RecordsList recordsList)
        {
            var message = await recordService.DeleteRecords(recordsList);

            if (message != null)
                return new JsonResult(new { error = true, message });

            return RedirectToAction("Index");
        }
    }
}
