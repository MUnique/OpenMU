﻿// <copyright file="Trap.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// The implementation of a monster, which can attack players.
/// </summary>
public sealed class Trap : NonPlayerCharacter, IAttacker
{
    private const byte TrapAttackAnimation = 0x78;
    private readonly INpcIntelligence _intelligence;

    /// <summary>
    /// Initializes a new instance of the <see cref="Trap"/> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The stats.</param>
    /// <param name="map">The map on which this instance will spawn.</param>
    /// <param name="trapIntelligence">The trap intelligence.</param>
    public Trap(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map, INpcIntelligence trapIntelligence)
        : base(spawnInfo, stats, map)
    {
        this.Attributes = new TrapAttributeHolder(this);
        this._intelligence = trapIntelligence;
        this._intelligence.Npc = this;
        this._intelligence.Start();
    }

    /// <inheritdoc/>
    public IAttributeSystem Attributes { get; }

    /// <summary>
    /// Attacks the specified player.
    /// </summary>
    /// <param name="player">The player.</param>
    public void Attack(IAttackable player)
    {
        // need to find specific animation
        // Maybe add SpecificAnimation and AttackWhenPlayerOn properties to MonsterDefinition?? or create new TrapDefinition?
        player.AttackBy(this, null);
        this.ForEachWorldObserver(p => p.ViewPlugIns.GetPlugIn<IShowAnimationPlugIn>()?.ShowAnimation(this, TrapAttackAnimation, player, this.Rotation), true);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool managed)
    {
        base.Dispose(managed);
        if (managed)
        {
            (this._intelligence as IDisposable)?.Dispose();
        }
    }
}