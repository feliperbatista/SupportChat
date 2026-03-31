using Application.Features.Contacts.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController(ISender mediator) : ControllerBase
    {
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> UpdateName(Guid id, UpdateNameRequest request, CancellationToken ct)
        {
            var contact = await mediator.Send(new UpdateContactNameCommand(id, request.Name), ct);
            return Ok(contact);
        }
    }

    public record UpdateNameRequest(string Name);
}