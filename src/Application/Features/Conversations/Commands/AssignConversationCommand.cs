using System;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Conversations.Commands;

public record AssignConversationCommand(
    Guid ConversationId, 
    Guid AgentId) : IRequest<ConversationDto>;

public class AssignConversationCommandHandler(
    IConversationRepository conversationRepo,
    IAgentRepository agentRepo,
    INotificationService notifications
) : IRequestHandler<AssignConversationCommand, ConversationDto>
{
    public async Task<ConversationDto> Handle(AssignConversationCommand request, CancellationToken ct)
    {
        var conversation = await conversationRepo.GetByIdAsync(request.ConversationId, ct)
            ?? throw new KeyNotFoundException("Conversation not found.");

        var agent = await agentRepo.GetByIdAsync(request.AgentId, ct) 
            ?? throw new KeyNotFoundException("Agent not found.");

        conversation.AssignTo(request.AgentId);
        await conversationRepo.UpdateAsync(conversation, ct);
        await conversationRepo.SaveChangesAsync(ct);

        await notifications.NotifyConversationAssignedAsync(conversation.Id, agent.Id, ct);

        return conversation.ToDto();
    }
}