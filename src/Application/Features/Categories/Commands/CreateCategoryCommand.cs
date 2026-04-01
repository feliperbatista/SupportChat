using System;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Categories.Commands;

public record CreateCategoryCommand(string Name, Guid DepartmentId) : IRequest<CategoryDto>;

public class CreateCategoryCommandHandler(
    IConversationCategoryRepository conversationCategoryRepo,
    IDepartmentRepository departmentRepo
) : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        var department = await departmentRepo.GetByIdAsync(request.DepartmentId, ct)
            ?? throw new KeyNotFoundException("Department not found");

        var category = ConversationCategory.Create(request.Name, request.DepartmentId);

        await conversationCategoryRepo.AddAsync(category, ct);
        await conversationCategoryRepo.SaveChangesAsync(ct);

        return category.ToDto();
    }
}