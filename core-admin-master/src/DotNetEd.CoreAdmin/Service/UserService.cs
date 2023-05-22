using Database;
using Database.Models;
using Database.Services;
using DotNetEd.CoreAdmin.ViewModels;
using DotNetEd.CoreAdmin.ViewModels.Device;
using DotNetEd.CoreAdmin.ViewModels.User;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.Service
{
    public class UserService 
    {
        private readonly DeviceBookingContext context;
        private readonly FileUploadLocalService uploadService;
        private readonly FileDownloadService downloadService;

        public UserService(DeviceBookingContext context, 
            FileUploadLocalService uploadService,
            FileDownloadService downloadService)
        {
            this.context = context;
            this.uploadService = uploadService;
            this.downloadService = downloadService;
        }

        public async Task<string> BlockUser(long id)
        {
            var user = await context.Users
              .Include(u => u.Records)
              .Include(u => u.Image)
              .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return "User is not found";

            user.IsBlocked = true;

            if (user.Image != null)
                context.Images.Remove(user.Image);

            user.Records.ToList().ForEach(r => r.Booked = false);

            context.Entry(user).State = EntityState.Modified;
           
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

        public async Task<string> BlockUsers(UserList list)
        {
            var users = list.Users.Where(u => u.Selected)
                                  .Select(async u => await context.Users.Include(u => u.Image)
                                                                        .Include(u => u.Records)
                                                                        .FirstOrDefaultAsync(e => e.Id == u.Id))
                                  .Select(u => u.Result)
                                  .ToList();

            var images = users.Where(u => u.Image != null).Select(u => u.Image).ToList();
            var records = users.SelectMany(u => u.Records).ToList();

            if (images.Count > 0)
                context.Images.RemoveRange(images);

            if (records.Count > 0)
                records.ForEach(r => r.Booked = false);

            users.ForEach(u => u.IsBlocked = true);
            
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

        public async Task<List<UserDTO>> GetUsersFromDB()
        {
            var list = await context.Users
                .Include(u => u.Image)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Name = $"{u.Firstname} {u.Secondname}",
                    ImagePath = downloadService.GetFileToDownload(u.Image.Path) == null ? "default.png" : "/api/image/?filePath=" + u.Image.Path,
                    Status = u.Status,
                    isBlocked = u.IsBlocked
                })
                .ToListAsync();
            return list;
        }

        public async Task CreateUser(UserToCreate formData)
        {
            var user = new User
            {
                Firstname = char.ToUpper(formData.Firstname[0]) + formData.Firstname[1..].ToLower(),
                Secondname = char.ToUpper(formData.Secondname[0]) + formData.Secondname[1..].ToLower(),
                Username = formData.Username,
                DepartmentId = formData.DepartmentId,
                Status = formData.Status.ToString(),
                ConnectLink = formData.ConnectLink
            };
            context.Users.Add(user);
            try
            {
                await context.SaveChangesAsync();
                if (formData.Image != null)
                    await uploadService.UploadPathToUser(user, formData.Image);
            }
            catch (DbUpdateException)
            {
                return;
            }
        }

        public async Task<string> EditUser(User user, UserToEdit formData)
        {
            user.Firstname = char.ToUpper(formData.Firstname[0]) + formData.Firstname[1..].ToLower();
            user.Secondname = char.ToUpper(formData.Secondname[0]) + formData.Secondname[1..].ToLower();
            user.Username = formData.Username;
            user.Status = formData.Status.ToString();
            user.ConnectLink = formData.ConnectLink;
            user.DepartmentId = formData.DepartmentId;
            user.IsBlocked = (bool)formData.IsBlocked;

            context.Entry(user).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
                if (formData.Image != null)
                     await uploadService.UploadPathToUser(user, formData.Image);
            }
            catch (DbUpdateException)
            {
                return "Error saving to database";
            }

            return null;
        }

        public async Task<ReportToDelete> GetUserToDelete(long id)
        {
            var user = await context.Users.FindAsync(id);

            if (user == null)
                return null;

            return new ReportToDelete { Id = user.Id, Name = $"{user.Firstname} {user.Secondname}" };
        }

        public async Task<UserToEdit> GetUserToEdit(long id)
        {
            var user = await context.Users.FindAsync(id);

            if (user == null)
                return null;

            var userToEdit = new UserToEdit
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Secondname = user.Secondname,
                Username = user.Username,
                Status = (StatusOfUser)Enum.Parse(typeof(StatusOfUser), user.Status),
                ConnectLink = user.ConnectLink,
                DepartmentId = (long)user.DepartmentId,
                IsBlocked = user.IsBlocked
            };
            return userToEdit;
        }
    }
}
       