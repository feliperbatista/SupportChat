namespace Application.DTOs;

public record ConversationDto
(
    Guid Id,
    ContactDto Contact,
    AgentDto? AssignedAgent,
    string Status,
    bool IsInQueue,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    MessageDto? LastMessage
);