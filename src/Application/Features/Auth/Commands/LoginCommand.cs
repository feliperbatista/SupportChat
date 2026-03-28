using System;
using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;

public class LoginCommandHandler(
    IAgentRepository agentRepo,
    IJwtService jwtService
) : IRequestHandler<LoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var agent = await agentRepo.GetByEmailAsync(request.Email, ct)
            ?? throw new UnauthorizedAccessException("Inavild email or password");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, agent.PasswordHash))
            throw new UnauthorizedAccessException("Inavild email or password");

        if (!agent.IsActive)
            throw new UnauthorizedAccessException("Account is deactivated");

        var token = jwtService.GenerateToken(agent);

        return new AuthResponseDto(token, 
            new AgentDto(agent.Id, agent.Name, agent.Email, agent.AvatarUrl, agent.Status.ToString()));
    }
}