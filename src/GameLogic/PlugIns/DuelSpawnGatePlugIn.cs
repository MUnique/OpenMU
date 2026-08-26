// <copyright file="DuelSpawnGatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin which spawns a duelist at its side of the duel area, instead of the safezone.
/// </summary>
[PlugIn]
[Display(Name = nameof(DuelSpawnGatePlugIn), Description = "Spawns a player which is in a duel at its side of the duel area.")]
[Guid("1D2C8B5A-4E7F-4C93-8B21-6A0E5D9F3C74")]
public class DuelSpawnGatePlugIn : IPlayerSpawnGateSelectionPlugIn
{
    /// <inheritdoc />
    public ValueTask SelectSpawnGateAsync(Player player, SpawnGateSelectionArgs args)
    {
        if (args.Gate is not null)
        {
            return ValueTask.CompletedTask;
        }

        if (player.DuelRoom is { State: DuelState.DuelAccepted or DuelState.DuelStarted } duelRoom
            && duelRoom.GetSpawnGate(player) is { } duelExitGate)
        {
            args.Gate = duelExitGate;
        }

        return ValueTask.CompletedTask;
    }
}
