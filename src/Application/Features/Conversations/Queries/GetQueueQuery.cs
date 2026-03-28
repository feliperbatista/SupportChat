using System;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Conversations.Queries;

public record GetQueueQuery : IRequest<IEnumerable<ConversationDto>>;

public class GetQueueQueryHandler(
    IConversationRepository conversationRepo
) : IRequestHandler<GetQueueQuery, IEnumerable<ConversationDto>>
{
    public async Task<IEnumerable<ConversationDto>> Handle(GetQueueQuery request, CancellationToken ct)
    {
        var conversations = await conversationRepo.GetQueueAsync(ct);
        return conversations.Select(c => c.ToDto());
    }
}