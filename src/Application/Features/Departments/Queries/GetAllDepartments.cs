using System;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Departments.Queries;

public record GetAllDepartments : IRequest<IEnumerable<DepartmentDto>>;

public class GetAllDepartmentsHandler(
    IDepartmentRepository departmentRepo
) : IRequestHandler<GetAllDepartments, IEnumerable<DepartmentDto>>
{
    public async Task<IEnumerable<DepartmentDto>> Handle(GetAllDepartments request, CancellationToken ct)
    {
        var departments = await departmentRepo.GetAllAsync(ct);
        return departments.Select(d => d.ToDto());
    }
}