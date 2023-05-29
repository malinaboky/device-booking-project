using DotNetEd.CoreAdmin.Service;
using DotNetEd.CoreAdmin.ViewModels.Tag;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.Controllers
{
    public class TagPageController : Controller
    {
        private readonly TagService tagService;

        public TagPageController(TagService tagService)
        {
            this.tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await tagService.GetTags());
        }

        [HttpPost]
        public async Task<IActionResult> CreateOsPost([FromForm] CreateTag info)
        {
            if (ModelState.IsValid)
                await tagService.SaveOs(info);
            return View("Index", await tagService.GetTags());
        }

        [HttpPost]
        public async Task<IActionResult> CreateTagPost([FromForm] CreateTag info)
        {
            if (ModelState.IsValid)
                await tagService.SaveTag(info);
            return View("Index", await tagService.GetTags());
        }

        [HttpPost]
        public async Task<IActionResult> CreateTypePost([FromForm] CreateTag info)
        {
            if (ModelState.IsValid)
                await tagService.SaveType(info);
            return View("Index", await tagService.GetTags());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTagsPost([FromForm] ListOfTags list)
        {
            await tagService.DeleteTags(list.ListTag);
            return View("Index", await tagService.GetTags());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTypesPost([FromForm] ListOfTags list)
        {
            await tagService.DeleteTypes(list.ListType);
            return View("Index", await tagService.GetTags());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOsPost([FromForm] ListOfTags list)
        {
            await tagService.DeleteOsAmount(list.ListOs);
            return View("Index", await tagService.GetTags());
        }

        [HttpPost]
        public async Task<IActionResult> ChangeOs([FromBody] ChangeTag os)
        {
            await tagService.EditOs(os);
            return View("Index", await tagService.GetTags());
        }
    }
}
