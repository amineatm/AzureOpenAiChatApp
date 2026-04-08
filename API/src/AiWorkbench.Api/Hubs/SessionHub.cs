using AiWorkbench.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AiWorkbench.Api.Hubs;

[Authorize]
public class SessionHub(IUserConnectionService connections) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value;
        if (userId != null)
        {
            connections.AddConnection(Guid.Parse(userId), Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst("sub")?.Value;
        if (userId != null)
        {
            connections.RemoveConnection(Guid.Parse(userId), Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
