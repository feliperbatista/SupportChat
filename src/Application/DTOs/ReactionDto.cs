namespace Application.DTOs;

public record ReactionDto
(
    Guid Id,
    Guid MessageId,
    string Emoji,
    string FromPhoneNumber,
    bool IsFromAgent,
    DateTime CreatedAt
);