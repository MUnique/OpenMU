// <copyright file="GuildChangePublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;

namespace MUnique.OpenMU.GuildServer.Host;

using global::Dapr.Client;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// Publisher for guild changes over Dapr.
/// </summary>
public class GuildChangePublisher : IGuildChangePublisher
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<GuildChangePublisher> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildChangePublisher" /> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="logger">The logger.</param>
    public GuildChangePublisher(DaprClient daprClient, ILogger<GuildChangePublisher> logger)
    {
        this._daprClient = daprClient;
        this._logger = logger;
    }

    /// <inheritdoc />
    public async ValueTask GuildPlayerKickedAsync(string playerName)
    {
        try
        {
            await this._daprClient.PublishEventAsync("pubsub", nameof(IGameServer.GuildPlayerKickedAsync), playerName);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.GuildPlayerKickedAsync));
        }
    }

    /// <inheritdoc />
    public async ValueTask GuildDeletedAsync(uint guildId)
    {
        try
        {
            await this._daprClient.PublishEventAsync("pubsub", nameof(IGameServer.GuildDeletedAsync), guildId);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.GuildDeletedAsync));
        }
    }

    /// <inheritdoc />
    public async ValueTask AssignGuildToPlayerAsync(byte serverId, string characterName, GuildMemberStatus status)
    {
        try
        {
            await this._daprClient.InvokeMethodAsync($"gameServer{serverId + 1}", nameof(IGameServer.AssignGuildToPlayerAsync), new GuildMemberAssignArguments(characterName, status));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.AssignGuildToPlayerAsync));
        }
    }
}