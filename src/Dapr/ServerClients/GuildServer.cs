// <copyright file="GuildServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using System.Collections.Immutable;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Implementation of an <see cref="IGuildServer"/> which accesses the guild server remotely over Dapr.
/// </summary>
public class GuildServer : IGuildServer
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<GuildServer> _logger;
    private readonly string _targetAppId;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildServer"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="logger">The logger.</param>
    public GuildServer(DaprClient daprClient, ILogger<GuildServer> logger)
    {
        this._daprClient = daprClient;
        this._logger = logger;
        this._targetAppId = "guildServer";
    }

    /// <inheritdoc />
    public async ValueTask<bool> GuildExistsAsync(string guildName)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<string, bool>(this._targetAppId, nameof(this.GuildExistsAsync), guildName).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when checking a guild existence.");
            throw;
        }
    }

    /// <inheritdoc />
    public async ValueTask<Guild?> GetGuildAsync(uint guildId)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<uint, Guild?>(this._targetAppId, nameof(this.GetGuildAsync), guildId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild retrieval message.");
            return null;
        }
    }

    /// <inheritdoc />
    public async ValueTask<uint> GetGuildIdByNameAsync(string guildName)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<string, uint>(this._targetAppId, nameof(this.GetGuildIdByNameAsync), guildName).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when getting the id by guild name.");
            return 0;
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> CreateGuildAsync(string name, string masterName, Guid masterId, byte[] logo, byte serverId)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<GuildCreationArguments, bool>(this._targetAppId, nameof(this.CreateGuildAsync), new GuildCreationArguments(name, masterName, masterId, logo, serverId)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild creation message.");
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask CreateGuildMemberAsync(uint guildId, Guid characterId, string characterName, GuildPosition role, byte serverId)
    {
        try
        {
            await this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(this.CreateGuildMemberAsync), new GuildMemberCreationArguments(guildId, characterId, characterName, role, serverId)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild member creation.");
        }
    }

    /// <inheritdoc />
    public async ValueTask ChangeGuildMemberPositionAsync(uint guildId, Guid characterId, GuildPosition role)
    {
        try
        {
            await this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(this.ChangeGuildMemberPositionAsync), new GuildMemberRoleChangeArguments(guildId, characterId, role)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild member position change.");
        }
    }

    /// <inheritdoc />
    public ValueTask PlayerEnteredGameAsync(Guid characterId, string characterName, byte serverId)
    {
        // Handled by EventPublisher, through pub/sub component.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask GuildMemberLeftGameAsync(uint guildId, Guid guildMemberId, byte serverId)
    {
        // Handled by EventPublisher, through pub/sub component.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public async ValueTask<IImmutableList<GuildListEntry>> GetGuildListAsync(uint guildId)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<uint, IImmutableList<GuildListEntry>>(this._targetAppId, nameof(this.GetGuildListAsync), guildId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when getting a guild list.");
            return ImmutableList<GuildListEntry>.Empty;
        }
    }

    /// <inheritdoc />
    public async ValueTask KickMemberAsync(uint guildId, string playerName)
    {
        try
        {
            await this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(this.KickMemberAsync), new GuildMemberArguments(guildId, playerName)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when kicking a guild member.");
        }
    }

    /// <inheritdoc />
    public async ValueTask<GuildPosition> GetGuildPositionAsync(Guid characterId)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<Guid, GuildPosition>(this._targetAppId, nameof(this.GetGuildPositionAsync), characterId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when retrieving a guild position.");
            return GuildPosition.Undefined;
        }
    }

    /// <inheritdoc />
    public async ValueTask IncreaseGuildScoreAsync(uint guildId)
    {
        try
        {
            await this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(this.IncreaseGuildScoreAsync), guildId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild score increase.");
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> CreateAllianceAsync(uint masterGuildId, uint targetGuildId)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<(uint, uint), bool>(this._targetAppId, nameof(this.CreateAllianceAsync), (masterGuildId, targetGuildId)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when creating an alliance.");
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> RemoveAllianceGuildAsync(uint masterGuildId, uint targetGuildId)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<(uint, uint), bool>(this._targetAppId, nameof(this.RemoveAllianceGuildAsync), (masterGuildId, targetGuildId)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when removing a guild from alliance.");
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> DisbandAllianceAsync(uint masterGuildId)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<uint, bool>(this._targetAppId, nameof(this.DisbandAllianceAsync), masterGuildId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when disbanding an alliance.");
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask<IImmutableList<AllianceGuildEntry>> GetAllianceGuildsAsync(uint guildId)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<uint, IImmutableList<AllianceGuildEntry>>(this._targetAppId, nameof(this.GetAllianceGuildsAsync), guildId).ConfigureAwait(false)
                   ?? ImmutableList<AllianceGuildEntry>.Empty;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when getting alliance guilds.");
            return ImmutableList<AllianceGuildEntry>.Empty;
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> IsAllianceMasterAsync(uint guildId)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<uint, bool>(this._targetAppId, nameof(this.IsAllianceMasterAsync), guildId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when checking alliance master.");
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask<uint> GetAllianceMasterIdAsync(uint guildId)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<uint, uint>(this._targetAppId, nameof(this.GetAllianceMasterIdAsync), guildId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when getting alliance master id.");
            return 0;
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> SetHostilityAsync(uint guildId, uint targetGuildId, bool create)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<(uint, uint, bool), bool>(this._targetAppId, nameof(this.SetHostilityAsync), (guildId, targetGuildId, create)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when setting hostility.");
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask<GuildRelationship> GetGuildRelationshipAsync(uint guild1, uint guild2)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<(uint, uint), GuildRelationship>(this._targetAppId, nameof(this.GetGuildRelationshipAsync), (guild1, guild2)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when getting guild relationship.");
            return GuildRelationship.None;
        }
    }
}