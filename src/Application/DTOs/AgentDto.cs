namespace Application.DTOs;

public record AgentDto
(
    Guid Id,
    string Name,
    string Email,
    string? AvatarUrl,
    string Status
);
