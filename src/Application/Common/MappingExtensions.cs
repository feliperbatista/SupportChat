using System;
using System.ComponentModel.Design;
using Application.DTOs;
using Domain.Entities;

namespace Application.Common;

public static class MappingExtensions
{
    public static MessageDto ToDto(this Message m) => new(
        m.Id,
        m.WhatsAppMessageId,
        m.ConversationId,
        m.Content,
        m.Type.ToString(),
        m.Status.ToString(),
        m.MediaUrl,
        m.IsFromAgent,
        m.SentByAgentId,
        null,
        m.QuotedMessageId,
        m.CreatedAt,
        m.Reactions.Select(r => r.ToDto())
    );

    public static ReactionDto ToDto(this Reaction r) => new(
        r.Id,
        r.MessageId,
        r.Emoji,
        r.FromPhoneNumber,
        r.IsFromAgent,
        r.CreatedAt
    );

    public static ContactDto ToDto(this Contact c) => new(
        c.Id, c.PhoneNumber, c.Name, c.ProfilePictureUrl
    );

    public static AgentDto ToDto(this Agent a) => new(
        a.Id, a.Name, a.Email, a.AvatarUrl, a.Status.ToString()
    );

    public static ConversationDto ToDto(this Conversation c) => new(
        c.Id,
        c.Contact.ToDto(),
        c.AssignedAgent?.ToDto(),
        c.Status.ToString(),
        c.IsInQueue(),
        c.CreatedAt,
        c.UpdatedAt,
        c.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault()?.ToDto()
    );
}
