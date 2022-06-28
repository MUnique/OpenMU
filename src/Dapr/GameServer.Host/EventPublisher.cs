// <copyright file="EventPublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.Host;

using global::Dapr.Client;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;
using Nito.AsyncEx.Synchronous;

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
    public void PlayerEnteredGame(byte serverId, Guid characterId, string characterName)
    {
        try
        {
            this._daprClient
                .PublishEventAsync(
                    PubSubName,
                    nameof(IEventPublisher.PlayerEnteredGame),
                    new PlayerOnlineStateArguments(characterId, characterName, serverId))
                .WaitAndUnwrapException();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when publishing a guild message");
        }
    }

    /// <inheritdoc />
    public void PlayerLeftGame(byte serverId, Guid characterId, string characterName, uint guildId = 0)
    {
        try
        {
            this._daprClient
                .PublishEventAsync(
                    PubSubName,
                    nameof(IEventPublisher.PlayerLeftGame),
                    new PlayerOnlineStateArguments(characterId, characterName, serverId, guildId))
                .WaitAndUnwrapException();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when publishing a guild message");
        }
    }

    /// <inheritdoc />
    public void GuildMessage(uint guildId, string sender, string message)
    {
        try
        {
            this._daprClient
                .PublishEventAsync(
                    PubSubName,
                    nameof(IGameServer.GuildChatMessage),
                    new GuildMessageArguments(guildId, sender, message))
                .WaitAndUnwrapException();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when publishing a guild message");
        }
    }

    /// <inheritdoc />
    public void AllianceMessage(uint guildId, string sender, string message)
    {
        try
        {
            this._daprClient
                .PublishEventAsync(
                    PubSubName,
                    nameof(IGameServer.AllianceChatMessage),
                    new GuildMessageArguments(guildId, sender, message))
                .WaitAndUnwrapException();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when publishing an alliance message");
        }
    }
}