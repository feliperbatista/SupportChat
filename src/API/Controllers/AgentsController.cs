using Application.Features.Agents.Commands;
using Application.Features.Agents.Queries;
using Domain.Enums;
using Humanizer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AgentsController(ISender mediator) : ControllerBase
{
    private Guid AgentId => Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllAgentsQuery(), ct);
        return Ok(result);
    }

    [Authorize]
    [HttpPatch("status")]
    public async Task<IActionResult> UpdateStatus(UpdateAgentStatusRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateAgentStatusCommand(AgentId, request.AgentStatus), ct);
        return NoContent();
    }

    public record UpdateAgentStatusRequest(AgentStatus AgentStatus);
}