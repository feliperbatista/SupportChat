using System;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Messages.Queries;

public record GetMessageQuery( 
    Guid ConversationId,
    Guid AgentId) : IRequest<IEnumerable<MessageDto>>;

public class GetMessageQueryHandler(
    IConversationRepository conversationRepo,
    IMessageRepository messageRepo
) : IRequestHandler<GetMessageQuery, IEnumerable<MessageDto>>
{
    public async Task<IEnumerable<MessageDto>> Handle(GetMessageQuery request, CancellationToken ct)
    {
        var conversation = await conversationRepo.GetByIdAsync(request.ConversationId, ct)
            ?? throw new KeyNotFoundException("Conversation not found.");

        if (conversation.AssignedAgentId != request.AgentId)
            throw new UnauthorizedAccessException("You are not assigned to this conversation.");

        var messages = await messageRepo.GetByConversationIdAsync(request.ConversationId, ct);
        return messages.Select(m => m.ToDto());
    }
}