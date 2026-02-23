namespace Corelink.Application.Contracts.Storage;

public interface IFileService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
    Task DeleteAsync(string publicUrl);
}