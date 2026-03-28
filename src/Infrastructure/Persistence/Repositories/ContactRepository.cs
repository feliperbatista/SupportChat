using System;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ContactRepository(AppDbContext db) : IContactRepository
{
    public async Task AddAsync(Contact contact, CancellationToken ct = default)
        => await db.Contacts.AddAsync(contact, ct);

    public async Task<Contact?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Contacts.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Contact?> GetByPhoneAsync(string phone, CancellationToken ct = default)
        => await db.Contacts.FirstOrDefaultAsync(c => c.PhoneNumber == phone, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public Task UpdateAsync(Contact contact, CancellationToken ct = default)
    {
        db.Contacts.Update(contact);
        return Task.CompletedTask;
    }
}
