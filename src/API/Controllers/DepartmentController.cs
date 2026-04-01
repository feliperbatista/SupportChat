using Application.Features.Departments.Commands;
using Application.Features.Departments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController(ISender mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var departments = await mediator.Send(new GetAllDepartments(), ct);
            return Ok(departments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var department = await mediator.Send(new GetDepartmentById(id), ct);
            return Ok(department);
        }

        [Authorize(Policy = "CanManageCreation")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id, CancellationToken ct)
        {
            await mediator.Send(new DeleteDepartmentCommand(id), ct);
            return NoContent();
        }

        [Authorize(Policy = "CanManageCreation")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateDepartmentRequest request, CancellationToken ct)
        {
            await mediator.Send(new UpdateDepartmentCommand(id, request.Name), ct);
            return NoContent();
        }

        [Authorize(Policy = "CanManageCreation")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateDepartmentRequest request, CancellationToken ct)
        {
            var department = await mediator.Send(new CreateDepartmentCommand(request.Name), ct);
            return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
        }
    }

    public record CreateDepartmentRequest(string Name);
    public record UpdateDepartmentRequest(string Name);
}
