using System;
using Domain.Entities;

namespace Application.Interfaces;

public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Conversation?> GetByContactPhoneAsync(string phone, CancellationToken ct = default);
    Task<IEnumerable<Conversation>> GetQueueAsync(CancellationToken ct = default);
    Task<IEnumerable<Conversation>> GetByAgentIdAsync(Guid agentId, CancellationToken ct = default);
    Task AddAsync(Conversation conversation, CancellationToken ct = default);
    Task UpdateAsync(Conversation conversation, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
