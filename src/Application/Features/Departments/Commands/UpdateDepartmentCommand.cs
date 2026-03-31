using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Departments.Commands;

public record UpdateDepartmentCommand(Guid Id, string Name) : IRequest;

public class UpdateDepartmentCommandHandler(
    IDepartmentRepository departmentRepo
) : IRequestHandler<UpdateDepartmentCommand>
{
    public async Task Handle(UpdateDepartmentCommand request, CancellationToken ct)
    {
        var department = await departmentRepo.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Department not found");

        department.UpdateDepartment(request.Name);

        await departmentRepo.UpdateAsync(department, ct);
        await departmentRepo.SaveChangesAsync(ct);
    }
}