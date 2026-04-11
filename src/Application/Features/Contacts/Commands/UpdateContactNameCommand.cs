using System;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Contacts.Commands;

public record UpdateContactNameCommand(Guid Id, string Name)
    : IRequest<ContactDto>;

public class UpdateContactNameCommandHandler(
    IContactRepository contactRepo,
    INotificationService notificationService
    ) : IRequestHandler<UpdateContactNameCommand, ContactDto>
{
    public async Task<ContactDto> Handle(UpdateContactNameCommand request, CancellationToken ct)
    {
        var contact = await contactRepo.GetByIdAsync(request.Id, ct) 
            ?? throw new KeyNotFoundException("Contact not found");

        contact.UpdateName(request.Name);
        await contactRepo.UpdateAsync(contact, ct);
        await contactRepo.SaveChangesAsync(ct);

        await notificationService.NotifyContactNameUpdatedAsync(contact.Id, contact.Name, ct);

        return contact.ToDto();
    }
}