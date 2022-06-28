// <copyright file="ChatServerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.Host;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// The API controller for the <see cref="IChatServer"/>.
/// </summary>
[ApiController]
[Route("")]
public class ChatServerController : ControllerBase
{
    private readonly IChatServer _chatServer;

    private readonly ILogger<ChatServerController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServerController"/> class.
    /// </summary>
    /// <param name="chatServer">The chat server.</param>
    /// <param name="logger">The logger.</param>
    public ChatServerController(IChatServer chatServer, ILogger<ChatServerController> logger)
    {
        this._chatServer = chatServer;
        this._logger = logger;
    }

    /// <summary>
    /// Registers the client to the server.
    /// </summary>
    /// <param name="data">The registration arguments.</param>
    /// <returns>The authentication info.</returns>
    [HttpPost(nameof(IChatServer.RegisterClient))]
    public ChatServerAuthenticationInfo RegisterClient([FromBody] RegisterChatClientArguments data)
    {
        try
        {
            return this._chatServer.RegisterClient(data.RoomId, data.ClientName);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, $"Error during registration of {data.ClientName} for room {data.RoomId}.");
            throw;
        }
    }

    /// <summary>
    /// Creates the chat room.
    /// </summary>
    /// <returns>The id of the created chat room.</returns>
    [HttpPost(nameof(IChatServer.CreateChatRoom))]
    public ushort CreateChatRoom()
    {
        try
        {
            return this._chatServer.CreateChatRoom();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error creating a new chat room.");
            throw;
        }
    }
}