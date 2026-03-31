using System;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Departments.Commands;

public record CreateDepartmentCommand(string Name) : IRequest<DepartmentDto>;

public class CreateDepartmentCommandHandler(
    IDepartmentRepository departmentRepo
) : IRequestHandler<CreateDepartmentCommand, DepartmentDto>
{
    public async Task<DepartmentDto> Handle(CreateDepartmentCommand request, CancellationToken ct)
    {
        var department = Department.Create(request.Name);

        await departmentRepo.AddAsync(department, ct);
        await departmentRepo.SaveChangesAsync(ct);

        return department.ToDto();
    }
}