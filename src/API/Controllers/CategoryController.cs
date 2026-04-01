using Application.Features.Categories.Commands;
using Application.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ISender mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var categories = await mediator.Send(new GetAllCategoriesQuery(), ct);

            return Ok(categories);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var category = await mediator.Send(new GetCategoryByIdCommand(id), ct);

            return Ok(category);
        }

        [Authorize(Policy = "CanManageCreation")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryRequest request, CancellationToken ct)
        {
            var category = await mediator.Send(new CreateCategoryCommand(request.Name, request.DepartmentId), ct);

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [Authorize(Policy = "CanManageCreation")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Remove(Guid id, CancellationToken ct)
        {
            await mediator.Send(new DeleteCategoryCommand(id), ct);

            return NoContent();
        }
    }

    public record CreateCategoryRequest(string Name, Guid DepartmentId);
}
