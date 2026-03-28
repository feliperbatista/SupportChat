using System;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ConversationRepository(AppDbContext db) : IConversationRepository
{
    private IQueryable<Conversation> WithIncludes()
        => db.Conversations
            .Include(c => c.Contact)
            .Include(c => c.AssignedAgent)
            .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .ThenInclude(m => m.Reactions);

    public async Task AddAsync(Conversation conversation, CancellationToken ct = default)
        => await db.Conversations.AddAsync(conversation, ct);

    public async Task<IEnumerable<Conversation>> GetByAgentIdAsync(Guid agentId, CancellationToken ct = default)
        => await WithIncludes()
            .Where(c => c.AssignedAgentId == agentId &&
                c.Status == Domain.Enums.ConversationStatus.Open)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync(ct);

    public Task<Conversation?> GetByContactPhoneAsync(string phone, CancellationToken ct = default)
        => db.Conversations
            .Include(c => c.Contact)
            .Include(c => c.AssignedAgent)
            .Include(c => c.Messages)
                .ThenInclude(m => m.Reactions)
            .FirstOrDefaultAsync(c => 
                c.Contact.PhoneNumber == phone &&
                c.Status != Domain.Enums.ConversationStatus.Resolved &&
                c.Status != Domain.Enums.ConversationStatus.Closed, ct);

    public Task<Conversation?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Conversations
            .Include(c => c.Contact)
            .Include(c => c.AssignedAgent)
            .Include(c => c.Messages)
                .ThenInclude(m => m.Reactions)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IEnumerable<Conversation>> GetQueueAsync(CancellationToken ct = default)
        => await WithIncludes()
            .Where(c => c.AssignedAgentId == null &&
                c.Status == Domain.Enums.ConversationStatus.Open)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public Task UpdateAsync(Conversation conversation, CancellationToken ct = default)
    {
        db.Conversations.Update(conversation);
        return Task.CompletedTask;
    }
}
