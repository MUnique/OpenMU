// <copyright file="GuildServerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GuildServer.Host;

using global::Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// Controller which handles external requests coming from other dapr applications,
/// most probably <see cref="MUnique.OpenMU.ServerClients.GuildServer"/>.
/// </summary>
[ApiController]
[Route("")]
public class GuildServerController : ControllerBase
{
    private readonly IGuildServer _guildServer;

    private readonly ILogger<GuildServerController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildServerController"/> class.
    /// </summary>
    /// <param name="guildServer">The guild server.</param>
    /// <param name="logger">The logger.</param>
    public GuildServerController(IGuildServer guildServer, ILogger<GuildServerController> logger)
    {
        this._guildServer = guildServer;
        this._logger = logger;
    }

    /// <summary>
    /// Checks if the guild with the specified name exists.
    /// </summary>
    /// <param name="guildName">Name of the guild.</param>
    /// <returns>True, if the guild exists; False, otherwise.</returns>
    [HttpPost(nameof(IGuildServer.GuildExists))]
    public bool GuildExists([FromBody] string guildName)
    {
        return this._guildServer.GuildExists(guildName);
    }

    /// <summary>
    /// Gets the guild by the guild identifier.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <returns>The guild.</returns>
    [HttpPost(nameof(IGuildServer.GetGuild))]
    public Guild? GetGuild([FromBody] uint guildId)
    {
        return this._guildServer.GetGuild(guildId);
    }

    /// <summary>
    /// Gets the guild id by the guild name.
    /// </summary>
    /// <param name="guildName">The guild name.</param>
    /// <returns>The guild id. <c>0</c>, if not found.</returns>
    [HttpPost(nameof(IGuildServer.GetGuildIdByName))]
    public uint GetGuildIdByName([FromBody] string guildName)
    {
        return this._guildServer.GetGuildIdByName(guildName);
    }

    /// <summary>
    /// Creates the guild and sets the guild master online at the guild server. A separate call to <see cref="PlayerEnteredGame" /> is not required.
    /// </summary>
    /// <param name="data">The guild creation arguments.</param>
    [HttpPost(nameof(IGuildServer.CreateGuild))]
    public void CreateGuild([FromBody] GuildCreationArguments data)
    {
        this._guildServer.CreateGuild(data.Name, data.MasterName, data.MasterId, data.Logo, data.ServerId);
    }

    /// <summary>
    /// Creates the guild member and sets it online at the guild server. A separate call to <see cref="PlayerEnteredGame" /> is not required.
    /// </summary>
    /// <param name="data">The guild member creation arguments.</param>
    [HttpPost(nameof(IGuildServer.CreateGuildMember))]
    public void CreateGuildMember([FromBody] GuildMemberCreationArguments data)
    {
        this._guildServer.CreateGuildMember(data.GuildId, data.CharacterId, data.CharacterName, data.Role, data.ServerId);
    }

    /// <summary>
    /// Updates the guild member position.
    /// </summary>
    /// <param name="data">The change arguments.</param>
    [HttpPost(nameof(IGuildServer.ChangeGuildMemberPosition))]
    public void ChangeGuildMemberPosition([FromBody] GuildMemberRoleChangeArguments data)
    {
        this._guildServer.ChangeGuildMemberPosition(data.GuildId, data.CharacterId, data.NewRole);
    }

    /// <summary>
    /// Notifies the guild server that a player (potential guild member) entered the game.
    /// </summary>
    /// <param name="data">The arguments of the changed player.</param>
    [Topic("pubsub", nameof(IEventPublisher.PlayerEnteredGame))]
    [HttpPost(nameof(IEventPublisher.PlayerEnteredGame))]
    public void PlayerEnteredGame([FromBody] PlayerOnlineStateArguments data)
    {
        this._guildServer.PlayerEnteredGame(data.CharacterId, data.CharacterName, data.ServerId);
    }

    /// <summary>
    /// Notifies the guild server that a guild member left the game.
    /// </summary>
    /// <param name="data">The arguments of the changed player.</param>
    [Topic("pubsub", nameof(IEventPublisher.PlayerLeftGame))]
    [HttpPost(nameof(IEventPublisher.PlayerLeftGame))]
    public void PlayerLeftGame([FromBody] PlayerOnlineStateArguments data)
    {
        this._guildServer.GuildMemberLeftGame(data.GuildId, data.CharacterId, data.ServerId);
    }

    /// <summary>
    /// Gets the guild member list.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <returns>The guild member list.</returns>
    [HttpPost(nameof(IGuildServer.GetGuildList))]
    public IEnumerable<GuildListEntry> GetGuildList([FromBody] uint guildId)
    {
        return this._guildServer.GetGuildList(guildId);
    }

    /// <summary>
    /// Kicks a guild member from a guild.
    /// </summary>
    /// <param name="data">The guild member arguments.</param>
    [HttpPost(nameof(IGuildServer.KickMember))]
    public void KickMember([FromBody] GuildMemberArguments data)
    {
        this._guildServer.KickMember(data.GuildId, data.PlayerName);
    }

    /// <summary>
    /// Gets the guild position of a specific character.
    /// </summary>
    /// <param name="characterId">The character identifier.</param>
    /// <returns>The guild position.</returns>
    [HttpPost(nameof(IGuildServer.GetGuildPosition))]
    public GuildPosition GetGuildPosition([FromBody] Guid characterId)
    {
        return this._guildServer.GetGuildPosition(characterId);
    }

    /// <summary>
    /// Increases the guild score by one.
    /// </summary>
    /// <param name="guildId">The identifier of the guild.</param>
    [HttpPost(nameof(IGuildServer.IncreaseGuildScore))]
    public void IncreaseGuildScore([FromBody] uint guildId)
    {
        this._guildServer.IncreaseGuildScore(guildId);
    }
}