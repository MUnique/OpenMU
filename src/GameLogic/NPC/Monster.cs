﻿// <copyright file="Monster.cs" company="MUnique">
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
/// The implementation of a monster, which can attack players.
/// </summary>
public sealed class Monster : NonPlayerCharacter, IAttackable, IAttacker, ISupportWalk, IMovable
{
    private readonly IDropGenerator _dropGenerator;
    private readonly object _moveLock = new ();
    private readonly INpcIntelligence _intelligence;
    private readonly PlugInManager _plugInManager;
    private readonly Walker _walker;
    private readonly IEventStateProvider? _eventStateProvider;

    private Timer? _respawnTimer;
    private int _health;
    private bool _isCalculatingPath;
    private PathFinder? _pathFinder;

    /// <summary>
    /// Initializes a new instance of the <see cref="Monster" /> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The stats.</param>
    /// <param name="map">The map on which this instance will spawn.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    /// <param name="npcIntelligence">The monster intelligence.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    /// <param name="eventStateProvider">The event state provider.</param>
    public Monster(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map, IDropGenerator dropGenerator, INpcIntelligence npcIntelligence, PlugInManager plugInManager, IEventStateProvider? eventStateProvider = null)
        : base(spawnInfo, stats, map)
    {
        this._dropGenerator = dropGenerator;
        this.Attributes = new MonsterAttributeHolder(this);
        this.MagicEffectList = new MagicEffectsList(this);
        this._walker = new Walker(this, () => this.StepDelay);
        this._intelligence = npcIntelligence;
        this._plugInManager = plugInManager;
        this._eventStateProvider = eventStateProvider;
        this._intelligence.Npc = this;
        this._intelligence.Start();
    }

    /// <summary>
    /// Occurs when this instance died.
    /// </summary>
    public event EventHandler<DeathInformation>? Died;

    /// <inheritdoc/>
    public MagicEffectsList MagicEffectList { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Monster"/> is walking.
    /// </summary>
    /// <value>
    ///   <c>true</c> if walking; otherwise, <c>false</c>.
    /// </value>
    public bool IsWalking => this.WalkTarget != default;

    /// <summary>
    /// Gets a value indicating whether this <see cref="IAttackable" /> is currently teleporting and can't be directly targeted.
    /// It can still receive damage, if the teleport target coordinates are within an target skill area for area attacks.
    /// </summary>
    /// <value>
    ///   <c>true</c> if teleporting; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>Teleporting for monsters oor npcs is not implemented yet.</remarks>
    public bool IsTeleporting => false;

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

    /// <summary>
    /// Gets the target by which this instance was summoned by.
    /// </summary>
    public Player? SummonedBy => (this._intelligence as SummonedMonsterIntelligence)?.Owner;

    /// <inheritdoc/>
    public bool IsAlive { get; set; }

    /// <inheritdoc/>
    public DeathInformation? LastDeath { get; private set; }

    /// <inheritdoc cref="IAttackable" />
    public IAttributeSystem Attributes { get; }

    /// <inheritdoc/>
    public Point WalkTarget => this._walker.CurrentTarget;

    /// <inheritdoc/>
    public TimeSpan StepDelay => this.Definition.MoveDelay;

    private bool ShouldRespawn => this.SpawnArea.SpawnTrigger == SpawnTrigger.Automatic
                                  || (this.SpawnArea.SpawnTrigger == SpawnTrigger.AutomaticDuringEvent && (this._eventStateProvider?.IsEventRunning ?? false))
                                  || (this.SpawnArea.SpawnTrigger == SpawnTrigger.AutomaticDuringWave && (this._eventStateProvider?.IsSpawnWaveActive(this.SpawnArea.WaveNumber) ?? false));

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        this._respawnTimer?.Dispose();
        this.Health = (int)this.Attributes[Stats.MaximumHealth];
        this.IsAlive = true;
    }

    /// <summary>
    /// Attacks the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    public void Attack(IAttackable target)
    {
        target.AttackBy(this, null);
        this.ForEachWorldObserver(p => p.ViewPlugIns.GetPlugIn<IShowAnimationPlugIn>()?.ShowMonsterAttackAnimation(this, target, this.GetDirectionTo(target)), true);
        if (this.Definition.AttackSkill is { } attackSkill)
        {
            this.ForEachWorldObserver(p => p.ViewPlugIns.GetPlugIn<IShowSkillAnimationPlugIn>()?.ShowSkillAnimation(this, target, attackSkill, true), true);
        }
    }

    /// <summary>
    /// Walks to the target coordinates.
    /// </summary>
    /// <param name="target">The target object.</param>
    public void WalkTo(Point target)
    {
        if (this._isCalculatingPath || this.IsWalking)
        {
            return;
        }

        IList<PathResultNode>? calculatedPath;
        this._isCalculatingPath = true;
        try
        {
            if (this._pathFinder is null)
            {
                this._pathFinder = new PathFinder(new GridNetwork(this.CurrentMap.Terrain.AIgrid, true));
            }
            else
            {
                this._pathFinder.ResetPathFinder();
            }

            calculatedPath = this._pathFinder.FindPath(this.Position, target);
            if (calculatedPath is null)
            {
                return;
            }
        }
        finally
        {
            this._isCalculatingPath = false;
        }

        var targetNode = calculatedPath.Last(); // that's one step before the target coordinates actually are reached.
        Span<WalkingStep> steps = stackalloc WalkingStep[calculatedPath.Count];
        var i = 0;
        foreach (var step in calculatedPath.Select(GetStep))
        {
            steps[i] = step;
            i++;
        }

        this.WalkTo(new Point(targetNode.X, targetNode.Y), steps);
    }

    /// <summary>
    /// Walks to the target object.
    /// </summary>
    /// <param name="target">The target object.</param>
    public void WalkTo(ILocateable target) => this.WalkTo(target.Position);

    /// <summary>
    /// Walks to the specified target coordinates using the specified steps.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="steps">The steps.</param>
    public void WalkTo(Point target, Span<WalkingStep> steps)
    {
        this._walker.Stop();
        this._walker.WalkTo(target, steps);
        this.Move(target, MoveType.Walk);
    }

    /// <inheritdoc/>
    public int GetDirections(Span<Direction> directions)
    {
        return this._walker.GetDirections(directions);
    }

    /// <inheritdoc />
    public int GetSteps(Span<WalkingStep> steps) => this._walker.GetSteps(steps);

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
    public void ReflectDamage(IAttacker reflector, uint damage)
    {
        this.Hit(new HitInfo(damage, 0, DamageAttributes.Reflected), reflector, null);
    }

    /// <inheritdoc />
    public void ApplyPoisonDamage(IAttacker initialAttacker, uint damage)
    {
        this.Hit(new HitInfo(damage, 0, DamageAttributes.Poison), initialAttacker, null);
    }

    /// <inheritdoc/>
    public void Move(Point target)
    {
        this.Move(target, MoveType.Instant);
    }

    /// <summary>
    /// Moves this instance randomly.
    /// </summary>
    internal void RandomMove()
    {
        byte randx = (byte)GameLogic.Rand.NextInt(Math.Max(0, this.Position.X - 1), Math.Min(0xFF, this.Position.X + 2));
        byte randy = (byte)GameLogic.Rand.NextInt(Math.Max(0, this.Position.Y - 1), Math.Min(0xFF, this.Position.Y + 2));
        if (this.CurrentMap.Terrain.AIgrid[randx, randy] == 1)
        {
            var target = new Point(randx, randy);
            var current = this.Position;
            Span<WalkingStep> steps = stackalloc WalkingStep[1];
            steps[0] = new WalkingStep
            {
                From = current,
                To = target,
                Direction = current.GetDirectionTo(target),
            };
            this.WalkTo(target, steps);
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool managed)
    {
        if (managed)
        {
            this._respawnTimer?.Dispose();
            this._walker.Dispose();
            (this._intelligence as IDisposable)?.Dispose();
            this.IsAlive = false;
        }

        base.Dispose(managed);
    }

    /// <inheritdoc />
    protected override void Move(Point target, MoveType type)
    {
        if (type == MoveType.Instant || type == MoveType.Teleport)
        {
            this._walker.Stop();
        }

        this.CurrentMap.Move(this, target, this._moveLock, type);
    }

    private static WalkingStep GetStep(PathResultNode node)
    {
        return new ()
        {
            Direction = node.PreviousPoint.GetDirectionTo(new Point(node.X, node.Y)),
            From = node.PreviousPoint,
            To = new Point(node.X, node.Y),
        };
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

    private void OnDeath(IAttacker attacker)
    {
        this._walker.Stop();
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

        var player = attacker as Player ?? (attacker as Monster)?.SummonedBy;
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

        if (!this.ShouldRespawn)
        {
            this.RemoveFromMapAndDispose();
        }
    }

    private void RemoveFromMapAndDispose()
    {
        this.CurrentMap.Remove(this);
        this.Dispose();
        if (this._intelligence is SummonedMonsterIntelligence summonedMonsterIntelligence)
        {
            summonedMonsterIntelligence.Owner.SummonDied();
        }
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

    private void Hit(HitInfo hitInfo, IAttacker attacker, Skill? skill)
    {
        if (!this.IsAlive)
        {
            return;
        }

        var killed = this.TryHit(hitInfo.HealthDamage + hitInfo.ShieldDamage, attacker);

        var player = attacker as Player ?? (attacker as Monster)?.SummonedBy ?? this.SummonedBy;
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
        }
    }

    private bool TryHit(uint damage, IAttacker attacker)
    {
        if (damage > 0)
        {
            this._intelligence.RegisterHit(attacker);
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
}