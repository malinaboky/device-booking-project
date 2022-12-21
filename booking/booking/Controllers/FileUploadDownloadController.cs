using booking.Entities;
using booking.Interfaces;
using booking.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using System.Web;

namespace booking.Controllers
{
    [Route("api/image")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [EnableCors("CorsPolicy")]
    public class FileUploadDownloadController : Controller
    {
        private readonly IBufferedFileUploadService _bufferedFileUploadService;
        private readonly IFileDownloadService _fileDownloadService;
        private readonly DeviceBookingContext _context;

        public FileUploadDownloadController(DeviceBookingContext context, IConfiguration configuration)
        {
            _bufferedFileUploadService = new BufferedFileUploadLocalService(context, configuration);
            _fileDownloadService = new FileDownloadService(configuration);
            _context = context;
        }

        [HttpGet]
        public ActionResult Download([FromQuery] string filePath)
        {
            var fullPath = _fileDownloadService.GetFileToDownload(filePath);
            if (fullPath == null)
                return NotFound(new { error = true, message = "Image is not found" });

            var extention = Path.GetExtension(filePath).ToLowerInvariant();
            var stream = new FileStream(fullPath, FileMode.Open);
            var fileName = Path.GetFileName(fullPath);
            var contentType = MimeMapping.GetMimeMapping(extention);

            if (contentType == null)
            {
                contentType = "application/octet-stream";
            }

            return new FileStreamResult(stream, contentType)
            {
                FileDownloadName = fileName
            };
        }

        [HttpPost("device/{id}")]
        public async Task<ActionResult> UploadDeviceImg(int id, IFormFile file)
        {
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
                return NotFound(new { error = true, message = "Device is not found" });
            try
            {
                if (await _bufferedFileUploadService.UploadPathToDevice(device, file))
                    return Ok(new { message = "File upload successful" });

                return BadRequest(new { error = true, message = "File upload failed" });
            }
            catch (Exception)
            {
                return BadRequest(new { error = true, message = "File upload failed" });
            }
        }

        [HttpPost("user")]
        public async Task<ActionResult> UploadUserImage(IFormFile file)
        {
            if (HttpContext.User.Identity == null)
                return NotFound(new { error = true, message = "User is not found" });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);

            try
            {
                if (await _bufferedFileUploadService.UploadPathToUser(user, file))
                    return Ok(new { message = "File upload successful" });

                return BadRequest(new { error = true, message = "File upload failed" });
            }
            catch (Exception)
            {
                return BadRequest(new { error = true, message = "File upload failed" });
            }
        }


        [HttpPost("report/{reportId}")]
        public async Task<ActionResult> UploadReportImage(int reportId, IFormFile file)
        {
            var report = await _context.Reports.Include(r => r.ImageInfos).FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
                return NotFound(new { error = true, message = "Report is not found" });
            try
            {
                if (await _bufferedFileUploadService.UploadPathToReport(report, file))
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
