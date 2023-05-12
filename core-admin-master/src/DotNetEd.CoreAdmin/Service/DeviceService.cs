using Database;
using Database.Models;
using Database.Services;
using DotNetEd.CoreAdmin.ViewModels.Device;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.Service
{
    public class DeviceService 
    {
        private readonly DeviceBookingContext context;
        private readonly FileUploadLocalService uploadService;

        public DeviceService(DeviceBookingContext context,
            FileUploadLocalService uploadService)
        {
            this.context = context;
            this.uploadService = uploadService;
        }

        public async Task CreateDevice(DeviceToCreate formData)
        {
            _ = double.TryParse(formData.Diagonal.Replace('.',','), out double diagonal);
            var device = new Device
            {
                Name = formData.Name,
                DepartmentId = formData.DepartmentId,
                OsId = formData.OsId,
                TypeId = formData.TypeId,
                Resolution = $"{formData.Hight} x {formData.Width}",
                Diagonal = diagonal,
                Class = formData.ClassOfDevice.ToString(),
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

            context.Records.RemoveRange(device.Records);
            context.TagInfos.RemoveRange(device.TagInfos);
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
                context.Records.RemoveRange(device.Records);
                context.TagInfos.RemoveRange(device.TagInfos);
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
                    Os = d.Os.Name,
                    Diagonal = (double)d.Diagonal,
                    ImagePath = "/api/image/?filePath=" + d.Image.Path,
                    QrPath = "/api/image/?filePath=" + d.Qr.Path
                })
                .ToListAsync();
            return list;
        }

        public async Task<DeviceToDelete> GetDeviceToDelete(long id)
        {
            var device = await context.Devices.FindAsync(id);

            if (device == null)
                return null;

            return new DeviceToDelete { Id = device.Id, Name = device.Name };
        }

        public async Task<DeviceToEdit> GetDeviceToEdit(long id)
        {
            var device = await context.Devices.FindAsync(id);
            if (device == null)
                return null;
            var resolution = device.Resolution.Replace(" ", "").Split('x');
            var deviceToEdit = new DeviceToEdit
            {
                Id = device.Id,
                Name = device.Name,
                DepartmentId = (long)device.DepartmentId,
                OsId = (long)device.OsId,
                TypeId = (long)device.TypeId,
                Hight = resolution[0],
                Width = resolution[1],
                Diagonal = device.Diagonal.ToString(),
                ClassOfDevice = (ClassOfDevice)Enum.Parse(typeof(ClassOfDevice), device.Class)
            };
            return deviceToEdit;
        }

        public async Task<string> EditDevice(Device device, DeviceToEdit formData)
        {
            _ = double.TryParse(formData.Diagonal, out double diagonal);

            device.Name = formData.Name;
            device.DepartmentId = formData.DepartmentId;
            device.OsId = formData.OsId;
            device.TypeId = formData.TypeId;
            device.Diagonal = diagonal;
            device.Class = formData.ClassOfDevice.ToString();
            device.Resolution = $"{formData.Hight} x {formData.Width}";

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
