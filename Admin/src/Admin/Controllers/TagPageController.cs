using Database.Models;
using DotNetEd.CoreAdmin.Service;
using DotNetEd.CoreAdmin.ViewModels.Tag;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
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
            var messange = await tagService.EditOs(os);
            return Json(new { messange });
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
            var messange = await tagService.EditType(type);
            return Json(new { messange });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteTagPost(long id)
        {
            await tagService.DeleteTag(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteOsPost(long id)
        {
            await tagService.DeleteOs(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteTypePost(long id)
        {
            await tagService.DeleteType(id);
            return RedirectToAction("Index");
        }
    }
}
