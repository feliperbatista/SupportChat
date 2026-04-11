using System;
using Application.DTOs;

namespace Application.Interfaces;

public interface INotificationService
{
    Task NotifyNewMessageAsync(Guid conversationId, Guid? agentId, MessageDto message, CancellationToken ct = default);
    Task NotifyMessageStatusAsync(Guid conversationId, string whatsAppMessageId, string status, CancellationToken ct = default);
    Task NotifyReactionAsync(Guid conversationId, Guid messageId, string emoji, string from, CancellationToken ct = default);
    Task NotifyReactionRemovedAsync(Guid conversationId, Guid messageId, string from, CancellationToken ct = default);
    Task NotifyConversationAssignedAsync(Guid conversationId, Guid agentId, CancellationToken ct = default);
    Task NotifyConversationQueuedAsync(ConversationDto conversation, CancellationToken ct = default);
    Task NotifyAgentStatusAsync(Guid agentId, string status, CancellationToken ct = default);
    Task NotifyContactNameUpdatedAsync(Guid contactId, string name, CancellationToken ct = default);
}
