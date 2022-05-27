// <copyright file="Destructible.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.World;
using System.Threading;

/// <summary>
/// The implementation of a destructible, which can attack players.
/// </summary>
public sealed class Destructible : NonPlayerCharacter, IAttackable
{
    private int _health;

    /// <summary>
    /// Initializes a new instance of the <see cref="Destructible" /> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The stats.</param>
    /// <param name="map">The map on which this instance will spawn.</param>
    public Destructible(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map)
        : base(spawnInfo, stats, map)
    {
        this.Attributes = new DestructibleAttributeHolder(this);
    }

    /// <summary>
    /// Gets or sets the current health.
    /// </summary>
    public int Health
    {
        get => Math.Max(this._health, 0);
        set => this._health = value;
    }

    /// <inheritdoc cref="IAttackable" />
    public IAttributeSystem Attributes { get; }

    /// <inheritdoc/>
    public MagicEffectsList MagicEffectList => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool IsAlive { get; set; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="IAttackable" /> is currently teleporting and can't be directly targeted.
    /// It can still receive damage, if the teleport target coordinates are within an target skill area for area attacks.
    /// </summary>
    /// <value>
    ///   <c>true</c> if teleporting; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>Teleporting for monsters or npcs is not implemented yet.</remarks>
    public bool IsTeleporting => false;

    /// <inheritdoc/>
    public DeathInformation? LastDeath { get; private set; }

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        this.Health = (int)this.Attributes[Stats.MaximumHealth];
        this.IsAlive = true;
    }

    /// <inheritdoc/>
    public void AttackBy(IAttacker attacker, SkillEntry? skill)
    {
        var hitInfo = attacker.CalculateDamage(this, skill);
        this.Hit(hitInfo, attacker, skill?.Skill);
        if (hitInfo.HealthDamage > 0)
        {
            attacker.ApplyAmmunitionConsumption(hitInfo);
            (attacker as Player)?.AfterHitTarget();
        }
    }

    /// <inheritdoc/>
    public void ReflectDamage(IAttacker reflector, uint damage)
    {
        // A destructible doesn't attack, so it doesn't reflect.
    }

    /// <inheritdoc/>
    public void ApplyPoisonDamage(IAttacker initialAttacker, uint damage)
    {
        // A destructible can't be poisoned.
    }

    /// <inheritdoc/>
    protected override void Dispose(bool managed)
    {
        if (managed)
        {
            this.IsAlive = false;
        }

        base.Dispose(managed);
    }

    private void Hit(HitInfo hitInfo, IAttacker attacker, Skill? skill)
    {
        if (!this.IsAlive)
        {
            return;
        }

        var killed = this.TryHit(hitInfo.HealthDamage + hitInfo.ShieldDamage);

        var player = attacker as Player ?? (attacker as Monster)?.SummonedBy;
        if (player is not null)
        {
            player.ViewPlugIns.GetPlugIn<IShowHitPlugIn>()?.ShowHit(this, hitInfo);
            player.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotHitPlugIn>()?.AttackableGotHit(this, attacker, hitInfo);
        }

        if (killed)
        {
            this.OnDeath(attacker);
        }
    }

    private bool TryHit(uint damage)
    {
        if (damage >= this.Health)
        {
            this.IsAlive = false;
            this.Health = 0;
            return true;
        }

        try
        {
            Interlocked.Add(ref this._health, -(int)damage);
            return false;
        }
        catch
        {
            return false;
        }
    }

    private void OnDeath(IAttacker attacker)
    {
        this.MiniGameEvent(attacker);

        this.ObserverLock.EnterWriteLock();
        try
        {
            foreach (IWorldObserver o in this.Observers)
            {
                o.ViewPlugIns.GetPlugIn<IObjectGotKilledPlugIn>()?.ObjectGotKilled(this, attacker);
            }

            this.Observers.Clear();
        }
        finally
        {
            this.ObserverLock.ExitWriteLock();
        }

        var player = attacker as Player ?? (attacker as Monster)?.SummonedBy;
        if (player is { })
        {
            player.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotKilledPlugIn>()?.AttackableGotKilled(this, attacker);
        }

        this.RemoveFromMapAndDispose();
    }

    private void MiniGameEvent(IAttacker attacker)
    {
        var player = attacker as Player;
        if (player?.CurrentMiniGame is BloodCastleContext bloodCastle)
        {
            bloodCastle.OnDestructibleDied(player, this);
        }
    }

    private void RemoveFromMapAndDispose()
    {
        this.CurrentMap.Remove(this);
        this.Dispose();
    }
}