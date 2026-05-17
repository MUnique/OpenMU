// <copyright file="Monster.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using System.Buffers;
using System.Diagnostics;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;

/// <summary>
/// The implementation of a monster, which can attack players.
/// </summary>
public sealed class Monster : AttackableNpcBase, IAttackable, IAttacker, ISupportWalk, IMovable, ISummonable
{
    private readonly AsyncLock _moveLock = new();
    private readonly INpcIntelligence _intelligence;
    private readonly Walker _walker;

    /// <summary>
    /// The power up elements of the monster skill.
    /// These are "cached" elements which will be created on demand and can be applied multiple times.
    /// </summary>
    private readonly (AttributeDefinition Target, IElement Boost)[] _skillPowerUps;

    /// <summary>
    /// The duration of the <see cref="_skillPowerUps"/>.
    /// </summary>
    private readonly IElement? _skillPowerUpDuration;

    private readonly IObjectPool<PathFinder> _pathFinderPool;

    private bool _isCalculatingPath;

    private bool _isReadyToWalk;

    /// <summary>
    /// Initializes a new instance of the <see cref="Monster" /> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The stats.</param>
    /// <param name="map">The map on which this instance will spawn.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    /// <param name="npcIntelligence">The monster intelligence.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    /// <param name="pathFinderPool">The path finder pool.</param>
    /// <param name="eventStateProvider">The event state provider.</param>
    public Monster(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map, IDropGenerator dropGenerator, INpcIntelligence npcIntelligence, PlugInManager plugInManager, IObjectPool<PathFinder> pathFinderPool, IEventStateProvider? eventStateProvider = null)
        : base(spawnInfo, stats, map, eventStateProvider, dropGenerator, plugInManager)
    {
        this._pathFinderPool = pathFinderPool;
        this._walker = new Walker(this, this.GetStepDelay);
        this._intelligence = npcIntelligence;

        (this._skillPowerUps, this._skillPowerUpDuration) = this.CreateMagicEffectPowerUps();

        this._intelligence.Npc = this;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Monster"/> is walking.
    /// </summary>
    /// <value>
    ///   <c>true</c> if walking; otherwise, <c>false</c>.
    /// </value>
    public bool IsWalking => this.WalkTarget != default;

    /// <inheritdoc />
    public bool CanWalkOnSafezone => this._intelligence.CanWalkOnSafezone;

    /// <summary>
    /// Gets the target by which this instance was summoned by.
    /// </summary>
    public Player? SummonedBy => (this._intelligence as SummonedMonsterIntelligence)?.Owner;

    /// <inheritdoc />
    public Point WalkTarget => this._walker.CurrentTarget;

    /// <inheritdoc/>
    public TimeSpan StepDelay => this.GetStepDelay(null);

    /// <inheritdoc/>
    /// <remarks>Monsters don't do combos.</remarks>
    public ComboStateMachine? ComboState => null;

    /// <inheritdoc/>
    protected override bool CanSpawnInSafezone => base.CanSpawnInSafezone || this.SummonedBy is not null;

    /// <summary>
    /// Determines whether the specified player is being targeted by this monster.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <returns>True if the player is the current target; otherwise, false.</returns>
    public bool IsTargetingPlayer(Player player)
    {
        if (this._intelligence is BasicMonsterIntelligence basicIntelligence)
        {
            return basicIntelligence.IsTargetingPlayer(player);
        }

        return false;
    }

    /// <summary>
    /// Attacks the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    public async ValueTask AttackAsync(IAttackable target)
    {
        var hitInfo = await target.AttackByAsync(this, null, false).ConfigureAwait(false);

        await this.ForEachWorldObserverAsync<IShowAnimationPlugIn>(p => p.ShowMonsterAttackAnimationAsync(this, target, this.GetDirectionTo(target)), true).ConfigureAwait(false);
        if (this.Definition.AttackSkill is { } attackSkill)
        {
            await target.TryApplyElementalEffectsAsync(this, attackSkill, this._skillPowerUps, this._skillPowerUpDuration, hitInfo).ConfigureAwait(false);

            await this.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowSkillAnimationAsync(this, target, attackSkill, true), true).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public override void OnSpawn()
    {
        base.OnSpawn();
        this._isReadyToWalk = true;
    }

    /// <summary>
    /// Walks to the target coordinates.
    /// </summary>
    /// <param name="target">The target object.</param>
    public async ValueTask<bool> WalkToAsync(Point target)
    {
        if (this._isCalculatingPath || this.IsWalking || !this._isReadyToWalk)
        {
            return false;
        }

        IList<PathResultNode>? calculatedPath;
        this._isCalculatingPath = true;
        PathFinder? pathFinder = null;
        try
        {
            pathFinder = await this._pathFinderPool.GetAsync().ConfigureAwait(false);
            pathFinder.ResetPathFinder();
            calculatedPath = pathFinder.FindPath(this.Position, target, this.CurrentMap.Terrain.AIgrid, this.CanWalkOnSafezone);
            if (calculatedPath is null)
            {
                return false;
            }
        }
        finally
        {
            this._isCalculatingPath = false;
            if (pathFinder is not null)
            {
                this._pathFinderPool.Return(pathFinder);
            }
        }

        if (calculatedPath.Count == 0)
        {
            return false;
        }

        // The walker just supports maximum 16 steps.
        while (calculatedPath.Count > 16)
        {
            calculatedPath.RemoveAt(calculatedPath.Count - 1);
        }

        var targetNode = calculatedPath.Last(); // that's one step before the target coordinates actually are reached.
        using var stepsRent = MemoryPool<WalkingStep>.Shared.Rent(calculatedPath.Count);
        var steps = stepsRent.Memory.Slice(0, calculatedPath.Count);
        var i = 0;
        foreach (var step in calculatedPath.Select(GetStep))
        {
            steps.Span[i] = step;
            i++;
        }

        await this.WalkToAsync(new Point(targetNode.X, targetNode.Y), steps).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Walks to the target object.
    /// </summary>
    /// <param name="target">The target object.</param>
    public ValueTask<bool> WalkToAsync(ILocateable target) => this.WalkToAsync(target.Position);

    /// <summary>
    /// Walks to the specified target coordinates using the specified steps.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="steps">The steps.</param>
    public async ValueTask WalkToAsync(Point target, Memory<WalkingStep> steps)
    {
        if (Debugger.IsAttached)
        {
            this.ValidatePath(steps);
        }

        await this._walker.StopAsync().ConfigureAwait(false);
        var token = await this._walker.InitializeWalkToAsync(target, steps).ConfigureAwait(false);
        await this.MoveAsync(target, MoveType.Walk).ConfigureAwait(false);
        await this._walker.StartWalkAsync(token).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask<int> GetDirectionsAsync(Memory<Direction> directions)
    {
        return this._walker.GetDirectionsAsync(directions);
    }

    /// <inheritdoc />
    public ValueTask<int> GetStepsAsync(Memory<WalkingStep> steps) => this._walker.GetStepsAsync(steps);

    /// <inheritdoc />
    public ValueTask StopWalkingAsync() => this._walker.StopAsync();

    /// <inheritdoc />
    public override ValueTask ReflectDamageAsync(IAttacker reflector, uint damage)
    {
        return this.HitAsync(new HitInfo(damage, 0, DamageAttributes.Reflected), reflector, null);
    }

    /// <inheritdoc />
    public override ValueTask ApplyPoisonDamageAsync(IAttacker initialAttacker, uint damage)
    {
        return this.HitAsync(new HitInfo(damage, 0, DamageAttributes.Poison), initialAttacker, null);
    }

    /// <inheritdoc />
    public override ValueTask ApplyBleedingDamageAsync(IAttacker initialAttacker, uint damage)
    {
        return this.HitAsync(new HitInfo(damage, 0, DamageAttributes.Undefined), initialAttacker, null);
    }

    /// <inheritdoc/>
    public ValueTask MoveAsync(Point target)
    {
        return this.MoveAsync(target, MoveType.Instant);
    }

    /// <summary>
    /// Moves this instance randomly.
    /// </summary>
    internal async ValueTask RandomMoveAsync()
    {
        if (!this._isReadyToWalk)
        {
            return;
        }

        var moveByX = Rand.NextInt(-this.Definition.MoveRange, this.Definition.MoveRange + 1);
        var moveByY = Rand.NextInt(-this.Definition.MoveRange, this.Definition.MoveRange + 1);

        var newX = this.Position.X + moveByX;
        var newY = this.Position.Y + moveByY;
        byte randx = (byte)Math.Min(0xFF, Math.Max(0, newX));
        byte randy = (byte)Math.Min(0xFF, Math.Max(0, newY));

        var target = new Point(randx, randy);
        if (this._intelligence.CanWalkOn(target))
        {
            await this.WalkToAsync(target).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    protected override void OnFirstObserverAdded()
    {
        base.OnFirstObserverAdded();
        this._intelligence.Start();
    }

    /// <inheritdoc/>
    protected override void OnLastObserverRemoved()
    {
        base.OnLastObserverRemoved();
        this._intelligence.Pause();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool managed)
    {
        if (managed)
        {
            this._walker.Dispose();
            (this._intelligence as IDisposable)?.Dispose();
        }

        base.Dispose(managed);
    }

    /// <inheritdoc />
    protected override async ValueTask MoveAsync(Point target, MoveType type)
    {
        if (type == MoveType.Instant || type == MoveType.Teleport)
        {
            await this._walker.StopAsync().ConfigureAwait(false);
        }

        await this.CurrentMap.MoveAsync(this, target, this._moveLock, type).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void OnRemoveFromMap()
    {
        base.OnRemoveFromMap();
        if (this._intelligence is SummonedMonsterIntelligence summonedMonsterIntelligence)
        {
            summonedMonsterIntelligence.Owner.SummonDied();
        }
    }

    /// <inheritdoc />
    protected override void RegisterHit(IAttacker attacker)
    {
        base.RegisterHit(attacker);
        this._intelligence.RegisterHit(attacker);
    }

    /// <inheritdoc />
    protected override Player? GetHitNotificationTarget(IAttacker attacker)
    {
        return base.GetHitNotificationTarget(attacker)
               ?? (attacker as Monster)?.SummonedBy ?? this.SummonedBy;
    }

    /// <inheritdoc />
    protected override async ValueTask OnDeathAsync(IAttacker attacker)
    {
        await this._walker.StopAsync().ConfigureAwait(false);
        await base.OnDeathAsync(attacker).ConfigureAwait(false);
    }

    private static WalkingStep GetStep(PathResultNode node)
    {
        return new()
        {
            Direction = node.PreviousPoint.GetDirectionTo(new Point(node.X, node.Y)),
            From = node.PreviousPoint,
            To = new Point(node.X, node.Y),
        };
    }

    private TimeSpan GetStepDelay(WalkingStep? step)
    {
        var tileDistance = step is { } walkingStep ? walkingStep.From.EuclideanDistanceTo(walkingStep.To) : 1.0;
        var delayMilliseconds = this.Definition.MoveDelay.TotalMilliseconds * Math.Max(1.0, tileDistance);
        delayMilliseconds /= this.GetMovementSpeedFactor();

        return TimeSpan.FromMilliseconds(delayMilliseconds);
    }

    private double GetMovementSpeedFactor()
    {
        var movementSpeedFactor = this.Attributes[Stats.MovementSpeedFactor];

        return movementSpeedFactor > 0 ? movementSpeedFactor : 1.0;
    }

    /// <summary>
    /// Creates the magic effect power ups for the given skill of a monster.
    /// </summary>
    private ((AttributeDefinition Target, IElement Boost)[] PowerUps, IElement? Duration) CreateMagicEffectPowerUps()
    {
        var skill = this.Definition.AttackSkill;
        if (skill?.MagicEffectDef is not { } magicEffectDefinition
            || magicEffectDefinition.Duration is not { } duration)
        {
            return ([], null);
        }

        if (magicEffectDefinition.PowerUpDefinitions.Any(p => p.Boost is null || p.TargetAttribute is null))
        {
            throw new InvalidOperationException($"Skill {skill.Name} ({skill.Number}) has no magic effect definition or is without a PowerUpDefinition.");
        }

        return (
            [.. magicEffectDefinition.PowerUpDefinitions.Select(p => (p.TargetAttribute!, this.Attributes.CreateElement(p)))],
            this.Attributes.CreateDurationElement(duration));
    }

    private void ValidatePath(Memory<WalkingStep> steps)
    {
        const double maxInitialStepDistanceThreshold = 1.5; // Greater than sqrt(2) to consider diagonal steps, but lower than 2.

        if (this.Position.EuclideanDistanceTo(steps.GetStart()) > maxInitialStepDistanceThreshold)
        {
            Debugger.Break();
        }

        foreach (var step in steps.Span)
        {
            if (this.CurrentMap.Terrain.AIgrid[step.To.X, step.To.Y] == 0)
            {
                Debugger.Break();
            }

            if (step.To.EuclideanDistanceTo(step.From) > maxInitialStepDistanceThreshold)
            {
                Debugger.Break();
            }
        }
    }
}
