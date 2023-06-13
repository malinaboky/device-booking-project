using Database.Models;
using Database.Interfaces;
using Database.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using System.Web;

namespace Database.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [EnableCors("CorsPolicy")]
    public class FileUploadDownloadController : Controller
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IFileDownloadService _fileDownloadService;
        private readonly DeviceBookingContext _context;

        public FileUploadDownloadController(DeviceBookingContext context,
            FileDownloadService fileDownloadService,
            FileUploadLocalService fileUploadLocalService)
        {
            _fileUploadService = fileUploadLocalService;
            _fileDownloadService = fileDownloadService;
            _context = context;
        }

        [HttpGet("api/image")]
        public ActionResult Download([FromQuery] string? filePath)
        {
            var fullPath = _fileDownloadService.GetFileToDownload(filePath);

            if (fullPath == null)
                return NotFound(new { error = true, message = "Image is not found" });

            var extention = Path.GetExtension(fullPath).ToLowerInvariant();
            var stream = new FileStream(fullPath, FileMode.Open);
            var fileName = Path.GetFileName(fullPath);
            var contentType = MimeMapping.GetMimeMapping(extention);

            if (contentType == null)
                contentType = "application/octet-stream";

            return new FileStreamResult(stream, contentType)
            {
                FileDownloadName = fileName
            };
        }

        [HttpGet("admin/image")]
        public ActionResult DownloadForAdmin([FromQuery] string? filePath)
        {
            var fullPath = _fileDownloadService.GetFileToDownload(filePath);

            if (fullPath == null)
                return NotFound(new { error = true, message = "Image is not found" });

            var extention = Path.GetExtension(fullPath).ToLowerInvariant();
            var stream = new FileStream(fullPath, FileMode.Open);
            var fileName = Path.GetFileName(fullPath);
            var contentType = MimeMapping.GetMimeMapping(extention);

            if (contentType == null)
                contentType = "application/octet-stream";

            return new FileStreamResult(stream, contentType)
            {
                FileDownloadName = fileName
            };
        }

        [HttpGet("admin/image/newtab")]
        public ActionResult DownloadForNewTab([FromQuery] string? filePath)
        {
            var fullPath = _fileDownloadService.GetFileToDownload(filePath);

            if (fullPath == null)
                return NotFound(new { error = true, message = "Image is not found" });

            var extention = Path.GetExtension(fullPath).ToLowerInvariant();
            var stream = new FileStream(fullPath, FileMode.Open);
            var contentType = MimeMapping.GetMimeMapping(extention);

            if (contentType == null)
            {
                contentType = "application/octet-stream";
            }

            Response.Headers.Add("Content-Disposition", "inline");

            return new FileStreamResult(stream, contentType);
        }

        [HttpPost("api/image/device/{id}")]
        public async Task<ActionResult> UploadDeviceImg(long id, IFormFile file)
        {
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
                return NotFound(new { error = true, message = "Device is not found" });
            try
            {
                if (await _fileUploadService.UploadPathToDevice(device, file))
                    return Ok(new { message = "File upload successful" });

                return BadRequest(new { error = true, message = "File upload failed" });
            }
            catch (Exception)
            {
                return BadRequest(new { error = true, message = "File upload failed" });
            }
        }

        [HttpPost("api/image/user")]
        public async Task<ActionResult> UploadUserImage(IFormFile file)
        {
            if (HttpContext.User.Identity?.Name == null)
                return NotFound(new { error = true, message = "User is not found" });

            var userId = long.Parse(HttpContext.User.Identity.Name);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound(new { error = true, message = "User is not found" });

            try
            {
                if (await _fileUploadService.UploadPathToUser(user, file))
                    return Ok(new { message = "File upload successful" });

                return BadRequest(new { error = true, message = "File upload failed" });
            }
            catch (Exception)
            {
                return BadRequest(new { error = true, message = "File upload failed" });
            }
        }


        [HttpPost("api/image/report/{reportId}")]
        public async Task<ActionResult> UploadReportImage(long reportId, IFormFile file)
        {
            var report = await _context.Reports.Include(r => r.ImageInfos).FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
                return NotFound(new { error = true, message = "Report is not found" });
            try
            {
                if (await _fileUploadService.UploadPathToReport(report, file))
                    return Ok(new { message = "File upload successful" });

                return BadRequest(new { error = true, message = "File upload failed" });
            }
            catch (Exception)
            {
                return BadRequest(new { error = true, message = "File upload failed" });
            }
        }
    }
}
