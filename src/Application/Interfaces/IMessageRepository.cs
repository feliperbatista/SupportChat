using System;
using Domain.Entities;

namespace Application.Interfaces;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Message?> GetByWhatsAppIdAsync(string whatsAppMessageId, CancellationToken ct = default);
    Task<IEnumerable<Message>> GetByConversationIdAsync(Guid conversationId, CancellationToken ct = default);
    Task AddAsync(Message message, CancellationToken ct = default);
    Task UpdateAsync(Message message, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
