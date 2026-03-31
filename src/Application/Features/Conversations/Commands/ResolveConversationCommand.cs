using System;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Conversations.Commands;

public record ResolveConversationCommand(Guid ConversationId, Guid AgentId, Guid CategoryId) : IRequest<ConversationDto>;

public class ResolveConversationCommandHandler(
    IConversationRepository conversationRepo,
    IConversationCategoryRepository conversationCategoryRepo
) : IRequestHandler<ResolveConversationCommand, ConversationDto>
{
    public async Task<ConversationDto> Handle(ResolveConversationCommand request, CancellationToken ct)
    {
        var conversation = await conversationRepo.GetByIdAsync(request.ConversationId, ct)
            ?? throw new KeyNotFoundException("Conversation not found.");

        var category = await conversationCategoryRepo.GetByIdAsync(request.CategoryId, ct)
            ?? throw new KeyNotFoundException("Category not found.");

        if (conversation.AssignedAgentId != request.AgentId)
            throw new UnauthorizedAccessException("You are not assigned to this conversation.");

        conversation.Resolve(category.Id);
        await conversationRepo.UpdateAsync(conversation, ct);
        await conversationRepo.SaveChangesAsync(ct);

        return conversation.ToDto();
    }
}