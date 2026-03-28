using System;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MessageRepository(AppDbContext db) : IMessageRepository
{
    public async Task AddAsync(Message message, CancellationToken ct = default)
        => await db.Messages.AddAsync(message, ct);

    public async Task<IEnumerable<Message>> GetByConversationIdAsync(Guid conversationId, CancellationToken ct = default)
        => await db.Messages
            .Include(m => m.Reactions)
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(ct);

    public Task<Message?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Messages
            .Include(m => m.Reactions)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

    public Task<Message?> GetByWhatsAppIdAsync(string whatsAppMessageId, CancellationToken ct = default)
        => db.Messages
            .Include(m => m.Reactions)
            .FirstOrDefaultAsync(m => m.WhatsAppMessageId == whatsAppMessageId, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public Task UpdateAsync(Message message, CancellationToken ct = default)
    {
        db.Messages.Update(message);
        return Task.CompletedTask;
    }
}
