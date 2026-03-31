using Application.Features.Conversations.Commands;
using Application.Features.Conversations.Queries;
using Application.Features.Messages.Commands;
using Application.Features.Messages.Queries;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ConversationController(ISender mediator) : ControllerBase
{
    private Guid AgentId => Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

    [HttpGet("queue")]
    public async Task<IActionResult> GetQueue(CancellationToken ct)
    {
        var result = await mediator.Send(new GetQueueQuery(), ct);
        return Ok(result);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine(CancellationToken ct)
    {
        var result = await mediator.Send(new GetMyConversationsQuery(AgentId), ct);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/assign")]
    public async Task<IActionResult> Assign(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new AssignConversationCommand(id, AgentId), ct);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid id, ResolveRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new ResolveConversationCommand(id, AgentId, request.CategoryId), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetMessageQuery(id, AgentId), ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/messages")]
    public async Task<IActionResult> SendMessage(Guid id, SendMessageRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new SendMessageCommand(id, AgentId, request.Content, request.Type, request.MediaUrl), ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/messages/{messageId:guid}/react")]
    public async Task<IActionResult> React(System.Guid id, Guid messageId, ReactRequest request, CancellationToken ct)
    {
        await mediator.Send(new SendReactionCommand(messageId, AgentId, request.Emoji), ct);
        return Ok();
    }
}

public record SendMessageRequest(string Content, MessageType Type, string? MediaUrl);
public record ReactRequest(string Emoji);
public record ResolveRequest(Guid CategoryId);
