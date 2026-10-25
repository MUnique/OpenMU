// <copyright file="IGameServerAdministration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Threading;

/// <summary>Request/response administration of live state owned by one game server.</summary>
/// <remarks>Internal server channel. Public front ends must authorize and audit writes before calling it.</remarks>
public interface IGameServerAdministration : IGameServerPluginOperations
{
    /// <summary>Reads one best-effort scalar snapshot without waiting for player persistence.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The connected characters, bounded by the server's player population.</returns>
    ValueTask<IReadOnlyList<LivePlayerInfo>> GetPlayerSnapshotAsync(CancellationToken cancellationToken);

    /// <summary>Reads live characters without credentials or IP addresses.</summary>
    /// <param name="mapNumber">An optional map filter.</param>
    /// <param name="guildId">An optional live guild filter.</param>
    /// <param name="offset">The number of matches to skip.</param>
    /// <param name="limit">The maximum number of matches, at most 100.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The bounded page.</returns>
    ValueTask<LivePlayerPage> GetPlayersAsync(ushort? mapNumber, uint? guildId, int offset, int limit, CancellationToken cancellationToken);

    /// <summary>Reads one exact live character.</summary>
    /// <param name="characterName">The character name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The live character, or null if it is unavailable.</returns>
    ValueTask<LivePlayerInfo?> GetPlayerAsync(string characterName, CancellationToken cancellationToken);

    /// <summary>Teleports a live character to a walkable point on a hosted map.</summary>
    /// <param name="characterName">The exact character name.</param>
    /// <param name="characterId">The expected character identifier.</param>
    /// <param name="mapNumber">The target map.</param>
    /// <param name="x">The horizontal coordinate.</param>
    /// <param name="y">The vertical coordinate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The confirmed action result.</returns>
    ValueTask<GameServerAdminResult> TeleportAsync(string characterName, Guid characterId, ushort mapNumber, byte x, byte y, CancellationToken cancellationToken);

    /// <summary>Gives one item through the live inventory and player persistence context.</summary>
    /// <param name="characterName">The exact character name.</param>
    /// <param name="characterId">The expected character identifier.</param>
    /// <param name="item">The structured item specification.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The confirmed action result.</returns>
    ValueTask<GameServerAdminResult> GiveItemAsync(string characterName, Guid characterId, ItemSpecification item, CancellationToken cancellationToken);

    /// <summary>Sets a live character's level within the configured limits.</summary>
    /// <param name="characterName">The exact character name.</param>
    /// <param name="characterId">The expected character identifier.</param>
    /// <param name="level">The requested level.</param>
    /// <param name="master">Whether to change master level instead of regular level.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The confirmed action result.</returns>
    ValueTask<GameServerAdminResult> SetLevelAsync(string characterName, Guid characterId, int level, bool master, CancellationToken cancellationToken);

    /// <summary>Reads the configured periodic mini game schedules.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The bounded event schedule.</returns>
    ValueTask<IReadOnlyList<LiveEventInfo>> GetEventsAsync(CancellationToken cancellationToken);

    /// <summary>Force-starts or restarts an enabled periodic mini game.</summary>
    /// <param name="eventType">The invariant mini game type.</param>
    /// <param name="restart">Whether an already running event may be removed first.</param>
    /// <param name="expectedEvent">The event state approved in the preview.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The confirmed start request result; the periodic task starts the event on its next tick.</returns>
    ValueTask<GameServerAdminResult> StartEventAsync(string eventType, bool restart, LiveEventInfo expectedEvent, CancellationToken cancellationToken);
}
