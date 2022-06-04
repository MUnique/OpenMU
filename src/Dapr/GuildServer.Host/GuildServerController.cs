using Dapr;

namespace MUnique.OpenMU.GuildServer.Host;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

[ApiController]
[Route("")]
public class GuildServerController : ControllerBase
{
    private readonly IGuildServer _guildServer;

    private readonly ILogger<GuildServerController> _logger;

    public GuildServerController(IGuildServer guildServer, ILogger<GuildServerController> logger)
    {
        this._guildServer = guildServer;
        this._logger = logger;
    }

    [HttpPost(nameof(IGuildServer.GuildExists))]
    public bool GuildExists([FromBody] string guildName)
    {
        return this._guildServer.GuildExists(guildName);
    }

    [HttpPost(nameof(IGuildServer.GetGuild))]
    public Guild? GetGuild([FromBody] uint guildId)
    {
        return this._guildServer.GetGuild(guildId);
    }

    [HttpPost(nameof(IGuildServer.GetGuildIdByName))]
    public uint GetGuildIdByName([FromBody] string guildName)
    {
        return this._guildServer.GetGuildIdByName(guildName);
    }

    [HttpPost(nameof(IGuildServer.CreateGuild))]
    public void CreateGuild([FromBody] GuildCreationArguments data)
    {
        this._guildServer.CreateGuild(data.Name, data.MasterName, data.MasterId, data.Logo, data.ServerId);
    }

    [HttpPost(nameof(IGuildServer.CreateGuildMember))]
    public void CreateGuildMember([FromBody] GuildMemberCreationArguments data)
    {
        this._guildServer.CreateGuildMember(data.GuildId, data.CharacterId, data.CharacterName, data.Role, data.ServerId);
    }

    [HttpPost(nameof(IGuildServer.ChangeGuildMemberPosition))]
    public void ChangeGuildMemberPosition([FromBody] GuildMemberRoleChangeArguments data)
    {
        this._guildServer.ChangeGuildMemberPosition(data.GuildId, data.CharacterId, data.NewRole);
    }

    [Topic("pubsub", nameof(IEventPublisher.PlayerEnteredGame))]
    [HttpPost(nameof(IEventPublisher.PlayerEnteredGame))]
    public void PlayerEnteredGame([FromBody] PlayerOnlineStateArguments data)
    {
        this._guildServer.PlayerEnteredGame(data.CharacterId, data.CharacterName, data.ServerId);
    }

    [Topic("pubsub", nameof(IEventPublisher.PlayerLeftGame))]
    [HttpPost(nameof(IEventPublisher.PlayerLeftGame))]
    public void PlayerLeftGame([FromBody] PlayerOnlineStateArguments data)
    {
        this._guildServer.GuildMemberLeftGame(data.GuildId, data.CharacterId, data.ServerId);
    }

    [HttpPost(nameof(IGuildServer.GetGuildList))]
    public IEnumerable<GuildListEntry> GetGuildList([FromBody] uint guildId)
    {
        return this._guildServer.GetGuildList(guildId);
    }

    [HttpPost(nameof(IGuildServer.KickMember))]
    public void KickMember([FromBody] GuildMemberArguments data)
    {
        this._guildServer.KickMember(data.GuildId, data.PlayerName);
    }

    [HttpPost(nameof(IGuildServer.GetGuildPosition))]
    public GuildPosition GetGuildPosition([FromBody] Guid characterId)
    {
        return this._guildServer.GetGuildPosition(characterId);
    }

    [HttpPost(nameof(IGuildServer.IncreaseGuildScore))]
    public void IncreaseGuildScore([FromBody] uint guildId)
    {
        this._guildServer.IncreaseGuildScore(guildId);
    }
}