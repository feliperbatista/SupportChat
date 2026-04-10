using System;

namespace Application.Interfaces;

public interface IAzureBlobService
{
    Task<string> UploadBlob(Stream fileStream, string fileName, string contentType, CancellationToken ct = default);
}
