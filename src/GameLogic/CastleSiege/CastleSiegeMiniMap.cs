// <copyright file="CastleSiegeMiniMap.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Gathers and pushes Castle Siege mini-map positions to online alliance masters.
/// </summary>
/// <remarks>
/// <see cref="BroadcastAsync"/> is only ever called from <c>CastleSiegePlugIn.OnTickAsync</c>, which already
/// holds <see cref="CastleSiegeContext.ExecutionLock"/> for the whole tick. It must never acquire that lock
/// itself (it is a non-reentrant <see cref="System.Threading.SemaphoreSlim"/>).
/// </remarks>
public static class CastleSiegeMiniMap
{
    /// <summary>
    /// The maximum number of player positions per update specified by Castle Siege issue #730.
    /// This limits the position payload independently of the client's growable buffer.
    /// </summary>
    private const int MaximumPlayersPerGuild = 1000;

    /// <summary>
    /// Pushes current player and NPC positions to every online alliance master of a participating guild.
    /// There is no client request packet for this today, so every eligible alliance master is treated as an
    /// implicit subscriber rather than gating on a per-player request.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns>A task that represents the asynchronous broadcast operation.</returns>
    public static async ValueTask BroadcastAsync(CastleSiegeContext context)
    {
        if (!context.Configuration.Enabled || context.CurrentState != CastleSiegeState.Start)
        {
            return;
        }

        // Map changes are not protected by ExecutionLock. Resolve each player's side and position once so
        // a departure cannot change a dictionary key between grouping the snapshot and sending it.
        var players = context.GetSiegePlayers()
            .Select(player => (Player: player, Side: context.GetPlayerJoinSide(player), Position: player.Position))
            .Where(entry => entry.Side != CastleSiegeJoinSide.None)
            .ToList();
        var recipients = players
            .Where(entry => CastleSiegeGuildResolver.ResolveParticipatingAllianceMaster(entry.Player, context) is not null)
            .ToList();
        if (recipients.Count == 0)
        {
            return;
        }

        var npcs = GatherNpcPositions(context);
        var playersBySide = players
            .GroupBy(entry => entry.Side)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<CastleSiegeMiniMapPlayerInfo>)group
                    .Take(MaximumPlayersPerGuild)
                    .Select(entry => new CastleSiegeMiniMapPlayerInfo(entry.Position.X, entry.Position.Y))
                    .ToList());

        await Task.WhenAll(recipients.Select(entry =>
                SendAsync(entry.Player, playersBySide[entry.Side], npcs).AsTask()))
            .ConfigureAwait(false);
    }

    private static async ValueTask SendAsync(
        Player player,
        IReadOnlyList<CastleSiegeMiniMapPlayerInfo> players,
        IReadOnlyList<CastleSiegeMiniMapNpcInfo> npcs)
    {
        // Order matters: the client clears its whole position buffer when it receives the player packet, and
        // only appends on the NPC packet. The player packet must go first, and must still be sent when the
        // list is empty, or a future refactor could short-circuit it and leave stale entries on the client.
        await player.InvokeViewPlugInAsync<ICastleSiegeMiniMapPlugIn>(
                view => view.ShowPlayerPositionsAsync(players))
            .ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICastleSiegeMiniMapPlugIn>(
                view => view.ShowNpcPositionsAsync(npcs))
            .ConfigureAwait(false);
    }

    private static List<CastleSiegeMiniMapNpcInfo> GatherNpcPositions(CastleSiegeContext context)
    {
        var npcs = new List<CastleSiegeMiniMapNpcInfo>();
        AddAlive(context.NpcController.GetDefenseStructures(CastleSiegeGate.MonsterNumber), true, npcs);
        AddAlive(context.NpcController.GetDefenseStructures(CastleSiegeStatue.MonsterNumber), false, npcs);
        return npcs;
    }

    private static void AddAlive(IEnumerable<CastleSiegeNpcRuntime> runtimes, bool isGate, List<CastleSiegeMiniMapNpcInfo> target)
    {
        foreach (var runtime in runtimes.Where(runtime => runtime.IsAlive))
        {
            target.Add(new CastleSiegeMiniMapNpcInfo(isGate, runtime.Definition.SpawnX, runtime.Definition.SpawnY));
        }
    }
}
