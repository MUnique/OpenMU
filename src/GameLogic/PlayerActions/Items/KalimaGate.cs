// <copyright file="KalimaGate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;
using MonsterSpawnArea = MUnique.OpenMU.Persistence.BasicModel.MonsterSpawnArea;

/// <summary>
/// Opens a gate to Kalima, as the lost map and the kalima ticket do.
/// </summary>
internal static class KalimaGate
{
    /// <summary>
    /// The highest level of Kalima.
    /// </summary>
    internal const int MaximumLevel = 7;

    /// <summary>
    /// The number of the gate NPC of the first level of Kalima. The gate NPCs of the following levels have the following numbers.
    /// </summary>
    private const byte GateNpcStartNumber = 152;

    /// <summary>
    /// The numbers of the Kalima maps, by their level.
    /// </summary>
    private static readonly int[] KalimaMapNumbers = [24, 25, 26, 27, 28, 29, 36];

    /// <summary>
    /// Determines whether the specified map is a Kalima map.
    /// </summary>
    /// <param name="mapNumber">The map number.</param>
    /// <returns><c>true</c>, if the map is a Kalima map.</returns>
    public static bool IsKalimaMap(int mapNumber) => KalimaMapNumbers.Contains(mapNumber);

    /// <summary>
    /// Opens a gate to the specified level of Kalima at the target coordinates, which the player and its party can pass.
    /// The player gets a message, when it's not possible.
    /// </summary>
    /// <param name="player">The player who opens the gate.</param>
    /// <param name="kalimaLevel">The level of Kalima, from 1 to <see cref="MaximumLevel"/>.</param>
    /// <param name="target">The coordinates of the gate.</param>
    /// <returns><c>true</c>, if the gate was opened.</returns>
    public static async ValueTask<bool> TryOpenAsync(Player player, int kalimaLevel, Point target)
    {
        if (player.CurrentMap is not { } currentMap)
        {
            return false;
        }

        if (player.CurrentMiniGame is not null)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.NoKalimaGateOnEventMap)).ConfigureAwait(false);
            return false;
        }

        if (player.IsAtSafezone() || currentMap.Terrain.SafezoneMap[target.X, target.Y])
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.NoKalimaGateInSafezone)).ConfigureAwait(false);
            return false;
        }

        var gateNpcNumber = GateNpcStartNumber + kalimaLevel - 1;
        var gateNpcDef = player.GameContext.Configuration.Monsters.FirstOrDefault(def => def.Number == gateNpcNumber);
        if (gateNpcDef is null)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.UndefinedGateNpc)).ConfigureAwait(false);
            return false;
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

        var targetGate = player.GameContext.Configuration.Maps.FirstOrDefault(g => g.Number == KalimaMapNumbers[kalimaLevel - 1])?.ExitGates.FirstOrDefault();
        if (targetGate is null)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.KalimaEntranceNotFound)).ConfigureAwait(false);
            return false;
        }

        var gate = new GateNpc(spawnArea, gateNpcDef, currentMap, player, targetGate, TimeSpan.FromMinutes(1));
        gate.Initialize();
        await currentMap.AddAsync(gate).ConfigureAwait(false);
        return true;
    }
}
