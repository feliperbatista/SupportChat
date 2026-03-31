using System;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ConversationCategoryRepository(AppDbContext db) : IConversationCategoryRepository
{
    public async Task AddAsync(ConversationCategory ConversationCategory, CancellationToken ct = default)
        => await db.ConversationCategorys.AddAsync(ConversationCategory, ct);

    public Task DeleteAsync(ConversationCategory ConversationCategory, CancellationToken ct = default)
    {
        db.ConversationCategorys.Remove(ConversationCategory);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<ConversationCategory>> GetAllAsync(CancellationToken ct = default)
        => await db.ConversationCategorys.OrderBy(d => d.Name).ToListAsync(ct);

    public async Task<ConversationCategory?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.ConversationCategorys.FirstOrDefaultAsync(d => d.Id == id, cancellationToken: ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await db.SaveChangesAsync(ct);
}
