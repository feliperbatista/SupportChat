using System;
using Application.Interfaces;
using Domain.Enums;
using MediatR;

namespace Application.Features.Agents.Commands;

public record UpdateAgentStatusCommand(Guid AgentId, AgentStatus Status) : IRequest;

public class UpdateAgentStatusCommandHandler(
    IAgentRepository agentRepo
) : IRequestHandler<UpdateAgentStatusCommand>
{
    public async Task Handle(UpdateAgentStatusCommand request, CancellationToken ct)
    {
        var agent = await agentRepo.GetByIdAsync(request.AgentId, ct)
            ?? throw new KeyNotFoundException("Agent not found");

        agent.UpdateStatus(request.Status);
        await agentRepo.SaveChangesAsync(ct);
    }
}