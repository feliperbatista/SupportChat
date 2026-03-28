using System;
using Domain.Entities;

namespace Application.Interfaces;

public interface IAgentRepository
{
    Task<Agent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Agent?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<IEnumerable<Agent>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(Agent agent, CancellationToken ct = default);
    Task UpdateAsync(Agent agent, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
