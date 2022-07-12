// <copyright file="IGameServerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// The context of a game server.
/// </summary>
public interface IGameServerContext : IGameContext
{
    /// <summary>
    /// Occurs when a guild has been deleted.
    /// </summary>
    event EventHandler<GuildDeletedEventArgs>? GuildDeleted;

    /// <summary>
    /// Gets the identifier of the server.
    /// </summary>
    byte Id { get; }

    /// <summary>
    /// Gets the guild server.
    /// </summary>
    IGuildServer GuildServer { get; }

    /// <summary>
    /// Gets the login server.
    /// </summary>
    ILoginServer LoginServer { get; }

    /// <summary>
    /// Gets the friend server.
    /// </summary>
    IFriendServer FriendServer { get; }

    /// <summary>
    /// Gets the message publisher.
    /// </summary>
    IEventPublisher EventPublisher { get; }

    /// <summary>
    /// Gets the server configuration.
    /// </summary>
    GameServerConfiguration ServerConfiguration { get; }

    /// <summary>
    /// Executes an action for each player of the guild.
    /// </summary>
    /// <param name="guildId">The guild id.</param>
    /// <param name="action">The action which should be executed.</param>
    ValueTask ForEachGuildPlayerAsync(uint guildId, Func<Player, Task> action);

    /// <summary>
    /// Executes an action for each player of the alliance of the guild.
    /// </summary>
    /// <param name="guildId">The guild id.</param>
    /// <param name="action">The action which should be executed.</param>
    ValueTask ForEachAlliancePlayerAsync(uint guildId, Func<Player, Task> action);

    /// <summary>
    /// Registers a guild member to the game, e.g. after a player entered a guild.
    /// </summary>
    /// <param name="guildMember">The guild member.</param>
    ValueTask RegisterGuildMemberAsync(Player guildMember);

    /// <summary>
    /// Unregisters a guild member from the game, e.g. after a player left the game or the guild.
    /// </summary>
    /// <param name="guildMember">The guild member.</param>
    ValueTask UnregisterGuildMemberAsync(Player guildMember);

    /// <summary>
    /// Removes a whole guild, usually after it has been disbanded.
    /// </summary>
    /// <param name="guildId">The id of the guild.</param>
    ValueTask RemoveGuildAsync(uint guildId);
}