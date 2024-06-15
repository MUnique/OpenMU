// <copyright file="LostMapDroppedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;
using MonsterSpawnArea = MUnique.OpenMU.Persistence.BasicModel.MonsterSpawnArea;

/// <summary>
/// This plugin transforms a stack of symbol of kundun into a lost map.
/// todo: implement plugin configuration to resolve magic numbers.
/// </summary>
[PlugIn(nameof(LostMapDroppedPlugIn), "This plugin handles the drop of the lost map item. It creates the gate to the kalima map.")]
[Guid("F6DB10E0-AE7F-4BC6-914F-B858763C5CF7")]
public sealed class LostMapDroppedPlugIn : IItemDropPlugIn
{
    private static readonly int[] KalimaMapNumbers = [24, 25, 26, 27, 28, 29, 36];

    private const byte GateNpcStartNumber = 152;

    /// <inheritdoc />
    public async ValueTask HandleItemDropAsync(Player player, Item item, Point target, IItemDropPlugIn.ItemDropArguments cancelArgs)
    {
        if (!item.IsLostMap())
        {
            return;
        }

        cancelArgs.WasHandled = true;
        var currentMap = player.CurrentMap;
        if (currentMap is null)
        {
            return;
        }

        if (item.Level is < 1 or > 7)
        {
            await player.ShowMessageAsync("The lost map is not valid.").ConfigureAwait(false);
            return;
        }

        if (player.CurrentMiniGame is not null)
        {
            await player.ShowMessageAsync("Cannot create kalima gate on event map.").ConfigureAwait(false);
            return;
        }

        var gatePosition = target;
        if (player.IsAtSafezone() || player.CurrentMap?.Terrain.SafezoneMap[gatePosition.X, gatePosition.Y] is true)
        {
            await player.ShowMessageAsync("Cannot create kalima gate in safe zone.").ConfigureAwait(false);
            return;
        }

        var gateNpcNumber = GateNpcStartNumber + item.Level - 1;
        var gateNpcDef = player.GameContext.Configuration.Monsters.FirstOrDefault(def => def.Number == gateNpcNumber);
        if (gateNpcDef is null)
        {
            await player.ShowMessageAsync("The gate npc is not defined.").ConfigureAwait(false);
            return;
        }

        var spawnArea = new MonsterSpawnArea
        {
            Direction = Direction.West,
            Quantity = 1,
            MonsterDefinition = gateNpcDef,
            SpawnTrigger = SpawnTrigger.ManuallyForEvent,
            X1 = target.X,
            X2 = target.X,
            Y1 = target.Y,
            Y2 = target.Y,
        };

        var targetGate = player.GameContext.Configuration.Maps.FirstOrDefault(g => g.Number == KalimaMapNumbers[item.Level - 1])?.ExitGates.FirstOrDefault();
        if (targetGate is null)
        {
            await player.ShowMessageAsync("The kalima entrance wasn't found.").ConfigureAwait(false);
            return;
        }

        var gate = new GateNpc(spawnArea, gateNpcDef, currentMap, player, targetGate, TimeSpan.FromMinutes(1));
        gate.Initialize();
        await currentMap.AddAsync(gate).ConfigureAwait(false);
        cancelArgs.Success = true;
    }
}