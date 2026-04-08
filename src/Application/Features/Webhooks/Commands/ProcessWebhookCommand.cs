using System;
using System.Runtime.InteropServices.Marshalling;
using System.Text.Json;
using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Webhooks.Commands;

public record ProcessWebhookCommand(JsonElement Payload) : IRequest;

public class ProcessWebhookCommandHandler(
    IContactRepository contactRepo,
    IConversationRepository conversationRepo,
    IMessageRepository messageRepo,
    IWhatsAppService whatsApp,
    INotificationService notifications
) : IRequestHandler<ProcessWebhookCommand>
{
    public async Task Handle(ProcessWebhookCommand request, CancellationToken ct)
    {
        if (!request.Payload.TryGetProperty("entry", out var entries)) return;

        foreach (var entry in entries.EnumerateArray())
        {
            if (!entry.TryGetProperty("changes", out var changes)) continue;

            foreach (var change in changes.EnumerateArray())
            {
                var value = change.GetProperty("value");

                if (value.TryGetProperty("statuses", out var statuses))
                    await HandleStatusUpdates(statuses, ct);

                if (value.TryGetProperty("messages", out var messages))
                    await HandleIncomingMessages(value, messages, ct);
            }
        }
    }

    private async Task HandleStatusUpdates(JsonElement statuses, CancellationToken ct)
    {
        foreach (var status in statuses.EnumerateArray())
        {
            var waMessageId = status.GetProperty("id").GetString()!;
            var statusStr = status.GetProperty("status").GetString()!;

            var message = await messageRepo.GetByWhatsAppIdAsync(waMessageId, ct);
            if (message is null) continue;

            var newStatus = statusStr switch
            {
                "sent" => MessageStatus.Sent,
                "delivered" => MessageStatus.Delivered,
                "read" => MessageStatus.Read,
                "failed" => MessageStatus.Failed,
                _ => message.Status
            };

            message.UpdateStatus(newStatus);
            await messageRepo.UpdateAsync(message, ct);
            await messageRepo.SaveChangesAsync(ct);

            await notifications.NotifyMessageStatusAsync(message.ConversationId, waMessageId, statusStr, ct);
        }
    }

    private async Task HandleIncomingMessages(JsonElement value, JsonElement messages, CancellationToken ct)
    {
        var contacts = value.TryGetProperty("contacts", out var c) ? c : default;

        foreach (var msg in messages.EnumerateArray())
        {
            var waMessageId = msg.GetProperty("id").GetString()!;
            var fromPhone = msg.GetProperty("from").GetString()!;
            var typeStr = msg.GetProperty("type").GetString()!;

            var alreadyExists = await messageRepo.GetByWhatsAppIdAsync(waMessageId, ct);
            if (alreadyExists is not null)
            {
                continue;
            }

            var contact = await contactRepo.GetByPhoneAsync(fromPhone, ct);
            if (contact is null)
            {
                var name = TryGetContactName(contacts, fromPhone);
                contact = Contact.Create(fromPhone, name ?? fromPhone);
                await contactRepo.AddAsync(contact, ct);
                await contactRepo.SaveChangesAsync(ct);
            }

            var conversation = await conversationRepo.GetByContactPhoneAsync(fromPhone, ct);
            if (conversation is null)
            {
                conversation = Conversation.Create(contact.Id);
                await conversationRepo.AddAsync(conversation, ct);
                await conversationRepo.SaveChangesAsync(ct);
            }

            var (content, mediaId, mimeType) = ParseMessageContent(msg, typeStr);
            var mediaUrl = string.IsNullOrEmpty(mediaId) ? null : await whatsApp.GetMediaUrl(mediaId, ct);
            var messageType = ParseMessageType(typeStr);

            if (messageType == MessageType.Reaction)
            {
                await HandleReaction(msg, conversation, ct);
                continue;
            }

            var domainMessage = Message.CreateIncoming(waMessageId, conversation.Id, content, messageType, mediaUrl, mediaId, mimeType);

            await messageRepo.AddAsync(domainMessage, ct);
            await messageRepo.SaveChangesAsync(ct);

            await whatsApp.MarkAsReadAsync(waMessageId, ct);

            var dto = domainMessage.ToDto();
            Console.WriteLine($"[Webhook] New incoming message dto: {dto.Id} conv={dto.ConversationId} type={dto.Type}");

            await notifications.NotifyNewMessageAsync(conversation.Id, conversation.AssignedAgentId, dto, ct);

            Console.WriteLine($"[Webhook] NotifyNewMessageAsync sent for conv-{conversation.Id}");  
            if (conversation.IsInQueue())
                await notifications.NotifyConversationQueuedAsync(conversation.ToDto(), ct);
        }
    }

    private async Task HandleReaction(JsonElement msg, Conversation conversation, CancellationToken ct)
    {
        var reactionEl = msg.GetProperty("reaction");
        var waMessageId = reactionEl.GetProperty("message_id").GetString()!;
        var emoji = reactionEl.GetProperty("emoji").GetString()!;
        var from = msg.GetProperty("from").GetString()!;

        var targetMessage = await messageRepo.GetByWhatsAppIdAsync(waMessageId, ct);
        if (targetMessage is null) return;

        if (string.IsNullOrEmpty(emoji))
        {
            await messageRepo.RemoveReactionAsync(targetMessage.Id, from, ct);
            await messageRepo.SaveChangesAsync(ct);

            await notifications.NotifyReactionRemovedAsync(conversation.Id, targetMessage.Id, from, ct);
        }

        var reaction = Reaction.Create(targetMessage.Id, emoji, from, false);
        await messageRepo.AddReactionAsync(reaction, ct);
        await messageRepo.SaveChangesAsync(ct);

        await notifications.NotifyReactionAsync(conversation.Id, targetMessage.Id, emoji, from, ct);
    }

    private static (string content, string? mediaId, string? mimeType) ParseMessageContent(JsonElement msg, string type)
    {
        return type switch
        {
            "text"     => (msg.GetProperty("text").GetProperty("body").GetString()!, null, null),
            "image"    => (TryGet(msg, "image", "caption") ?? "", TryGet(msg, "image", "id"), TryGet(msg, "image", "mime_type")),
            "audio"    => ("", TryGet(msg, "audio", "id"), TryGet(msg, "audio", "mime_type")),
            "video"    => (TryGet(msg, "video", "caption") ?? "", TryGet(msg, "video", "id"), TryGet(msg, "video", "mime_type")),
            "document" => (TryGet(msg, "document", "filename") ?? "", TryGet(msg, "document", "id"), TryGet(msg, "document", "mime_type")),
            "sticker"  => ("", TryGet(msg, "sticker", "id"), null),
            _          => ("Unsupported message type", null, null)
        };
    }

    private static MessageType ParseMessageType(string type) => type switch
    {
        "text"     => MessageType.Text,
        "image"    => MessageType.Image,
        "audio"    => MessageType.Audio,
        "video"    => MessageType.Video,
        "document" => MessageType.Document,
        "sticker"  => MessageType.Sticker,
        "reaction" => MessageType.Reaction,
        _          => MessageType.Unknown
    };

    private static string? TryGet(JsonElement el, string prop1, string prop2)
    {
        if (el.TryGetProperty(prop1, out var inner) &&
            inner.TryGetProperty(prop2, out var val))
            return val.GetString();
        return null;
    }

    private static string? TryGetContactName(JsonElement contacts, string phone)
    {
        if (contacts.ValueKind != JsonValueKind.Array) return null;
        foreach (var c in contacts.EnumerateArray())
            if (c.TryGetProperty("wa_id", out var id) && id.GetString() == phone)
                if (c.TryGetProperty("profile", out var profile) &&
                    profile.TryGetProperty("name", out var name))
                    return name.GetString();
        return null;
    }
}