using System;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Conversations.Queries;

public record GetMyConversationsQuery(Guid AgentId) : IRequest<IEnumerable<ConversationDto>>;

public class GetMyConversationsQueryHandler(
    IConversationRepository conversationRepo
) : IRequestHandler<GetMyConversationsQuery, IEnumerable<ConversationDto>>
{
    public async Task<IEnumerable<ConversationDto>> Handle(GetMyConversationsQuery request, CancellationToken ct)
    {
        var conversations = await conversationRepo.GetByAgentIdAsync(request.AgentId, ct);
        return conversations.Select(c => c.ToDto());
    }
}