using Database.Models;
using Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Image = Database.Models.Image;
using System.Collections;
using Microsoft.AspNetCore.Http.Internal;
using Net.Codecrete.QrCodeGenerator;

namespace Database.Services
{
    public class FileUploadLocalService : IFileUploadService
    {
        private readonly DeviceBookingContext _context;
        private readonly string _parentImageFolder;
        public FileUploadLocalService(DeviceBookingContext context, IConfiguration configuration)
        {
            _context = context;
            _parentImageFolder = configuration["RootFolder:DefaultImagePath"];
        }

        public async Task<bool> UploadPathToDevice(Device device, IFormFile file)
        {
            var path = await UploadFile(file, "Device", device.Id.ToString());

            if (path.Length == 0)
                return false;

            try
            {
                var image = await _context.Images.FindAsync(device.ImageId);
                if (image != null)
                    _context.Images.Remove(image);

                image = new Image { Path = path };
                _context.Images.Add(image);
                await _context.SaveChangesAsync();

                device.Image = image;
                _context.Entry(device).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> UploadPathToDeviceQR(Device device)
        {
            var url = $"http://84.201.132.210/devices/device?id={device.Id}";
            var qr = QrCode.EncodeText(url, QrCode.Ecc.Medium);
            var path = UploadQrFile(qr, "QR", device.Id.ToString());

            if (path.Length == 0)
                return false;

            try
            {
                var image = await _context.Images.FindAsync(device.QrId);
                if (image != null)
                    _context.Images.Remove(image);

                image = new Image { Path = path };
                _context.Images.Add(image);
                await _context.SaveChangesAsync();

                device.Qr = image;
                _context.Entry(device).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> UploadPathToUser(User user, IFormFile file)
        {
            var path = await UploadFile(file, "User", user.Id.ToString());

            if (path.Length == 0)
                return false;

            try
            {
                var image = await _context.Images.FindAsync(user.ImageId);
                if (image != null)
                    _context.Images.Remove(image);

                image = new Image { Path = path };
                _context.Images.Add(image);
                await _context.SaveChangesAsync();

                user.Image = image;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> UploadPathToReport(Report report, IFormFile file)
        {
            var countImg = report.ImageInfos.Count == 0 ? "" : $"({report.ImageInfos.Count})";

            var path = await UploadFile(file, "Report", $"{report.Id}{countImg}");

            if (path.Length == 0)
                return false;

            try
            {
                var image = new Image { Path = path };
                _context.Images.Add(image);
                await _context.SaveChangesAsync();

                var imageInfo = new ImageInfo { ReportId = report.Id, ImageId = image.Id };     
                _context.ImageInfos.Add(imageInfo);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<string> UploadFile(IFormFile file, string type, string id)
        {
            var newPath = "";
            try
            {
                if (file.Length > 0)
                {
                    var path = Path.GetFullPath(Path.Combine(_parentImageFolder,type));
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    using (var fileStream = new FileStream(Path.Combine(path, $"{type}_{id}{Path.GetExtension(file.FileName)}"), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                        newPath = Path.Combine(type, $"{type}_{id}{Path.GetExtension(file.FileName)}");
                    }
                }
                return newPath;
            }
            catch (Exception ex)
            {
                throw new Exception("File Copy Failed", ex);
            }
        }

        public string UploadQrFile(QrCode file, string type, string id)
        {
            try
            {
                var path = Path.GetFullPath(Path.Combine(_parentImageFolder, type));
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var pathToQr = Path.Combine(path, $"{type}_{id}.png");

                file.SaveAsPng(pathToQr, 16, 3);

                var newPath = Path.Combine(type, $"{type}_{id}.png");

                return newPath;
            }
            catch (Exception ex)
            {
                throw new Exception("File Copy Failed", ex);
            }
        }
    }
}
