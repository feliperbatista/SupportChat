using System;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Agents.Queries;

public record GetAllAgentsQuery : IRequest<IEnumerable<AgentDto>>;

public class GetAllAgentsQueryHandler(
    IAgentRepository agentRepo
) : IRequestHandler<GetAllAgentsQuery, IEnumerable<AgentDto>>
{
    public async Task<IEnumerable<AgentDto>> Handle(GetAllAgentsQuery request, CancellationToken ct)
    {
        var agents = await agentRepo.GetAllActiveAsync(ct);
        return agents.Select(a => a.ToDto());
    }
}