using System;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Agents.Commands;

public record CreateAgentCommand(
    string Name,
    string Email,
    string Password
) : IRequest<AgentDto>;

public class CreateAgentCommandHandler(
    IAgentRepository agentRepo
) : IRequestHandler<CreateAgentCommand, AgentDto>
{
    public async Task<AgentDto> Handle(CreateAgentCommand request, CancellationToken ct)
    {
        var existing = await agentRepo.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
            throw new InvalidOperationException($"Email '{request.Email}' is already in use.");

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var agent = Agent.Create(request.Name, request.Email, hash);

        await agentRepo.AddAsync(agent, ct);
        await agentRepo.SaveChangesAsync(ct);

        return new AgentDto(agent.Id, agent.Name, agent.Email, agent.AvatarUrl, agent.Status.ToString());
    }
}