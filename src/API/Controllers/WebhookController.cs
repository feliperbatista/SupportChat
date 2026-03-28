using System.Text.Json;
using Application.Features.Webhooks.Commands;
using Infrastructure.WhatsApp;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers;

[Route("webhook")]
[ApiController]
public class WebhookController(ISender mediator, IOptions<WhatsAppOptions> opts) : ControllerBase
{
    [HttpGet]
    public IActionResult Verify(
    [FromQuery(Name = "hub.mode")]         string mode,
    [FromQuery(Name = "hub.verify_token")] string token,
    [FromQuery(Name = "hub.challenge")]    string challenge)
    {
        if (mode == "subscribe" && token == opts.Value.VerifyToken)
            return Ok(challenge);

        return Forbid();
    }

    [HttpPost]
    public async Task<IActionResult> Receive(
    [FromBody] JsonElement payload,
    CancellationToken ct)
    {
        await mediator.Send(new ProcessWebhookCommand(payload), ct);
        return Ok();
    }
}