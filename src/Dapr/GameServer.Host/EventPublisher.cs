// <copyright file="EventPublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.Host;

using global::Dapr.Client;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// Implementation of a <see cref="IEventPublisher"/> which publishes the events
/// through the Dapr pub/sub component.
/// </summary>
public class EventPublisher : IEventPublisher
{
    private const string PubSubName = "pubsub";
    private readonly DaprClient _daprClient;
    private readonly ILogger<GameServerStatePublisher> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventPublisher"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="logger">The logger.</param>
    public EventPublisher(DaprClient daprClient, ILogger<GameServerStatePublisher> logger)
    {
        this._daprClient = daprClient;
        this._logger = logger;
    }

    /// <inheritdoc />
    public async ValueTask PlayerEnteredGameAsync(byte serverId, Guid characterId, string characterName)
    {
        try
        {
            await this._daprClient
                .PublishEventAsync(
                    PubSubName,
                    nameof(IEventPublisher.PlayerEnteredGameAsync),
                    new PlayerOnlineStateArguments(characterId, characterName, serverId)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when publishing a guild message");
        }
    }

    /// <inheritdoc />
    public async ValueTask PlayerLeftGameAsync(byte serverId, Guid characterId, string characterName, uint guildId = 0)
    {
        try
        {
            await this._daprClient
                .PublishEventAsync(
                    PubSubName,
                    nameof(IEventPublisher.PlayerLeftGameAsync),
                    new PlayerOnlineStateArguments(characterId, characterName, serverId, guildId)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when publishing a guild message");
        }
    }

    /// <inheritdoc />
    public async ValueTask GuildMessageAsync(uint guildId, string sender, string message)
    {
        try
        {
            await this._daprClient
                .PublishEventAsync(
                    PubSubName,
                    nameof(IGameServer.GuildChatMessageAsync),
                    new GuildMessageArguments(guildId, sender, message)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when publishing a guild message");
        }
    }

    /// <inheritdoc />
    public async ValueTask AllianceMessageAsync(uint guildId, string sender, string message)
    {
        try
        {
            await this._daprClient
                .PublishEventAsync(
                    PubSubName,
                    nameof(IGameServer.AllianceChatMessageAsync),
                    new GuildMessageArguments(guildId, sender, message)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when publishing an alliance message");
        }
    }

    /// <summary>
    /// Notifies that a client tried to log into an already logged-in account.
    /// The connected player can be notified about that.
    /// </summary>
    /// <param name="serverId">The identifier of the server on which the client tried to enter.</param>
    /// <param name="loginName">The login name.</param>
    public async ValueTask PlayerAlreadyLoggedInAsync(byte serverId, string loginName)
    {
        try
        {
            await this._daprClient
                .PublishEventAsync(
                    PubSubName,
                    nameof(IGameServer.PlayerAlreadyLoggedInAsync),
                    new PlayerLoggedInArguments(serverId, loginName)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when publishing an alliance message");
        }
    }
}