// <copyright file="GuildServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

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
    public bool GuildExists(string guildName)
    {
        try
        {
            return this._daprClient.InvokeMethodAsync<string, bool>(this._targetAppId, nameof(this.GuildExists), guildName).Result;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when checking a guild existence.");
            throw;
        }
    }

    /// <inheritdoc />
    public Guild? GetGuild(uint guildId)
    {
        try
        {
            return this._daprClient.InvokeMethodAsync<uint, Guild?>(this._targetAppId, nameof(this.GetGuild), guildId).Result;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild retrieval message.");
            return null;
        }
    }

    /// <inheritdoc />
    public uint GetGuildIdByName(string guildName)
    {
        try
        {
            return this._daprClient.InvokeMethodAsync<string, uint>(this._targetAppId, nameof(this.GetGuildIdByName), guildName).Result;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when getting the id by guild name.");
            return 0;
        }
    }

    /// <inheritdoc />
    public bool CreateGuild(string name, string masterName, Guid masterId, byte[] logo, byte serverId)
    {
        try
        {
            return this._daprClient.InvokeMethodAsync<GuildCreationArguments, bool>(this._targetAppId, nameof(this.CreateGuild), new GuildCreationArguments(name, masterName, masterId, logo, serverId)).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild creation message.");
            return false;
        }
    }

    /// <inheritdoc />
    public void CreateGuildMember(uint guildId, Guid characterId, string characterName, GuildPosition role, byte serverId)
    {
        try
        {
            this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(this.CreateGuildMember), new GuildMemberCreationArguments(guildId, characterId, characterName, role, serverId));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild member creation.");
        }
    }

    /// <inheritdoc />
    public void ChangeGuildMemberPosition(uint guildId, Guid characterId, GuildPosition role)
    {
        try
        {
            this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(this.ChangeGuildMemberPosition), new GuildMemberRoleChangeArguments(guildId, characterId, role));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild member position change.");
        }
    }

    /// <inheritdoc />
    public void PlayerEnteredGame(Guid characterId, string characterName, byte serverId)
    {
        // Handled by EventPublisher, through pub/sub component.
        /*
        try
        {
            this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(this.PlayerEnteredGame), new PlayerEnteredGameArguments(characterId, characterName, serverId));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a PlayerEnteredGame.");
        }*/
    }

    /// <inheritdoc />
    public void GuildMemberLeftGame(uint guildId, Guid guildMemberId, byte serverId)
    {
        // Handled by EventPublisher, through pub/sub component.
        /*
        try
        {
            this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(this.GuildMemberLeftGame), new GuildPlayerLeftGameArguments(guildId, guildMemberId, serverId));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a GuildMemberLeftGame.");
        }*/
    }

    /// <inheritdoc />
    public IEnumerable<GuildListEntry> GetGuildList(uint guildId)
    {
        try
        {
            return this._daprClient.InvokeMethodAsync<uint, IEnumerable<GuildListEntry>>(this._targetAppId, nameof(this.GetGuildList), guildId).Result;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when getting a guild list.");
            return Enumerable.Empty<GuildListEntry>();
        }
    }

    /// <inheritdoc />
    public void KickMember(uint guildId, string playerName)
    {
        try
        {
            this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(this.KickMember), new GuildMemberArguments(guildId, playerName));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when kicking a guild member.");
        }
    }

    /// <inheritdoc />
    public GuildPosition GetGuildPosition(Guid characterId)
    {
        try
        {
            return this._daprClient.InvokeMethodAsync<Guid, GuildPosition>(this._targetAppId, nameof(this.GetGuildPosition), characterId).Result;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when retrieving a guild position.");
            return GuildPosition.Undefined;
        }
    }

    /// <inheritdoc />
    public void IncreaseGuildScore(uint guildId)
    {
        try
        {
            this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(this.IncreaseGuildScore), guildId);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild score increase.");
        }
    }
}