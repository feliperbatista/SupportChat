namespace Application.DTOs;

public record MessageDto
(
    Guid Id,
    string? WhatsAppMessageId,
    Guid ConversationId,
    string Content,
    string Type,
    string Status,
    string? MediaUrl,
    bool IsFromAgent,
    Guid? SentByAgentId,
    string? SentByAgentName,
    string? QuotedMessageId,
    DateTime CreatedAt,
    IEnumerable<ReactionDto> Reactions
);