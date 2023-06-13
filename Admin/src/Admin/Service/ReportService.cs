using Database;
using Database.Models;
using Database.Services;
using DotNetEd.CoreAdmin.ViewModels;
using DotNetEd.CoreAdmin.ViewModels.Device;
using DotNetEd.CoreAdmin.ViewModels.Report;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.Service
{
    public class ReportService 
    {
        private readonly DeviceBookingContext context;
        private readonly FileUploadLocalService uploadService;
        private readonly FileDownloadService downloadService;

        public ReportService(DeviceBookingContext context, 
            FileUploadLocalService uploadService,
            FileDownloadService downloadService)
        {
            this.context = context;
            this.uploadService = uploadService;
            this.downloadService = downloadService;
        }

        public async Task<string> DeleteReport(long id)
        {
            var report = await context.Reports
                .Include(d => d.ImageInfos)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (report == null)
                return "Report is not found";

            var listImages = new List<Image>();
            foreach (var info in report.ImageInfos)
                listImages.Add(await context.Images.FindAsync(info.ImageId));
     
            context.Images.RemoveRange(listImages);
            context.Reports.Remove(report);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return "Error saving to database";
            }

            return null;
        }

        public async Task<string> DeleteReports(ReportsList list)
        {
            var reports = new List<Report>();
            var images = new List<Image>();
            if (list.Reports == null)
                return null;
            foreach (var report in list.Reports)
                if (report.Selected)
                {
                    var info = await context.Reports.Include(r => r.ImageInfos)
                                                    .FirstOrDefaultAsync(r => r.Id == report.Id);
                    if (info == null)
                        continue;
                    reports.Add(info);
                    if (info.ImageInfos.Count == 0)
                        continue;
                    foreach(var image in info.ImageInfos)
                        images.Add(await context.Images.FindAsync(image.ImageId));                  
                }
            if (images.Count > 0)
                context.Images.RemoveRange(images);
            if (reports.Count > 0)
                context.Reports.RemoveRange(reports);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return "Error saving to database";
            }

            return null;
        }

        public async Task<List<ReportDTO>> GetReportsFromDB()
        {
            Dictionary<string, string> reportMap = new()
            {
                { "Open", "Открыт"},
                { "UnderConsider", "В рассмотрении"},
                {"InProgress", "В процессе" },
                {"Close", "Закрыт"}
            };
            var reports = await context.Reports
                .Select(r => new ReportDTO
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Reason = r.Reason,
                    Description = r.Description,
                    Status = reportMap[r.Status]
                }).ToListAsync();
            return reports;
        }

        public async Task<ReportToEdit> GetReportToEdit(long id)
        {
            var report = await context.Reports.Include(r => r.User).FirstOrDefaultAsync(r => id == r.Id);

            if (report == null)
                return null;

            var imageInfos = await context.ImageInfos.Include(i => i.Image).Where(i => i.ReportId == id).ToListAsync();

            var reportToEdit = new ReportToEdit
            {
                Id = report.Id,
                Reason = report.Reason,
                Description = report.Description,
                Status = (StatusOfReports)Enum.Parse(typeof(StatusOfReports), report.Status),
                UserId = report.UserId,
                UserName = $"{report.User.Firstname} {report.User.Secondname}",
                ReviewerId = report.ReviewerId,
                Images = imageInfos.Select(i => 
                downloadService.GetFileToDownload(i.Image.Path) == null ? "/default.png" : "/admin/image/newtab/?filePath=" + i.Image.Path).ToList()
            };
            return reportToEdit;
        }

        public async Task<string> EditDevice(Report report, ReportToEdit formData)
        {
            report.Status = formData.Status.ToString();
            report.ReviewerId = formData.ReviewerId;

            context.Entry(report).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return "Error saving to database";
            }

            return null;
        }
    }
}
