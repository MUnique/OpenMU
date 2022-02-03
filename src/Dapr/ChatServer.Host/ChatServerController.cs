namespace MUnique.OpenMU.ChatServer.Host;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

[ApiController]
[Route("")]
public class ChatServerController : ControllerBase
{
    private readonly IChatServer _chatServer;

    private readonly ILogger<ChatServerController> _logger;

    public ChatServerController(IChatServer chatServer, ILogger<ChatServerController> logger)
    {
        this._chatServer = chatServer;
        this._logger = logger;
    }

    [HttpPost(nameof(IChatServer.RegisterClient))]
    public ChatServerAuthenticationInfo RegisterClient([FromBody] RegisterChatClientArguments data)
    {
        return this._chatServer.RegisterClient(data.RoomId, data.ClientName);
    }

    [HttpPost(nameof(IChatServer.CreateChatRoom))]
    public ushort CreateChatRoom()
    {
        return this._chatServer.CreateChatRoom();
    }
}