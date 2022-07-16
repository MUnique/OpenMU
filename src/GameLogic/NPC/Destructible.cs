// <copyright file="Destructible.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The implementation of a destructible, which can attack players.
/// </summary>
public sealed class Destructible : AttackableNpcBase, IAttackable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Destructible" /> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The stats.</param>
    /// <param name="map">The map on which this instance will spawn.</param>
    /// <param name="eventStateProvider">The event state provider.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    /// <param name="plugInManager">The plugin manager.</param>
    public Destructible(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map, IEventStateProvider? eventStateProvider, IDropGenerator dropGenerator, PlugInManager plugInManager)
        : base(spawnInfo, stats, map, eventStateProvider, dropGenerator, plugInManager)
    {
    }

    /// <inheritdoc/>
    public override ValueTask ReflectDamageAsync(IAttacker reflector, uint damage)
    {
        // A destructible doesn't attack, so it doesn't reflect.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask ApplyPoisonDamageAsync(IAttacker initialAttacker, uint damage)
    {
        // A destructible is not an organism which can be poisoned.
        return ValueTask.CompletedTask;
    }
}