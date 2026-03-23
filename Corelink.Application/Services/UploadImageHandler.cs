using Corelink.Application.Contracts.Storage;

namespace Corelink.Application.Services;

public sealed class UploadImageHandler
{
    private readonly IFileService _storage;

    public UploadImageHandler(IFileService storage)
    {
        _storage = storage;
    }

    public async Task<string> HandleAsync(
        Stream stream,
        string fileName,
        string contentType)
    {
        return await _storage.UploadAsync(
            stream,
            fileName,
            contentType);
    }
}