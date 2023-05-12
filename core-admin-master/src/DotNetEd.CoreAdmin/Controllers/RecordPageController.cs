using Database;
using DotNetEd.CoreAdmin.Service;
using DotNetEd.CoreAdmin.ViewModels.Record;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.Controllers
{
    public class RecordPageController : Controller
    {
        private readonly RecordService recordService;
        private readonly DeviceBookingContext context;

        public RecordPageController(DeviceBookingContext context,
            RecordService service)
        {
            this.context = context;
            this.recordService = service;
        } 

        public async Task<IActionResult> Index()
        {
            return View("Index", new RecordsList { Records = await recordService.GetRecordsFromDB()});
        }
    }
}
