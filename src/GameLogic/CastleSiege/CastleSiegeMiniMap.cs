// <copyright file="CastleSiegeMiniMap.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Gathers and broadcasts Castle Siege mini-map positions to requesting alliance masters.
/// </summary>
/// <remarks>
/// <see cref="BroadcastAsync"/> is only ever called from <c>CastleSiegePlugIn.OnTickAsync</c>, which already
/// holds <see cref="CastleSiegeContext.ExecutionLock"/> for the whole tick. It must never acquire that lock
/// itself (it is a non-reentrant <see cref="System.Threading.SemaphoreSlim"/>).
/// </remarks>
public static class CastleSiegeMiniMap
{
    /// <summary>
    /// Sends current player and NPC positions to every alliance master of a participating guild who is
    /// currently on the Castle Siege map.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns>A task that represents the asynchronous broadcast operation.</returns>
    public static async ValueTask BroadcastAsync(CastleSiegeContext context)
    {
        var recipients = context.GetSiegePlayers()
            .Select(player => (Player: player, Participant: CastleSiegeGuildResolver.ResolveParticipatingAllianceMaster(player, context)))
            .Where(entry => entry.Participant is not null)
            .ToList();
        if (recipients.Count == 0)
        {
            return;
        }

        var npcs = GatherNpcPositions(context);
        var playersBySide = recipients
            .Select(entry => entry.Participant!.Side)
            .Distinct()
            .ToDictionary(side => side, side => GatherPlayerPositions(context, side));

        await Task.WhenAll(recipients.Select(entry =>
                SendAsync(entry.Player, playersBySide[entry.Participant!.Side], npcs).AsTask()))
            .ConfigureAwait(false);
    }

    private static async ValueTask SendAsync(
        Player player,
        IReadOnlyList<CastleSiegeMiniMapPlayerInfo> players,
        IReadOnlyList<CastleSiegeMiniMapNpcInfo> npcs)
    {
        await player.InvokeViewPlugInAsync<ICastleSiegeMiniMapPlugIn>(
                view => view.ShowPlayerPositionsAsync(players))
            .ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICastleSiegeMiniMapPlugIn>(
                view => view.ShowNpcPositionsAsync(npcs))
            .ConfigureAwait(false);
    }

    private static List<CastleSiegeMiniMapPlayerInfo> GatherPlayerPositions(CastleSiegeContext context, CastleSiegeJoinSide side)
    {
        return context.GetSiegePlayers()
            .Where(player => context.GetPlayerJoinSide(player) == side)
            .Select(player => new CastleSiegeMiniMapPlayerInfo(player.Position.X, player.Position.Y))
            .ToList();
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
