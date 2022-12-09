using Microsoft.AspNetCore.Mvc;

namespace booking.Interfaces
{
    public interface IFileDownloadService
    {
        string? GetFileToDownload(string filePath);
    }
}
