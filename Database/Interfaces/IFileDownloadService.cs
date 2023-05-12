
namespace Database.Interfaces
{
    public interface IFileDownloadService
    {
        string? GetFileToDownload(string? filePath);
    }
}
