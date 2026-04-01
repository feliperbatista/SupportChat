using System;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Categories.Queries;

public record GetCategoryByIdCommand(Guid CategoryId) : IRequest<CategoryDto>;

public class GetCategoryByIdCommandCommand(
    IConversationCategoryRepository conversationCategoryRepo
) : IRequestHandler<GetCategoryByIdCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(GetCategoryByIdCommand request, CancellationToken ct)
    {
        var category = await conversationCategoryRepo.GetByIdAsync(request.CategoryId, ct)
            ?? throw new KeyNotFoundException("Category not found");

        return category.ToDto();
    }
}
