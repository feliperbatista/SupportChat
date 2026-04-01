using System;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR;

public class NotificationService(IHubContext<ChatHub> hub) : INotificationService
{
    public async Task NotifyAgentStatusAsync(Guid agentId, string status, CancellationToken ct = default)
        => await hub.Clients
            .Group($"inbox")
            .SendAsync("AgentStatusChanged", new { agentId, status }, ct);

    public async Task NotifyConversationAssignedAsync(Guid conversationId, Guid agentId, CancellationToken ct = default)
        => await hub.Clients
            .Group($"agent-{agentId}")
            .SendAsync("ConversationAssigned", new { conversationId }, ct);

    public async Task NotifyConversationQueuedAsync(ConversationDto conversation, CancellationToken ct = default)
        => await hub.Clients
            .Group("inbox")
            .SendAsync("ConversationQueued", conversation, ct);

    public async Task NotifyMessageStatusAsync(Guid conversationId, string whatsAppMessageId, string status, CancellationToken ct = default)
        => await hub.Clients
            .Group($"conv-{conversationId}")
            .SendAsync("MessageStatusUpdated", new { whatsAppMessageId, status }, ct);

    public async Task NotifyNewMessageAsync(Guid conversationId, Guid? agentId, MessageDto message, CancellationToken ct = default)
    {
        await hub.Clients
            .Group($"conv-{conversationId}")
            .SendAsync("NewMessage", message, ct);

        if (agentId.HasValue)
            await hub.Clients
            .Group($"agent-{agentId}")
            .SendAsync("NewMessage", message, ct);
    }

    public async Task NotifyReactionAsync(Guid conversationId, Guid messageId, string emoji, string from, CancellationToken ct = default)
        => await hub.Clients
            .Group($"conv-{conversationId}")
            .SendAsync("ReactionReceived", new { messageId, emoji, from }, ct);

    public async Task NotifyReactionRemovedAsync(Guid conversationId, Guid messageId, string from, CancellationToken ct = default)
        => await hub.Clients
            .Group($"conv-{conversationId}")
            .SendAsync("ReactionRemoved", new { messageId, from }, ct);
}
