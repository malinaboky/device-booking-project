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

            var listImages = new List<Image>();
            foreach (var info in report.ImageInfos)
                listImages.Add(await context.Images.FindAsync(info.ImageId));

            if (report == null)
                return "Report is not found";

            context.Images.RemoveRange(listImages);
            context.ImageInfos.RemoveRange(report.ImageInfos);
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

        public async Task<List<string>> DeleteReports(ReportsList list)
        {
            var listMessages = new List<string>();
            foreach (var report in list.Reports)
            {
                var message = await DeleteReport(report.Id);
                if (message != null)
                    listMessages.Add(message);
            }
            return listMessages;
        }

        public async Task<List<ReportDTO>> GetReportsFromDB()
        {
            var reports = await context.Reports
                .Select(r => new ReportDTO
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Reason = r.Reason,
                    Description = r.Description,
                    Status = r.Status,
                    Name = r.Name
                }).ToListAsync();
            return reports;
        }

        public async Task<ReportToDelete> GetReportToDelete(long id)
        {
            var report = await context.Reports.FindAsync(id);

            if (report == null)
                return null;

            return new ReportToDelete { Id = report.Id, Name = report.Name };
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
                Name = report.Name,
                Reason = report.Reason,
                Description = report.Description,
                Status = (StatusOfReports)Enum.Parse(typeof(StatusOfReports), report.Status),
                UserId = report.UserId,
                UserName = $"{report.User.Firstname} {report.User.Secondname}",
                ReviewerId = report.ReviewerId,
                Images = imageInfos.Select(i => 
                downloadService.GetFileToDownload(i.Image.Path) == null ? "/default.png" : "/api/image/newtab/?filePath=" + i.Image.Path).ToList()
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
