using booking.Entities;

namespace booking.Interfaces
{
    public interface IBufferedFileUploadService
    {
        Task<string> UploadFile(IFormFile file, string type, string id);
        Task<bool> UploadPathToDevice(Device device, IFormFile file);
        Task<bool> UploadPathToUser(User user, IFormFile file);
        Task<bool> UploadPathToReport(Report report, IFormFile file);
    }
}
