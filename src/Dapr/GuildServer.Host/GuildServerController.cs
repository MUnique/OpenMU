// <copyright file="GuildServerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GuildServer.Host;

using System.Collections.Immutable;
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
    [HttpPost(nameof(IGuildServer.GuildExistsAsync))]
    public ValueTask<bool> GuildExists([FromBody] string guildName)
    {
        return this._guildServer.GuildExistsAsync(guildName);
    }

    /// <summary>
    /// Gets the guild by the guild identifier.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <returns>The guild.</returns>
    [HttpPost(nameof(IGuildServer.GetGuildAsync))]
    public ValueTask<Guild?> GetGuild([FromBody] uint guildId)
    {
        return this._guildServer.GetGuildAsync(guildId);
    }

    /// <summary>
    /// Gets the guild id by the guild name.
    /// </summary>
    /// <param name="guildName">The guild name.</param>
    /// <returns>The guild id. <c>0</c>, if not found.</returns>
    [HttpPost(nameof(IGuildServer.GetGuildIdByNameAsync))]
    public ValueTask<uint> GetGuildIdByName([FromBody] string guildName)
    {
        return this._guildServer.GetGuildIdByNameAsync(guildName);
    }

    /// <summary>
    /// Creates the guild and sets the guild master online at the guild server. A separate call to <see cref="PlayerEnteredGame" /> is not required.
    /// </summary>
    /// <param name="data">The guild creation arguments.</param>
    [HttpPost(nameof(IGuildServer.CreateGuildAsync))]
    public ValueTask<bool> CreateGuild([FromBody] GuildCreationArguments data)
    {
        return this._guildServer.CreateGuildAsync(data.Name, data.MasterName, data.MasterId, data.Logo, data.ServerId);
    }

    /// <summary>
    /// Creates the guild member and sets it online at the guild server. A separate call to <see cref="PlayerEnteredGame" /> is not required.
    /// </summary>
    /// <param name="data">The guild member creation arguments.</param>
    [HttpPost(nameof(IGuildServer.CreateGuildMemberAsync))]
    public ValueTask CreateGuildMember([FromBody] GuildMemberCreationArguments data)
    {
        return this._guildServer.CreateGuildMemberAsync(data.GuildId, data.CharacterId, data.CharacterName, data.Role, data.ServerId);
    }

    /// <summary>
    /// Updates the guild member position.
    /// </summary>
    /// <param name="data">The change arguments.</param>
    [HttpPost(nameof(IGuildServer.ChangeGuildMemberPositionAsync))]
    public ValueTask ChangeGuildMemberPosition([FromBody] GuildMemberRoleChangeArguments data)
    {
        return this._guildServer.ChangeGuildMemberPositionAsync(data.GuildId, data.CharacterId, data.NewRole);
    }

    /// <summary>
    /// Notifies the guild server that a player (potential guild member) entered the game.
    /// </summary>
    /// <param name="data">The arguments of the changed player.</param>
    [Topic("pubsub", nameof(IEventPublisher.PlayerEnteredGameAsync))]
    [HttpPost(nameof(IEventPublisher.PlayerEnteredGameAsync))]
    public ValueTask PlayerEnteredGame([FromBody] PlayerOnlineStateArguments data)
    {
        return this._guildServer.PlayerEnteredGameAsync(data.CharacterId, data.CharacterName, data.ServerId);
    }

    /// <summary>
    /// Notifies the guild server that a guild member left the game.
    /// </summary>
    /// <param name="data">The arguments of the changed player.</param>
    [Topic("pubsub", nameof(IEventPublisher.PlayerLeftGameAsync))]
    [HttpPost(nameof(IEventPublisher.PlayerLeftGameAsync))]
    public ValueTask PlayerLeftGame([FromBody] PlayerOnlineStateArguments data)
    {
        return this._guildServer.GuildMemberLeftGameAsync(data.GuildId, data.CharacterId, data.ServerId);
    }

    /// <summary>
    /// Gets the guild member list.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <returns>The guild member list.</returns>
    [HttpPost(nameof(IGuildServer.GetGuildListAsync))]
    public ValueTask<IImmutableList<GuildListEntry>> GetGuildList([FromBody] uint guildId)
    {
        return this._guildServer.GetGuildListAsync(guildId);
    }

    /// <summary>
    /// Kicks a guild member from a guild.
    /// </summary>
    /// <param name="data">The guild member arguments.</param>
    [HttpPost(nameof(IGuildServer.KickMemberAsync))]
    public ValueTask KickMember([FromBody] GuildMemberArguments data)
    {
        return this._guildServer.KickMemberAsync(data.GuildId, data.PlayerName);
    }

    /// <summary>
    /// Gets the guild position of a specific character.
    /// </summary>
    /// <param name="characterId">The character identifier.</param>
    /// <returns>The guild position.</returns>
    [HttpPost(nameof(IGuildServer.GetGuildPositionAsync))]
    public ValueTask<GuildPosition> GetGuildPosition([FromBody] Guid characterId)
    {
        return this._guildServer.GetGuildPositionAsync(characterId);
    }

    /// <summary>
    /// Increases the guild score by one.
    /// </summary>
    /// <param name="guildId">The identifier of the guild.</param>
    [HttpPost(nameof(IGuildServer.IncreaseGuildScoreAsync))]
    public ValueTask IncreaseGuildScore([FromBody] uint guildId)
    {
        return this._guildServer.IncreaseGuildScoreAsync(guildId);
    }
}