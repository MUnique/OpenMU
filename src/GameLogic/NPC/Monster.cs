// <copyright file="Monster.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The implementation of a monster, which can attack players.
/// </summary>
public sealed class Monster : AttackableNpcBase, IAttackable, IAttacker, ISupportWalk, IMovable
{
    private readonly object _moveLock = new ();
    private readonly INpcIntelligence _intelligence;
    private readonly Walker _walker;

    /// <summary>
    /// The power up element of the monsters skill.
    /// It is a "cached" element which will be created on demand and can be applied multiple times.
    /// </summary>
    private readonly IElement? _skillPowerUp;

    /// <summary>
    /// The duration of the <see cref="_skillPowerUp"/>.
    /// </summary>
    /// <remarks>
    /// It is an IElement, because the duration can be dependent from the player attributes.
    /// </remarks>
    private readonly IElement? _skillPowerUpDuration;

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
        : base(spawnInfo, stats, map, eventStateProvider, dropGenerator, plugInManager)
    {
        this._walker = new Walker(this, () => this.StepDelay);
        this._intelligence = npcIntelligence;

        (this._skillPowerUp, this._skillPowerUpDuration) = this.CreateMagicEffectPowerUp();

        this._intelligence.Npc = this;
        this._intelligence.Start();
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Monster"/> is walking.
    /// </summary>
    /// <value>
    ///   <c>true</c> if walking; otherwise, <c>false</c>.
    /// </value>
    public bool IsWalking => this.WalkTarget != default;

    /// <summary>
    /// Gets the target by which this instance was summoned by.
    /// </summary>
    public Player? SummonedBy => (this._intelligence as SummonedMonsterIntelligence)?.Owner;

    /// <inheritdoc/>
    public Point WalkTarget => this._walker.CurrentTarget;

    /// <inheritdoc/>
    public TimeSpan StepDelay => this.Definition.MoveDelay;

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
            target.TryApplyElementalEffects(this, attackSkill, this._skillPowerUp, this._skillPowerUpDuration);

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
    public override void ReflectDamage(IAttacker reflector, uint damage)
    {
        this.Hit(new HitInfo(damage, 0, DamageAttributes.Reflected), reflector, null);
    }

    /// <inheritdoc />
    public override void ApplyPoisonDamage(IAttacker initialAttacker, uint damage)
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
            this._walker.Dispose();
            (this._intelligence as IDisposable)?.Dispose();
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
    protected override void OnDeath(IAttacker attacker)
    {
        this._walker.Stop();
        base.OnDeath(attacker);
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

    /// <summary>
    /// Creates the magic effect power up for the given skill of a monster.
    /// </summary>
    private (IElement? PowerUp, IElement? Duration) CreateMagicEffectPowerUp()
    {
        var skill = this.Definition.AttackSkill;
        if (skill?.MagicEffectDef is null)
        {
            return (null, null);
        }

        if (skill.MagicEffectDef.PowerUpDefinition?.Boost is null)
        {
            throw new InvalidOperationException($"Skill {skill.Name} ({skill.Number}) has no magic effect definition or is without a PowerUpDefintion.");
        }

        if (skill.MagicEffectDef.PowerUpDefinition.Duration is null)
        {
            throw new InvalidOperationException($"PowerUpDefinition {skill.MagicEffectDef.PowerUpDefinition.GetId()} no Duration.");
        }

        var powerUpDef = skill.MagicEffectDef.PowerUpDefinition;
        return (this.Attributes!.CreateElement(powerUpDef.Boost), this.Attributes!.CreateElement(powerUpDef.Duration));
    }
}