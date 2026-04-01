using System;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Categories.Commands;

public record DeleteCategoryCommand(Guid CategoryId) : IRequest;

public class DeleteCategoryCommandHandler(
    IConversationCategoryRepository conversationCategoryRepo
) : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var category = await conversationCategoryRepo.GetByIdAsync(request.CategoryId, ct)
            ?? throw new KeyNotFoundException("Category not found");

        await conversationCategoryRepo.DeleteAsync(category, ct);
        await conversationCategoryRepo.SaveChangesAsync(ct);
    }
}