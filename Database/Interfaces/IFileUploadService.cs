using Database.Models;
using Microsoft.AspNetCore.Http;

namespace Database.Interfaces
{
    public interface IFileUploadService
    {
        Task<string> UploadFile(IFormFile file, string type, string id);
        Task<bool> UploadPathToDevice(Device device, IFormFile file);
        Task<bool> UploadPathToUser(User user, IFormFile file);
        Task<bool> UploadPathToReport(Report report, IFormFile file);
    }
}
