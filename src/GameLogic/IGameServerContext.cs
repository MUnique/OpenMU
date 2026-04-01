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
    event EventHandler<GuildEventArgs>? GuildDeleted;

    /// <summary>
    /// Occurs when a guild alliance has been changed.
    /// </summary>
    event EventHandler<GuildEventArgs>? GuildChanged;

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
    /// Refreshes the guild information.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    ValueTask RefreshGuildInfoAsync(uint guildId);

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

    /// <summary>
    /// Updates the cached rival guild pairs when the hostility state between two guilds changes.
    /// </summary>
    /// <param name="guildIdA">The first guild identifier.</param>
    /// <param name="allianceGuildIdsA">All guild IDs in guild A's alliance.</param>
    /// <param name="guildIdB">The second guild identifier.</param>
    /// <param name="allianceGuildIdsB">All guild IDs in guild B's alliance.</param>
    /// <param name="created"><c>true</c> if the hostility was created; <c>false</c> if it was removed.</param>
    void UpdateGuildHostility(uint guildIdA, IReadOnlyList<uint> allianceGuildIdsA, uint guildIdB, IReadOnlyList<uint> allianceGuildIdsB, bool created);

    /// <summary>
    /// Determines whether two guilds are rivals (hostile to each other).
    /// This uses a local cache and does not call the guild server.
    /// </summary>
    /// <param name="guild1Id">The first guild identifier.</param>
    /// <param name="guild2Id">The second guild identifier.</param>
    /// <returns><c>true</c> if the guilds are rivals; <c>false</c> otherwise.</returns>
    bool AreGuildsRival(uint guild1Id, uint guild2Id);
}