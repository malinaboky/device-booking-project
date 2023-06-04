using Admin.ViewModels.Device;
using Database;
using Database.Models;
using Database.Services;
using DotNetEd.CoreAdmin.ViewModels.Device;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.Service
{
    public class DeviceService 
    {
        private readonly DeviceBookingContext context;
        private readonly FileUploadLocalService uploadService;
        private readonly FileDownloadService downloadService;

        public DeviceService(DeviceBookingContext context,
            FileUploadLocalService uploadService, FileDownloadService downloadService)
        {
            this.context = context;
            this.uploadService = uploadService;
            this.downloadService = downloadService;
        }

        public async Task CreateDevice(DeviceToCreate formData)
        {
            var device = new Device
            {
                Name = formData.Name,
                DepartmentId = formData.DepartmentId,
                OsId = formData.OsId,
                TypeId = formData.TypeId,
                Resolution = formData.Resolution.Height == null ? null : $"{formData.Resolution.Height} x {formData.Resolution.Width}",
                Diagonal = formData.Diagonal == null ? null : double.Parse(formData.Diagonal.Replace('.', ',')),
                Class = formData.ClassOfDevice.ToString() == "" ? null : formData.ClassOfDevice.ToString(),
            };
            context.Devices.Add(device);
            try
            {
                await context.SaveChangesAsync();
                if (formData.Image != null)
                    await uploadService.UploadPathToDevice(device, formData.Image);
            }
            catch(DbUpdateException)
            {
                return;
            }
        }

        public async Task<string> DeleteDevice(long id)
        {
            var device = await context.Devices
                .Include(d => d.Records)
                .Include(d => d.TagInfos)
                .Include(d => d.Image)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
                return "Device is not found";

            var isBooked = device.Records.Any(r => r.Booked);
            if (isBooked)
                return "Device is booked now";

            if (device.Records != null)
                context.Records.RemoveRange(device.Records);

            if (device.TagInfos != null)
                context.TagInfos.RemoveRange(device.TagInfos);

            if (device.Image != null)
                context.Images.Remove(device.Image);

            context.Devices.Remove(device);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return"Error saving to database";
            }

            return null;
        }

        public async Task<string> DeleteDevices(DeviceList list)
        {
            var devices = new List<Device>();

            foreach (var device in list.Devices)
                if (device.Selected)
                    devices.Add(await context.Devices.Include(d => d.Records)
                        .Include(d => d.TagInfos)
                        .Include(d => d.Image)
                        .FirstOrDefaultAsync(d => d.Id == device.Id));

            var isBooked = devices.Any(d => d.Records.Any(r => r.Booked));
            if (isBooked)
                return "Device is booked now";

            foreach(var device in devices)
            {
                if (device.Records != null)
                    context.Records.RemoveRange(device.Records);

                if(device.TagInfos != null)
                    context.TagInfos.RemoveRange(device.TagInfos);

                if (device.Image != null)
                    context.Images.Remove(device.Image);
            }

            context.RemoveRange(devices);

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

        public async Task<List<DeviceDTO>> GetDevicesFromDB()
        {
            var list = await context.Devices
                .Include(d => d.Os)
                .Include(d => d.Image)
                .Include(d => d.Qr)
                .Select(d => new DeviceDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Os = d.Os == null ? "-" : d.Os.Name,
                    Diagonal = d.Diagonal == null ? 0 : (double)d.Diagonal,
                    ImagePath = downloadService.GetFileToDownload(d.Image.Path) == null ? "default.png" : "/api/image/?filePath=" + d.Image.Path,
                    QrPath = downloadService.GetFileToDownload(d.Qr.Path) == null ? "/default.png" : "/api/image/?filePath=" + d.Qr.Path,
                })
                .ToListAsync();
            return list;
        }

        public async Task<DeviceToEdit> GetDeviceToEdit(long id)
        {
            var device = await context.Devices.Include(d => d.Image)
                                              .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
                return null;

            var resolution = device.Resolution?.Replace(" ", "").Split('x');

            var deviceToEdit = new DeviceToEdit
            {
                Id = device.Id,
                Name = device.Name,
                DepartmentId = device.DepartmentId,
                OsId = device.OsId,
                TypeId = device.TypeId,
                Resolution = resolution == null ? new Resolution { Height = "", Width = ""} : new Resolution { Height = resolution[0], Width = resolution[1] },
                Diagonal = device.Diagonal == null ? "" : device.Diagonal.ToString(),
                ClassOfDevice = device.Class == null ? null : (ClassOfDevice)Enum.Parse(typeof(ClassOfDevice), device.Class),
                ImagePath = device.Image == null || downloadService.GetFileToDownload(device.Image.Path) == null ? "/default.png" : "/api/image/newtab/?filePath=" + device.Image.Path
        };
            return deviceToEdit;
        }

        public async Task<string> EditDevice(long id, DeviceToEdit formData)
        {
            var device = await context.Devices.FindAsync(id);

            if (device == null)
                return "Device is not found";

            device.Diagonal = formData.Diagonal == null ? null : double.Parse(formData.Diagonal.Replace('.', ','));
            device.Name = formData.Name;
            device.DepartmentId = formData.DepartmentId;
            device.OsId = formData.OsId;
            device.TypeId = formData.TypeId;
            device.Class = formData.ClassOfDevice.ToString() == "" ? null : formData.ClassOfDevice?.ToString();
            device.Resolution = formData.Resolution.Height == null ? null : $"{formData.Resolution.Height} x {formData.Resolution.Width}";

            context.Entry(device).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
                if (formData.Image != null)
                    await uploadService.UploadPathToDevice(device, formData.Image);
            }
            catch(DbUpdateException)
            {
                return "Error saving to database";
            }

            return null;
        }
    }
}
