using System;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

public class Message
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string? WhatsAppMessageId { get; private set; }
    public Guid ConversationId { get; private set; }
    public Conversation Conversation { get; private set; } = null!;
    public string Content { get; private set; } = string.Empty;
    public MessageType Type { get; private set; }
    public MessageStatus Status { get; private set; } = MessageStatus.Pending;
    public string? MediaUrl { get; private set; }
    public string? MediaMimeType { get; private set; }
    public string? MediaId { get; private set; }
    public bool IsFromAgent { get; private set; }
    public Guid? SentByAgentId { get; private set; }
    public string? QuotedMessageId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public ICollection<Reaction> Reactions { get; private set; } = [];

    private Message() {}

    public static Message CreateIncoming
    (
        string whatsAppMessageId,
        Guid conversationId,
        string content,
        MessageType type,
        string? mediaUrl = null,
        string? mediaId = null,
        string? mediaMimeType = null,
        string? quotedMessageId = null
    )
    {
        return new Message
        {
          WhatsAppMessageId = whatsAppMessageId,
          ConversationId = conversationId,
          Content = content,
          Type = type,
          Status = MessageStatus.Delivered,
          MediaUrl = mediaUrl,
          MediaId = mediaId,
          MediaMimeType = mediaMimeType,
          IsFromAgent = false,
          QuotedMessageId = quotedMessageId  
        };
    }

    public static Message CreateOutgoing
    (
        Guid conversationId,
        string content,
        MessageType type,
        Guid sentByAgentId
    )
    {
        return new Message
        {
            ConversationId = conversationId,
            Content = content,
            Type = type,
            Status = MessageStatus.Pending,
            IsFromAgent = true,
            SentByAgentId = sentByAgentId
        };
    }

    public void SetWhatsAppId(string id, string? mediaId, string? mediaUrl)
    {
        WhatsAppMessageId = id;
        MediaId = mediaId;
        MediaUrl = mediaUrl;
    }

    public void UpdateStatus(MessageStatus status)
    {
        Status = status;
    }

    public void SetMediaUrl(string url)
    {
        MediaUrl = url;
    }
}
