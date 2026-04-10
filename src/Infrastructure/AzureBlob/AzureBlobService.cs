using System;
using Application.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.AzureBlob;

public class AzureBlobService : IAzureBlobService
{
    private readonly BlobContainerClient _container;

    public AzureBlobService(BlobServiceClient blobServiceClient, IOptions<AzureBlobOptions> options)
    {
        var containerName = options.Value.ContainerName;

        _container = blobServiceClient.GetBlobContainerClient(containerName);
        _container.CreateIfNotExists(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
    }

    public async Task<string> UploadBlob(Stream fileStream, string fileName, CancellationToken ct = default)
    {
        string extension = Path.GetExtension(fileName);
        string blobName = Guid.NewGuid() + extension;

        var blobClient = _container.GetBlobClient(blobName);

        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders
        {
            ContentType = GetFileContentType(fileName)
        }, cancellationToken: ct);

        return blobClient.Uri.ToString();
    }

    private static string? GetFileContentType(string fileName)
    {
        string? contentType;

        if (fileName.EndsWith(".ogg"))
        {
            contentType = "audio/ogg";
        }
        else
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out contentType))
                contentType = "application/octet-stream";
        }

        return contentType;
    }
}
