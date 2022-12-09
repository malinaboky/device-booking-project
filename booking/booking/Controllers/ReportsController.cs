using booking.Entities;
using booking.DTO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace booking.Controllers
{
    [Route("api/report")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ReportsController : ControllerBase
    {
        private readonly DeviceBookingContext _context;

        public ReportsController(DeviceBookingContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> PostReport([FromBody] ReportDTO report)
        {
            if (HttpContext.User.Identity == null)
                return NotFound(new { error = true, message = "User is not found" });

            if (report.Reason == null && report.Description == null)
                return BadRequest(new { error = true, message = "Report is empty" });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);
            var newReport = new Report { Reason = report.Reason, Description = report.Description, User = user};

            _context.Reports.Add(newReport);
            try 
            {   
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                return BadRequest(new { error = true, message = "Error saving to database" });
            }

            return Ok(new { id = newReport.Id });
        }
    }
}
