using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR;

public class SubClaimUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
        => connection.User?.FindFirst("sub")?.Value;
}