using System;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Departments.Queries;

public record GetDepartmentById(Guid Id) : IRequest<DepartmentDto>;

public class GetDepartmentByIdHandler(
    IDepartmentRepository departmentRepo
) : IRequestHandler<GetDepartmentById, DepartmentDto>
{
    public async Task<DepartmentDto> Handle(GetDepartmentById request, CancellationToken ct)
    {
        var department = await departmentRepo.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Department not found");

        return department.ToDto();
    }
}