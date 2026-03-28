using System;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Interfaces;
using Microsoft.Extensions.Options;

namespace Infrastructure.WhatsApp;

public class WhatsAppService(HttpClient http, IOptions<WhatsAppOptions> opts) : IWhatsAppService
{
    private readonly string _messagesUrl =
        $"https://graph.facebook.com/v19.0/{opts.Value.PhoneNumberId}/messages";

    public async Task<string> DownloadMediaAsync(string mediaId, string emoji, CancellationToken ct = default)
    {
        var metaUrl = $"https://graph.facebook.com/v19.0/{mediaId}";
        var meta = await http.GetFromJsonAsync<JsonElement>(metaUrl, ct);
        var downloadUrl = meta.GetProperty("url").GetString()!;

        var bytes = await http.GetByteArrayAsync(downloadUrl, ct);

        return Convert.ToBase64String(bytes);
    }

    public Task MarkAsReadAsync(string whatsAppMessageId, CancellationToken ct = default)
        => PostAsync(new
        {
            messaging_product = "whatsapp",
            status = "read",
            message_id = whatsAppMessageId
        }, ct);

    public Task<string> SendAudioAsync(string to, string audioUrl, CancellationToken ct = default)
        => PostAsync(new
        {
            messaging_product = "whatsapp",
            to,
            type = "audio",
            audio = new { link = audioUrl }
        }, ct);

    public Task<string> SendDocumentAsync(string to, string documentUrl, string fileName, CancellationToken ct = default)
        => PostAsync(new
        {
            messaging_product = "whatsapp",
            to,
            type = "document",
            document = new { link = documentUrl , fileName }
        }, ct);

    public Task<string> SendImageAsync(string to, string imageUrl, string? caption = null, CancellationToken ct = default)
        => PostAsync(new
        {
            messaging_product = "whatsapp",
            to,
            type = "image",
            image = new { link = imageUrl, caption }
        }, ct);

    public Task SendReactionAsync(string to, string whatsAppMessageId, string emoji, CancellationToken ct = default)
        => PostAsync(new
        {
            messaging_product = "whatsapp",
            to,
            type = "reaction",
            reaction = new { message_id = whatsAppMessageId, emoji }
        }, ct);

    public Task<string> SendTextAsync(string to, string text, CancellationToken ct = default)
        => PostAsync(new
        {
            messaging_product = "whatsapp",
            to,
            type = "text",
            text = new { body = text }
        }, ct);

    private async Task<string> PostAsync(object payload, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync(_messagesUrl, payload, ct);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
        return body.GetProperty("messages")[0].GetProperty("id").GetString()!;
    }
}
