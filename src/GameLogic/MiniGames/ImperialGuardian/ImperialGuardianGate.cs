// <copyright file="ImperialGuardianGate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;

using System.Threading;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A gate or statue of the imperial guardian event.
/// It can only be attacked after the event allowed it, and a gate blocks the way until it's destroyed.
/// </summary>
public sealed class ImperialGuardianGate : AttackableNpcBase
{
    private int _isAttackable;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImperialGuardianGate"/> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The stats.</param>
    /// <param name="map">The map on which this instance will spawn.</param>
    /// <param name="eventStateProvider">The event state provider.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    /// <param name="plugInManager">The plugin manager.</param>
    /// <param name="isBlocking">If set to <c>true</c>, the gate blocks the way until it's destroyed.</param>
    public ImperialGuardianGate(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map, IEventStateProvider? eventStateProvider, IDropGenerator dropGenerator, PlugInManager plugInManager, bool isBlocking)
        : base(spawnInfo, stats, map, eventStateProvider, dropGenerator, plugInManager)
    {
        this.IsBlocking = isBlocking;
    }

    /// <summary>
    /// Gets a value indicating whether the gate blocks the way until it's destroyed.
    /// </summary>
    public bool IsBlocking { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the gate can be attacked.
    /// </summary>
    public bool IsAttackable
    {
        get => Volatile.Read(ref this._isAttackable) != 0;
        set => Volatile.Write(ref this._isAttackable, value ? 1 : 0);
    }

    /// <summary>
    /// Gets the area which the gate blocks, which is in front of it, depending on its direction.
    /// </summary>
    /// <returns>The blocked area.</returns>
    public (byte StartX, byte StartY, byte EndX, byte EndY) GetBlockedArea()
    {
        var x = this.Position.X;
        var y = this.Position.Y;
        return (int)this.SpawnArea.Direction switch
        {
            1 => Area(x - 2, y, x + 2, y + 3),
            3 => Area(x - 3, y - 2, x, y + 2),
            5 => Area(x - 2, y - 3, x + 2, y),
            7 => Area(x, y - 2, x + 3, y + 2),
            _ => Area(x - 1, y - 1, x + 1, y + 1),
        };
    }

    /// <inheritdoc/>
    public override ValueTask ReflectDamageAsync(IAttacker reflector, uint damage)
    {
        // A gate doesn't attack, so it doesn't reflect.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask ApplyPoisonDamageAsync(IAttacker initialAttacker, uint damage)
    {
        // A gate is not an organism which can be poisoned.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask ApplyBleedingDamageAsync(IAttacker initialAttacker, uint damage)
    {
        // A gate is not an organism which can bleed.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override bool CanBeAttackedBy(IAttacker attacker) => this.IsAttackable;

    private static (byte StartX, byte StartY, byte EndX, byte EndY) Area(int startX, int startY, int endX, int endY)
    {
        return (Clamp(startX), Clamp(startY), Clamp(endX), Clamp(endY));

        static byte Clamp(int value) => (byte)Math.Clamp(value, byte.MinValue, byte.MaxValue);
    }
}
