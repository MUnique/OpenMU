// <copyright file="SoccerSpawnGatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.GuildWar;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin which keeps a player on the soccer ground of a running guild war soccer match,
/// instead of spawning it at the safezone.
/// </summary>
[PlugIn]
[Display(Name = nameof(SoccerSpawnGatePlugIn), Description = "Spawns a player of a running soccer match on the soccer ground.")]
[Guid("8C4E1F0B-7A93-4D65-B0C8-2E5F9A1D6B37")]
public class SoccerSpawnGatePlugIn : IPlayerSpawnGateSelectionPlugIn
{
    /// <inheritdoc />
    public ValueTask SelectSpawnGateAsync(Player player, SpawnGateSelectionArgs args)
    {
        if (args.Gate is not null)
        {
            return ValueTask.CompletedTask;
        }

        if (player.GuildWarContext?.WarType == GuildWarType.Soccer
            && player.GuildWarContext.State == GuildWarState.Started
            && player.CurrentMap is SoccerGameMap soccerGameMap
            && soccerGameMap.Definition.BattleZone?.Ground is { } ground)
        {
            args.Gate = new ExitGate
            {
                Map = soccerGameMap.Definition,
                X1 = ground.X1,
                X2 = ground.X2,
                Y1 = ground.Y1,
                Y2 = ground.Y2,
            };
        }

        return ValueTask.CompletedTask;
    }
}
