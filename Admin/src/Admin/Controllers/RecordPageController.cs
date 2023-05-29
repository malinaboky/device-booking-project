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

        public RecordPageController(RecordService service)
        {
            recordService = service;
        } 

        public async Task<IActionResult> Index()
        {
            return View("Index", new RecordsList { Records = await recordService.GetRecordsFromDB()});
        }
    }
}
