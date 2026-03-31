using System;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class DepartmentRepository(AppDbContext db) : IDepartmentRepository
{
    public async Task AddAsync(Department department, CancellationToken ct = default)
        => await db.Departments.AddAsync(department, ct);

    public Task DeleteAsync(Department department, CancellationToken ct = default)
    {
        db.Departments.Remove(department);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken ct = default)
        => await db.Departments.OrderBy(d => d.Name).ToListAsync(ct);

    public async Task<Department?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken: ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await db.SaveChangesAsync(ct);

    public Task UpdateAsync(Department department, CancellationToken ct = default)
    {
        db.Departments.Update(department);
        return Task.CompletedTask;
    }
}
