using System;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Messages.Commands;

public record SendMessageCommand(
    Guid ConversationId,
    Guid AgentId,
    string Content,
    MessageType Type,
    string? MediaUrl = null
) : IRequest<MessageDto>;

public class SendMessageCommandHandler(
    IConversationRepository conversationRepo,
    IMessageRepository messageRepo,
    IWhatsAppService whatsApp,
    INotificationService notifications
) : IRequestHandler<SendMessageCommand, MessageDto>
{
    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken ct)
    {
        var conversation = await conversationRepo.GetByIdAsync(request.ConversationId, ct)
            ?? throw new KeyNotFoundException("Conversation not found");

        if (conversation.AssignedAgentId != request.AgentId)
            throw new UnauthorizedAccessException("You are not assigned to this conversation.");

        var message = Message.CreateOutgoing(
            request.ConversationId,
            request.Content,
            request.Type,
            request.AgentId,
            request.MediaUrl
        );

        await messageRepo.AddAsync(message, ct);
        await messageRepo.SaveChangesAsync(ct);

        var phoneNumber = conversation.Contact.PhoneNumber;
        var waMessageId = request.Type switch
        {
            MessageType.Text => await whatsApp.SendTextAsync(phoneNumber, request.Content, ct),
            MessageType.Image => await whatsApp.SendImageAsync(phoneNumber, request.MediaUrl!, request.Content, ct),
            MessageType.Audio => await whatsApp.SendAudioAsync(phoneNumber, request.MediaUrl!, ct),
            MessageType.Document => await whatsApp.SendDocumentAsync(phoneNumber, request.MediaUrl!, request.Content, ct),
            _ => throw new NotSupportedException($"Message type {request.Type} not supported for outgoind.")
        };

        message.SetWhatsAppId(waMessageId);
        message.UpdateStatus(MessageStatus.Sent);
        await messageRepo.UpdateAsync(message, ct);
        await messageRepo.SaveChangesAsync(ct);

        var dto = message.ToDto();
        await notifications.NotifyNewMessageAsync(conversation.Id, request.AgentId, dto, ct);

        return dto;
    }
}