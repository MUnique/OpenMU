// <copyright file="InMemoryEventPublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// An <see cref="IEventPublisher"/> which publishes directly to the available started servers in this application.
/// </summary>
public class InMemoryEventPublisher : IEventPublisher
{
    private readonly IDictionary<int, IGameServer> _gameServers;
    private readonly IFriendServer _friendServer;
    private readonly IGuildServer _guildServer;
    private readonly IReadOnlyCollection<IGameEventListener> _gameEventListeners;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryEventPublisher"/> class.
    /// </summary>
    /// <param name="gameServers">The game servers.</param>
    /// <param name="friendServer">The friend server.</param>
    /// <param name="guildServer">The guild server.</param>
    /// <param name="gameEventListeners">The listeners for the <see cref="GameEvent"/>s.</param>
    /// <param name="logger">The logger.</param>
    public InMemoryEventPublisher(IDictionary<int, IGameServer> gameServers, IFriendServer friendServer, IGuildServer guildServer, IEnumerable<IGameEventListener> gameEventListeners, ILogger<InMemoryEventPublisher> logger)
    {
        this._gameServers = gameServers;
        this._friendServer = friendServer;
        this._guildServer = guildServer;
        this._gameEventListeners = gameEventListeners.ToList();
        this._logger = logger;
    }

    /// <inheritdoc />
    public async ValueTask PlayerEnteredGameAsync(byte serverId, Guid characterId, string characterName)
    {
        await this._guildServer.PlayerEnteredGameAsync(characterId, characterName, serverId).ConfigureAwait(false);

        await this._friendServer.PlayerEnteredGameAsync(serverId, characterId, characterName).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask PlayerLeftGameAsync(byte serverId, Guid characterId, string characterName, uint guildId = 0)
    {
        if (guildId > 0)
        {
            await this._guildServer.GuildMemberLeftGameAsync(guildId, characterId, serverId).ConfigureAwait(false);
        }

        await this._friendServer.PlayerLeftGameAsync(characterId, characterName).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask GuildMessageAsync(uint guildId, string sender, string message)
    {
        foreach (var gameServer in this._gameServers)
        {
            await gameServer.Value.GuildChatMessageAsync(guildId, sender, message).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask AllianceMessageAsync(uint guildId, string sender, string message)
    {
        foreach (var gameServer in this._gameServers)
        {
            await gameServer.Value.AllianceChatMessageAsync(guildId, sender, message).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask WorldChatMessageAsync(string sender, string message)
    {
        foreach (var gameServer in this._gameServers)
        {
            await gameServer.Value.WorldChatMessageAsync(sender, message).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask PlayerAlreadyLoggedInAsync(byte serverId, string loginName)
    {
        foreach (var gameServer in this._gameServers)
        {
            await gameServer.Value.PlayerAlreadyLoggedInAsync(serverId, loginName).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask GameEventAsync(GameEvent gameEvent)
    {
        foreach (var listener in this._gameEventListeners)
        {
            try
            {
                await listener.OnGameEventAsync(gameEvent).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error when notifying the listener {listener} about the game event {gameEvent}", listener, gameEvent);
            }
        }
    }
}