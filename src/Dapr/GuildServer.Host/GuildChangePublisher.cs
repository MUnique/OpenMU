// <copyright file="GuildChangePublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GuildServer.Host;

using global::Dapr.Client;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.EntityFramework.Model;
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
            await this._daprClient.PublishEventAsync("pubsub", nameof(IGameServer.GuildPlayerKickedAsync), playerName).ConfigureAwait(false);
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
            await this._daprClient.PublishEventAsync("pubsub", nameof(IGameServer.GuildDeletedAsync), guildId).ConfigureAwait(false);
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
            await this._daprClient.InvokeMethodAsync($"gameServer{serverId + 1}", nameof(IGameServer.AssignGuildToPlayerAsync), new GuildMemberAssignArguments(characterName, status)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.AssignGuildToPlayerAsync));
        }
    }

    /// <inheritdoc />
    public async ValueTask AllianceCreatedAsync(uint masterGuildId, uint memberGuildId)
    {
        try
        {
            await this._daprClient.InvokeMethodAsync("pubsub", nameof(IGameServer.AssignGuildToPlayerAsync), (masterGuildId, memberGuildId)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.AllianceCreatedAsync));
        }
    }

    /// <inheritdoc />
    public async ValueTask AllianceDisbandedAsync(uint masterGuildId, uint memberGuildId)
    {
        try
        {
            await this._daprClient.InvokeMethodAsync("pubsub", nameof(IGameServer.AllianceDisbandedAsync), (masterGuildId, memberGuildId)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.AllianceDisbandedAsync));
        }
    }

    /// <inheritdoc />
    public async ValueTask GuildHostilityChangedAsync(uint guildIdA, IReadOnlyList<uint> allianceGuildIdsA, uint guildIdB, IReadOnlyList<uint> allianceGuildIdsB, bool created)
    {
        try
        {
            await this._daprClient.PublishEventAsync("pubsub", nameof(IGameServer.GuildHostilityChangedAsync), (guildIdA, allianceGuildIdsA, guildIdB, allianceGuildIdsB, created)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.GuildHostilityChangedAsync));
        }
    }
}