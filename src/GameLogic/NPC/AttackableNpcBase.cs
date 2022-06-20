// <copyright file="AttackableNpcBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using System.Diagnostics;
using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// An abstract base class for an <see cref="IAttackable"/> <see cref="NonPlayerCharacter"/>.
/// </summary>
public abstract class AttackableNpcBase : NonPlayerCharacter, IAttackable
{
    private readonly IEventStateProvider? _eventStateProvider;
    private readonly IDropGenerator _dropGenerator;
    private readonly PlugInManager _plugInManager;
    private Timer? _respawnTimer;
    private int _health;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttackableNpcBase" /> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The stats.</param>
    /// <param name="map">The map.</param>
    /// <param name="eventStateProvider">The event state provider.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    protected AttackableNpcBase(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map, IEventStateProvider? eventStateProvider, IDropGenerator dropGenerator, PlugInManager plugInManager)
        : base(spawnInfo, stats, map)
    {
        this._eventStateProvider = eventStateProvider;
        this._dropGenerator = dropGenerator;
        this._plugInManager = plugInManager;
        this.MagicEffectList = new MagicEffectsList(this);
        this.Attributes = new MonsterAttributeHolder(this);
    }

    /// <summary>
    /// Occurs when this instance died.
    /// </summary>
    public event EventHandler<DeathInformation>? Died;

    /// <inheritdoc />
    public IAttributeSystem Attributes { get; }

    /// <inheritdoc />
    public MagicEffectsList MagicEffectList { get; }

    /// <inheritdoc />
    public bool IsAlive { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="IAttackable" /> is currently teleporting and can't be directly targeted.
    /// It can still receive damage, if the teleport target coordinates are within an target skill area for area attacks.
    /// </summary>
    /// <value>
    ///   <c>true</c> if teleporting; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>Teleporting for monsters oor npcs is not implemented yet.</remarks>
    public bool IsTeleporting => false;

    /// <inheritdoc />
    public DeathInformation? LastDeath { get; protected set; }

    /// <inheritdoc/>
    public override Point Position
    {
        get => base.Position;
        set
        {
            if (base.Position != value)
            {
                base.Position = value;
                this._plugInManager?.GetPlugInPoint<IAttackableMovedPlugIn>()?.AttackableMoved(this);
            }
        }
    }

    /// <summary>
    /// Gets or sets the current health.
    /// </summary>
    public int Health
    {
        get => Math.Max(this._health, 0);
        set => this._health = value;
    }

    private bool ShouldRespawn => this.SpawnArea.SpawnTrigger == SpawnTrigger.Automatic
                                  || (this.SpawnArea.SpawnTrigger == SpawnTrigger.AutomaticDuringEvent && (this._eventStateProvider?.IsEventRunning ?? false))
                                  || (this.SpawnArea.SpawnTrigger == SpawnTrigger.AutomaticDuringWave && (this._eventStateProvider?.IsSpawnWaveActive(this.SpawnArea.WaveNumber) ?? false));

    /// <inheritdoc />
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

    /// <inheritdoc />
    public abstract void ReflectDamage(IAttacker reflector, uint damage);

    /// <inheritdoc />
    public abstract void ApplyPoisonDamage(IAttacker initialAttacker, uint damage);

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        this._respawnTimer?.Dispose();
        this.Health = this.SpawnArea.MaximumHealthOverride ?? (int)this.Attributes[Stats.MaximumHealth];
        this.IsAlive = true;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool managed)
    {
        if (managed)
        {
            this.Died = null;
            this._respawnTimer?.Dispose();
            this.IsAlive = false;
        }

        base.Dispose(managed);
    }

    /// <summary>
    /// Called when the object is removed from the map.
    /// </summary>
    protected virtual void OnRemoveFromMap()
    {
        // can be overwritten to do additional stuff.
    }

    private void RemoveFromMapAndDispose()
    {
        this.CurrentMap.Remove(this);
        this.Dispose();
        this.OnRemoveFromMap();
    }

    /// <summary>
    /// Respawns this instance on the map.
    /// </summary>
    private void Respawn()
    {
        try
        {
            if (!this.ShouldRespawn)
            {
                this.RemoveFromMapAndDispose();
                return;
            }

            this.Initialize();
            this.CurrentMap.Respawn(this);
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
    }

    /// <summary>
    /// Hits this instance with the specified hit information.
    /// </summary>
    /// <param name="hitInfo">The hit information.</param>
    /// <param name="attacker">The attacker.</param>
    /// <param name="skill">The skill.</param>
    protected void Hit(HitInfo hitInfo, IAttacker attacker, Skill? skill)
    {
        if (!this.IsAlive)
        {
            return;
        }

        var killed = this.TryHit(hitInfo.HealthDamage + hitInfo.ShieldDamage, attacker);

        var player = this.GetHitNotificationTarget(attacker);
        if (player is not null)
        {
            player.ViewPlugIns.GetPlugIn<IShowHitPlugIn>()?.ShowHit(this, hitInfo);
            player.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotHitPlugIn>()?.AttackableGotHit(this, attacker, hitInfo);
        }

        if (killed)
        {
            this.LastDeath = new DeathInformation(attacker.Id, attacker.GetName(), hitInfo, skill?.Number ?? 0);
            this.OnDeath(attacker);
            this.Died?.Invoke(this, this.LastDeath);
            if (!this.ShouldRespawn)
            {
                this.RemoveFromMapAndDispose();
            }
        }
    }

    /// <summary>
    /// Gets the target of a hit notification.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    /// <returns>The target player of a hit notification.</returns>
    protected virtual Player? GetHitNotificationTarget(IAttacker attacker)
    {
        return attacker as Player;
    }

    /// <summary>
    /// Registers the hit.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    protected virtual void RegisterHit(IAttacker attacker)
    {
        // can be overwritten;
    }

    private bool TryHit(uint damage, IAttacker attacker)
    {
        if (damage > 0)
        {
            this.RegisterHit(attacker);
        }

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

    private void HandleMoneyDrop(uint amount, Player killer)
    {
        // We don't drop money in Devil Square, etc.
        var shouldDropMoney = killer.GameContext.Configuration.ShouldDropMoney && killer.CurrentMiniGame is null;
        if (!shouldDropMoney)
        {
            var party = killer.Party;
            if (party is null)
            {
                killer.TryAddMoney((int)amount);
            }
            else
            {
                party.DistributeMoneyAfterKill(this, killer, amount);
            }

            return;
        }

        var droppedMoney = new DroppedMoney((uint)(amount * killer.Attributes![Stats.MoneyAmountRate]), this.Position, this.CurrentMap);
        this.CurrentMap.Add(droppedMoney);
    }

    private void DropItem(int exp, Player killer)
    {
        var generatedItems = this._dropGenerator.GenerateItemDrops(this.Definition, exp, killer, out var droppedMoney);
        if (droppedMoney > 0)
        {
            this.HandleMoneyDrop(droppedMoney.Value, killer);
        }

        var firstItem = !droppedMoney.HasValue;
        foreach (var item in generatedItems)
        {
            Point dropCoordinates;
            if (firstItem)
            {
                dropCoordinates = this.Position;
                firstItem = false;
            }
            else
            {
                dropCoordinates = this.CurrentMap.Terrain.GetRandomCoordinate(this.Position, 4);
            }

            var owners = killer.Party?.PartyList.AsEnumerable() ?? killer.GetAsEnumerable();
            var droppedItem = new DroppedItem(item, dropCoordinates, this.CurrentMap, null, owners);
            this.CurrentMap.Add(droppedItem);
        }
    }

    /// <summary>
    /// Called when this instance died.
    /// </summary>
    /// <param name="attacker">The attacker which killed this instance.</param>
    protected virtual void OnDeath(IAttacker attacker)
    {
        if (this.ShouldRespawn)
        {
            this._respawnTimer = new Timer(_ => this.Respawn(), null, (int)this.Definition.RespawnDelay.TotalMilliseconds, System.Threading.Timeout.Infinite);
        }

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

        var player = this.GetHitNotificationTarget(attacker);
        if (player is { })
        {
            int exp = player.Party?.DistributeExperienceAfterKill(this, player) ?? player.AddExpAfterKill(this);
            this.DropItem(exp, player);
            if (attacker == player)
            {
                player.AfterKilledMonster();
            }

            player.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotKilledPlugIn>()?.AttackableGotKilled(this, attacker);
            if (player.SelectedCharacter!.State > HeroState.Normal)
            {
                player.SelectedCharacter.StateRemainingSeconds -= (int)this.Attributes[Stats.Level];
            }
        }
    }
}
