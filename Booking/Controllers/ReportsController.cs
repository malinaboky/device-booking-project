using Database.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Database.DTO;

namespace Database.Controllers
{
    [Route("api/report")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [EnableCors("CorsPolicy")]
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
            if (HttpContext.User.Identity?.Name == null)
                return NotFound(new { error = true, message = "User is not found" });

            var id = long.Parse(HttpContext.User.Identity.Name);

            if (report.Reason == null && report.Description == null)
                return BadRequest(new { error = true, message = "Report is empty" });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound(new { error = true, message = "User is not found" });

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
