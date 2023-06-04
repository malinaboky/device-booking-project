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
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateTagPost([FromForm] CreateTag info)
        {
            if (ModelState.IsValid)
                await tagService.SaveTag(info);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateTypePost([FromForm] CreateTag info)
        {
            if (ModelState.IsValid)
                await tagService.SaveType(info);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTagsPost([FromForm] ListOfTags list)
        {
            await tagService.DeleteTags(list.ListTag);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTypesPost([FromForm] ListOfTags list)
        {
            await tagService.DeleteTypes(list.ListType);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOsPost([FromForm] ListOfTags list)
        {
            await tagService.DeleteOsAmount(list.ListOs);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeOs([FromBody] ChangeTag os)
        {
            await tagService.EditOs(os);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeTag([FromBody] ChangeTag tag)
        {
            var messange = await tagService.EditTag(tag);
            return Json(new { messange });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeType([FromBody] ChangeTag type)
        {
            await tagService.EditType(type);
            return RedirectToAction("Index");
        }

        //[HttpPost]
        //async Task<IActionResult> DeleteTag(long id)
        //{

        //}
    }
}
