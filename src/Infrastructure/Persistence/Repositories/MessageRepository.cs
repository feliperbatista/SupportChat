using System;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MessageRepository(AppDbContext db) : IMessageRepository
{
    public async Task AddAsync(Message message, CancellationToken ct = default)
        => await db.Messages.AddAsync(message, ct);

    public async Task AddReactionAsync(Reaction reaction, CancellationToken ct = default)
        => await db.Reactions.AddAsync(reaction, ct);

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

    public async Task RemoveReactionAsync(Guid messageId, string fromPhoneNumber, CancellationToken ct = default)
    {
        var reactions = await db.Reactions
            .Where(r => r.MessageId == messageId &&
                r.FromPhoneNumber == fromPhoneNumber &&
                !r.IsFromAgent)
            .ToListAsync(ct);

        if (reactions.Count != 0)
            db.Reactions.RemoveRange(reactions);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public Task UpdateAsync(Message message, CancellationToken ct = default)
    {
        db.Messages.Update(message);
        return Task.CompletedTask;
    }
}
