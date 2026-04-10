using System;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Interfaces;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace Infrastructure.WhatsApp;

public class WhatsAppService(HttpClient http, IOptions<WhatsAppOptions> opts) : IWhatsAppService
{
    private readonly string _messagesUrl =
        $"https://graph.facebook.com/v19.0/{opts.Value.PhoneNumberId}/messages";

    private readonly string _uploadMediaUrl =
        $"https://graph.facebook.com/v19.0/{opts.Value.PhoneNumberId}/media";

    private readonly Func<string, string> _getMediaUrl = mediaId 
        => $"https://graph.facebook.com/v19.0/{mediaId}?phone_number_id={opts.Value.PhoneNumberId}";

    public async Task<string> GetMediaUrl(string mediaId, CancellationToken ct = default)
    {
        var url = _getMediaUrl(mediaId);
        var meta = await http.GetFromJsonAsync<JsonElement>(url, ct);
        var downloadUrl = meta.GetProperty("url").GetString()!;
        return downloadUrl;
    }

    public Task MarkAsReadAsync(string whatsAppMessageId, CancellationToken ct = default)
        => PostAsync(new
        {
            messaging_product = "whatsapp",
            status = "read",
            message_id = whatsAppMessageId
        }, ct);

    public Task<string> SendAudioAsync(string to, string audioId, CancellationToken ct = default)
        => PostAsync(new
        {
            messaging_product = "whatsapp",
            to,
            type = "audio",
            audio = new { id = audioId, voice = true }
        }, ct);

    public Task<string> SendDocumentAsync(string to, string documentId, string fileName, CancellationToken ct = default)
        => PostAsync(new
        {
            messaging_product = "whatsapp",
            to,
            type = "document",
            document = new { id = documentId , fileName }
        }, ct);

    public Task<string> SendImageAsync(string to, string imageId, string? caption = null, CancellationToken ct = default)
        => PostAsync(new
        {
            messaging_product = "whatsapp",
            to,
            type = "image",
            image = new { id = imageId, caption }
        }, ct);

    public Task SendReactionAsync(string to, string whatsAppMessageId, string emoji, CancellationToken ct = default)
        => PostAsync(new
        {
            messaging_product = "whatsapp",
            to,
            type = "reaction",
            reaction = new { message_id = whatsAppMessageId, emoji }
        }, ct);

    public Task<string> SendStickerAsync(string to, string stickerId, CancellationToken ct = default)
        => PostAsync(new
        {
            messaging_product = "whatsapp",
            to,
            type = "sticker",
            sticker = new { id = stickerId }
        }, ct);

    public Task<string> SendTextAsync(string to, string text, CancellationToken ct = default)
        => PostAsync(new
        {
            messaging_product = "whatsapp",
            to,
            type = "text",
            text = new { body = text }
        }, ct);

    public Task<string> SendVideoAsync(string to, string videoId, CancellationToken ct = default)
        => PostAsync(new
        {
            messaging_product = "whatsapp",
            to,
            type = "video",
            video = new { id = videoId }
        }, ct);

    public async Task<string> UploadMedia(Stream stream, string fileName, CancellationToken ct = default)
    {
        using var form = new MultipartFormDataContent();

        var fileContent = new StreamContent(stream);

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

        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

        form.Add(fileContent, "file", Path.GetFileName(fileName));
        form.Add(new StringContent("whatsapp"), "messaging_product");

        var response = await http.PostAsync(_uploadMediaUrl, form, ct);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"WhatsApp error: {response.StatusCode} - {body}");
        }

        return body.GetProperty("id").GetString()!;
    }

    private async Task<string> PostAsync(object payload, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync(_messagesUrl, payload, ct);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
        return body.GetProperty("messages")[0].GetProperty("id").GetString()!;
    }
    
}
