namespace Application.DTOs;

public record AuthResponseDto
(
    string Token,
    AgentDto Agent
);