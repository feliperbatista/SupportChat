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

        var reacttion = Reaction.Create(request.MessageId, request.Emoji, "ageent", isFromAgent: true);
        message.Reactions.Add(reacttion);
        await messageRepo.UpdateAsync(message, ct);
        await messageRepo.SaveChangesAsync(ct);

        await whatsApp.SendReactionAsync(conversation.Contact.PhoneNumber, message.WhatsAppMessageId, request.Emoji, ct);
        await notifications.NotifyReactionAsync(conversation.Id, message.Id, request.Emoji, "agent", ct);
    }
}