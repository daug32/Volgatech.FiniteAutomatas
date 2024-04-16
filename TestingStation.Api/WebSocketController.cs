using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace TestingStation.Api;

[ApiController]
[Route( "" )]
public class WebSocketController : ControllerBase
{
}

public class ChatHub : Hub
{
    public async Task NewMessage( string message )
    {
        await Clients.All.SendAsync(
            method: "chat_newmessage",
            arg1: message );
    }
}