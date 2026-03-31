using Application.Interfaces;
using MediatR;

namespace Application.Features.Departments.Commands;

public record DeleteDepartmentCommand(Guid Id) : IRequest;

public class DeleteDepartmentCommandHandler(
    IDepartmentRepository departmentRepo
) : IRequestHandler<DeleteDepartmentCommand>
{
    public async Task Handle(DeleteDepartmentCommand request, CancellationToken ct)
    {
        var department = await departmentRepo.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Department not found");

        await departmentRepo.DeleteAsync(department, ct);
        await departmentRepo.SaveChangesAsync(ct);
    }
}