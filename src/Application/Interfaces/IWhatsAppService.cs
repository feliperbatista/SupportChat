using System;

namespace Application.Interfaces;

public interface IWhatsAppService
{
    Task<string> SendTextAsync(string to, string text, CancellationToken ct = default);
    Task<string> SendImageAsync(string to, string imageId, string? caption = null, CancellationToken ct = default);
    Task<string> SendAudioAsync(string to, string audioId, CancellationToken ct = default);
    Task<string> SendDocumentAsync(string to, string documentId, string fileName, CancellationToken ct = default);
    Task<string> SendStickerAsync(string to, string stickerId, CancellationToken ct = default);
    Task<string> SendVideoAsync(string to, string videoId, CancellationToken ct = default);
    Task SendReactionAsync(string to, string whatsAppMessageId, string emoji, CancellationToken ct = default);
    Task MarkAsReadAsync(string whatsAppMessageId, CancellationToken ct = default);
    Task<(Stream stream, string fileName, string mimeType)> DownloadMediaAsync(string mediaId, CancellationToken ct = default);
    Task<string> UploadMedia(Stream stream, string fileName, CancellationToken ct = default);
}
