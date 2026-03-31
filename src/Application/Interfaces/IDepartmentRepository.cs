using System;
using Domain.Entities;

namespace Application.Interfaces;

public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetAllAsync(CancellationToken ct = default);
    Task<Department?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Department department, CancellationToken ct = default);
    Task UpdateAsync(Department department, CancellationToken ct = default);
    Task DeleteAsync(Department department, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
