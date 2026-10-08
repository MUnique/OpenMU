// <copyright file="EventController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.CentralServer.Host;

using global::Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// Handles the events which are published by the game servers.
/// </summary>
/// <remarks>
/// Dapr subscribes an app only once to a topic, so the events which are of interest
/// for multiple servers of this process are dispatched here.
/// A failing server doesn't fail the request, because the event would then be delivered again
/// to all servers - also to the ones which already handled it.
/// </remarks>
[ApiController]
[Route("events")]
public class EventController : ControllerBase
{
    private readonly IGuildServer _guildServer;
    private readonly IFriendServer _friendServer;
    private readonly GameServerRegistry _registry;
    private readonly ILogger<EventController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventController"/> class.
    /// </summary>
    /// <param name="guildServer">The guild server.</param>
    /// <param name="friendServer">The friend server.</param>
    /// <param name="registry">The game server registry.</param>
    /// <param name="logger">The logger.</param>
    public EventController(IGuildServer guildServer, IFriendServer friendServer, GameServerRegistry registry, ILogger<EventController> logger)
    {
        this._guildServer = guildServer;
        this._friendServer = friendServer;
        this._registry = registry;
        this._logger = logger;
    }

    /// <summary>
    /// Notifies the guild and friend server that a player entered the game.
    /// </summary>
    /// <param name="data">The arguments of the player.</param>
    [Topic("pubsub", nameof(IEventPublisher.PlayerEnteredGameAsync))]
    [HttpPost(nameof(IEventPublisher.PlayerEnteredGameAsync))]
    public async Task PlayerEnteredGameAsync([FromBody] PlayerOnlineStateArguments data)
    {
        try
        {
            await this._guildServer.PlayerEnteredGameAsync(data.CharacterId, data.CharacterName, data.ServerId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when notifying the guild server that player {0} entered the game.", data.CharacterName);
        }

        try
        {
            await this._friendServer.PlayerEnteredGameAsync(data.ServerId, data.CharacterId, data.CharacterName).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when notifying the friend server that player {0} entered the game.", data.CharacterName);
        }
    }

    /// <summary>
    /// Notifies the guild and friend server that a player left the game.
    /// </summary>
    /// <param name="data">The arguments of the player.</param>
    [Topic("pubsub", nameof(IEventPublisher.PlayerLeftGameAsync))]
    [HttpPost(nameof(IEventPublisher.PlayerLeftGameAsync))]
    public async Task PlayerLeftGameAsync([FromBody] PlayerOnlineStateArguments data)
    {
        try
        {
            await this._guildServer.GuildMemberLeftGameAsync(data.GuildId, data.CharacterId, data.ServerId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when notifying the guild server that player {0} left the game.", data.CharacterName);
        }

        try
        {
            await this._friendServer.PlayerLeftGameAsync(data.CharacterId, data.CharacterName).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when notifying the friend server that player {0} left the game.", data.CharacterName);
        }
    }

    /// <summary>
    /// Handles the heartbeat of a game server by updating the registry.
    /// </summary>
    /// <param name="data">The heartbeat.</param>
    [Topic("pubsub", "GameServerHeartbeat")]
    [HttpPost("GameServerHeartbeat")]
    public async Task GameServerHeartbeatAsync([FromBody] GameServerHeartbeatArguments data)
    {
        try
        {
            await this._registry.UpdateRegistrationAsync(data).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error updating the game server registry");
        }
    }
}
