using Application.Features.Agents.Commands;
using Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(ISender mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new LoginCommand(request.Email, request.Password), ct);

        Response.Cookies.Append("auth_token", result.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddHours(24),
            Path = "/"
        });

        return Ok(result.Agent);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("auth_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });

        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CreateAgentCommand(request.Name, request.Email, request.Password), ct);
        return Created(string.Empty, result);
    }

    [Authorize]
    [HttpGet("signalr-token")]
    public IActionResult GetSignalRToken()
    {
        var agentId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (agentId is null) return Unauthorized();

        if (Request.Cookies.TryGetValue("auth_token", out var token))
            return Ok(new { token });

        return Unauthorized();
    }

    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string Name, string Email, string Password);
}
