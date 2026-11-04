// <copyright file="LostMapDroppedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using System.Runtime.InteropServices;
<<<<<<< HEAD
using MUnique.OpenMU.GameLogic.NPC;
=======
>>>>>>> origin/slave
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;
<<<<<<< HEAD
using MonsterSpawnArea = MUnique.OpenMU.Persistence.BasicModel.MonsterSpawnArea;
=======
>>>>>>> origin/slave

/// <summary>
/// This plugin transforms a stack of symbol of kundun into a lost map.
/// todo: implement plugin configuration to resolve magic numbers.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.LostMapDroppedPlugIn_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.LostMapDroppedPlugIn_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
[Guid("F6DB10E0-AE7F-4BC6-914F-B858763C5CF7")]
public sealed class LostMapDroppedPlugIn : IItemDropPlugIn
{
<<<<<<< HEAD
    private const byte GateNpcStartNumber = 152;

    private static readonly int[] KalimaMapNumbers = [24, 25, 26, 27, 28, 29, 36];

=======
>>>>>>> origin/slave
    /// <inheritdoc />
    public async ValueTask HandleItemDropAsync(Player player, Item item, Point target, IItemDropPlugIn.ItemDropArguments cancelArgs)
    {
        if (!item.IsLostMap())
        {
            return;
        }

        cancelArgs.WasHandled = true;
<<<<<<< HEAD
        var currentMap = player.CurrentMap;
        if (currentMap is null)
=======
        if (player.CurrentMap is null)
>>>>>>> origin/slave
        {
            return;
        }

<<<<<<< HEAD
        if (item.Level is < 1 or > 7)
=======
        if (item.Level is < 1 or > KalimaGate.MaximumLevel)
>>>>>>> origin/slave
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.InvalidLostMap)).ConfigureAwait(false);
            return;
        }

<<<<<<< HEAD
        if (player.CurrentMiniGame is not null)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.NoKalimaGateOnEventMap)).ConfigureAwait(false);
            return;
        }

        var gatePosition = target;
        if (player.IsAtSafezone() || player.CurrentMap?.Terrain.SafezoneMap[gatePosition.X, gatePosition.Y] is true)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.NoKalimaGateInSafezone)).ConfigureAwait(false);
            return;
        }

        var gateNpcNumber = GateNpcStartNumber + item.Level - 1;
        var gateNpcDef = player.GameContext.Configuration.Monsters.FirstOrDefault(def => def.Number == gateNpcNumber);
        if (gateNpcDef is null)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.UndefinedGateNpc)).ConfigureAwait(false);
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
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.KalimaEntranceNotFound)).ConfigureAwait(false);
            return;
        }

        var gate = new GateNpc(spawnArea, gateNpcDef, currentMap, player, targetGate, TimeSpan.FromMinutes(1));
        gate.Initialize();
        await currentMap.AddAsync(gate).ConfigureAwait(false);
        cancelArgs.Success = true;
=======
        cancelArgs.Success = await KalimaGate.TryOpenAsync(player, item.Level, target).ConfigureAwait(false);
>>>>>>> origin/slave
    }
}