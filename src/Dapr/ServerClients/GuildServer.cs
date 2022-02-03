// <copyright file="GuildServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using Dapr.Client;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

public class GuildServer : IGuildServer
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<GuildServer> _logger;
    private readonly string _targetAppId;

    public GuildServer(DaprClient daprClient, ILogger<GuildServer> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
        this._targetAppId = "guildServer";
    }

    public void GuildMessage(uint guildId, string sender, string message)
    {
        try
        {
            this._daprClient.PublishEventAsync("pubsub", nameof(GuildMessage), new GuildMessageArguments(guildId, sender, message));
            // this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(GuildMessage), new GuildMessageArguments(guildId, sender, message));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild message.");
        }
    }

    public void AllianceMessage(uint guildId, string sender, string message)
    {
        try
        {
            this._daprClient.PublishEventAsync("pubsub", nameof(AllianceMessage), new GuildMessageArguments(guildId, sender, message));
            // this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(AllianceMessage), new GuildMessageArguments(guildId, sender, message));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending an alliance guild message.");
        }
    }

    public bool GuildExists(string guildName)
    {
        try
        {
            return this._daprClient.InvokeMethodAsync<string, bool>(this._targetAppId, nameof(GuildExists), guildName).Result;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when checking a guild existence.");
            throw;
        }
    }

    public Guild? GetGuild(uint guildId)
    {
        try
        {
            return this._daprClient.InvokeMethodAsync<uint, Guild?>(this._targetAppId, nameof(GetGuild), guildId).Result;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild retrieval message.");
            return null;
        }
    }

    public uint GetGuildIdByName(string guildName)
    {
        try
        {
            return this._daprClient.InvokeMethodAsync<string, uint>(this._targetAppId, nameof(GetGuildIdByName), guildName).Result;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when getting the id by guild name.");
            return 0;
        }
    }

    public bool CreateGuild(string name, string masterName, Guid masterId, byte[] logo, byte serverId)
    {
        try
        {
            return this._daprClient.InvokeMethodAsync<GuildCreationArguments, bool>(this._targetAppId, nameof(CreateGuild), new GuildCreationArguments(name, masterName, masterId, logo, serverId)).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild creation message.");
            return false;
        }
    }

    public void CreateGuildMember(uint guildId, Guid characterId, string characterName, GuildPosition role, byte serverId)
    {
        try
        {
            this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(CreateGuildMember), new GuildMemberCreationArguments(guildId, characterId, characterName, role, serverId));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild member creation.");
        }
    }

    public void ChangeGuildMemberPosition(uint guildId, Guid characterId, GuildPosition role)
    {
        try
        {
            this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(ChangeGuildMemberPosition), new GuildMemberRoleChangeArguments(guildId, characterId, role));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild member position change.");
        }
    }

    public void PlayerEnteredGame(Guid characterId, string characterName, byte serverId)
    {
        // it's usually never called in this implementation. todo: check if it can be removed
        try
        {
            this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(PlayerEnteredGame), new PlayerEnteredGameArguments(characterId, characterName, serverId));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a PlayerEnteredGame.");
        }
    }

    public void GuildMemberLeftGame(uint guildId, Guid guildMemberId, byte serverId)
    {
        // it's usually never called in this implementation. todo: check if it can be removed
        try
        {
            this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(GuildMemberLeftGame), new GuildPlayerLeftGameArguments(guildId, guildMemberId, serverId));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a GuildMemberLeftGame.");
        }
    }

    public IEnumerable<GuildListEntry> GetGuildList(uint guildId)
    {
        try
        {
            return this._daprClient.InvokeMethodAsync<uint, IEnumerable<GuildListEntry>>(this._targetAppId, nameof(GetGuildList), guildId).Result;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when getting a guild list.");
            return Enumerable.Empty<GuildListEntry>();
        }
    }

    public void KickMember(uint guildId, string playerName)
    {
        try
        {
            this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(KickMember), new GuildMemberArguments(guildId, playerName));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when kicking a guild member.");
        }
    }

    public GuildPosition GetGuildPosition(Guid characterId)
    {
        try
        {
            return this._daprClient.InvokeMethodAsync<Guid, GuildPosition>(this._targetAppId, nameof(GetGuildPosition), characterId).Result;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when retrieving a guild position.");
            return GuildPosition.Undefined;
        }
    }

    public void IncreaseGuildScore(uint guildId)
    {
        try
        {
            this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(IncreaseGuildScore), guildId);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a guild score increase.");
        }
    }
}