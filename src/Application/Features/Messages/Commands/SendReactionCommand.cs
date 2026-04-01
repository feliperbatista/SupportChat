using System;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Messages.Commands;

public record SendReactionCommand(
    Guid MessageId,
    Guid AgentId,
    string Emoji
) : IRequest;

public class SendReactionCommandHandler(
    IMessageRepository messageRepo,
    IConversationRepository conversationRepo,
    IWhatsAppService whatsApp,
    INotificationService notifications
) : IRequestHandler<SendReactionCommand>
{
    public async Task Handle(SendReactionCommand request, CancellationToken ct)
    {
        var message = await messageRepo.GetByIdAsync(request.MessageId, ct)
            ?? throw new KeyNotFoundException("Message not found.");

        var conversation = await conversationRepo.GetByIdAsync(message.ConversationId, ct)
            ?? throw new KeyNotFoundException("Conversation not found.");

        if (string.IsNullOrEmpty(message.WhatsAppMessageId))
            throw new InvalidOperationException("Cannot react to a message without a WhatsApp ID.");

        var existing = message.Reactions
            .FirstOrDefault(r => r.Emoji == request.Emoji && r.IsFromAgent);

        if (existing is not null)
            return;

        var reaction = Reaction.Create(request.MessageId, request.Emoji, "agent", isFromAgent: true);
        await messageRepo.AddReactionAsync(reaction, ct);
        await messageRepo.SaveChangesAsync(ct);

        await whatsApp.SendReactionAsync(conversation.Contact.PhoneNumber, message.WhatsAppMessageId, request.Emoji, ct);
        await notifications.NotifyReactionAsync(conversation.Id, message.Id, request.Emoji, "agent", ct);
    }
}