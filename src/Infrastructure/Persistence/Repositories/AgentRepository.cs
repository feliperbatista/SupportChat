using System;
using System.Data.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AgentRepository(AppDbContext db) : IAgentRepository
{
    public async Task AddAsync(Agent agent, CancellationToken ct = default)
        => await db.Agents.AddAsync(agent, ct);

    public async Task<IEnumerable<Agent>> GetAllActiveAsync(CancellationToken ct = default)
        => await db.Agents.Where(a => a.IsActive).ToListAsync(ct);

    public async Task<Agent?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await db.Agents.FirstOrDefaultAsync(a => a.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase), ct);

    public async Task<Agent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Agents.FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public Task UpdateAsync(Agent agent, CancellationToken ct = default)
    {
        db.Agents.Update(agent);
        return Task.CompletedTask;
    }
}
