using booking.Entities;
using booking.Interfaces;
using booking.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace booking.Services
{
    public class FileDownloadService : IFileDownloadService
    {
        private readonly string _parentImageFolder;

        public FileDownloadService(IConfiguration configuration)
        {
            _parentImageFolder = configuration["RootFolder:DefaultImagePath"];
        }

        public string? GetFileToDownload(string filePath)
        {
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
