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
    Stream? FileStream = null,
    string? FileName = null
) : IRequest<MessageDto>;

public class SendMessageCommandHandler(
    IConversationRepository conversationRepo,
    IMessageRepository messageRepo,
    IWhatsAppService whatsApp,
    INotificationService notifications,
    IAudioConverter audioConverter,
    IAzureBlobService azureBlob
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
            request.AgentId
        );

        await messageRepo.AddAsync(message, ct);
        await messageRepo.SaveChangesAsync(ct);

        var phoneNumber = conversation.Contact.PhoneNumber;

        string? mediaId = null;
        string? mediaUrl = null;

        if (request.FileStream is not null)
        {
            using var originalStream = new MemoryStream();
            await request.FileStream.CopyToAsync(originalStream, ct);
            originalStream.Position = 0;

            Stream fileToUpload = originalStream;
            string fileName = request.FileName!;

            if (request.Type == MessageType.Audio)
            {
                fileToUpload = await audioConverter.ConvertWebMToOggAsync(originalStream, ct);
                fileName = Path.ChangeExtension(fileName, ".ogg");
            }

            using var waStream = new MemoryStream();
            await fileToUpload.CopyToAsync(waStream, ct);
            waStream.Position = 0;

            using var blobStream = new MemoryStream();
            fileToUpload.Position = 0;
            await fileToUpload.CopyToAsync(blobStream, ct);
            blobStream.Position = 0;

            mediaId = await whatsApp.UploadMedia(waStream, fileName, ct);

            if (fileToUpload.CanSeek)
                fileToUpload.Position = 0;

            mediaUrl = await azureBlob.UploadBlob(blobStream, fileName, ct);
        }

        var waMessageId = request.Type switch
        {
            MessageType.Text => await whatsApp.SendTextAsync(phoneNumber, request.Content, ct),
            MessageType.Image => await whatsApp.SendImageAsync(phoneNumber, mediaId!, request.Content, ct),
            MessageType.Audio => await whatsApp.SendAudioAsync(phoneNumber, mediaId!, ct),
            MessageType.Document => await whatsApp.SendDocumentAsync(phoneNumber, mediaId!, request.Content, ct),
            MessageType.Video => await whatsApp.SendVideoAsync(phoneNumber, mediaId!, ct),
            MessageType.Sticker => await whatsApp.SendStickerAsync(phoneNumber, mediaId!, ct),
            MessageType.Reaction => await whatsApp.SendStickerAsync(phoneNumber, mediaId!, ct),
            _ => throw new NotSupportedException($"Message type {request.Type} not supported for outgoing.")
        };

        message.SetWhatsAppId(waMessageId, mediaId, mediaUrl);
        message.UpdateStatus(MessageStatus.Sent);
        await messageRepo.UpdateAsync(message, ct);
        await messageRepo.SaveChangesAsync(ct);

        var dto = message.ToDto();
        await notifications.NotifyNewMessageAsync(conversation.Id, request.AgentId, dto, ct);

        return dto;
    }
}