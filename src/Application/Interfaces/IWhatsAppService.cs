using System;

namespace Application.Interfaces;

public interface IWhatsAppService
{
    Task<string> SendTextAsync(string to, string text, CancellationToken ct = default);
    Task<string> SendImageAsync(string to, string imageUrl, string? caption = null, CancellationToken ct = default);
    Task<string> SendAudioAsync(string to, string audioUrl, CancellationToken ct = default);
    Task<string> SendDocumentAsync(string to, string documentUrl, string fileName, CancellationToken ct = default);
    Task SendReactionAsync(string to, string whatsAppMessageId, string emoji, CancellationToken ct = default);
    Task MarkAsReadAsync(string whatsAppMessageId, CancellationToken ct = default);
    Task<string> DownloadMediaAsync(string mediaId, string emoji, CancellationToken ct = default);
}
