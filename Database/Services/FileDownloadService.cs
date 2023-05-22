using Database.Models;
using Database.Interfaces;
using Database.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Database.Services
{
    public class FileDownloadService : IFileDownloadService
    {
        private readonly string _parentImageFolder;

        public FileDownloadService(IConfiguration configuration)
        {
            _parentImageFolder = configuration["RootFolder:DefaultImagePath"];
        }

        public string? GetFileToDownload(string? filePath)
        {
            if (filePath == null)
                return null;
            try
            {
                var fullPath = Path.Combine(_parentImageFolder, filePath);
                if (File.Exists(fullPath))
                    return fullPath;
                return null;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
