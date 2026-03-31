using System;
using Domain.Entities;

namespace Application.Interfaces;

public interface IConversationCategoryRepository
{
    Task<IEnumerable<ConversationCategory>> GetAllAsync(CancellationToken ct = default);
    Task<ConversationCategory?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(ConversationCategory conversationCategory, CancellationToken ct = default);
    Task DeleteAsync(ConversationCategory conversationCategory, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);      
}
