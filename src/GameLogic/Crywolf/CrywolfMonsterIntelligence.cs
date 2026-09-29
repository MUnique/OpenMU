// <copyright file="CrywolfMonsterIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// The intelligence of the monsters of the army of Balgass during the crywolf event.
/// </summary>
/// <remarks>
/// The monsters don't move or attack until the battle starts. Then, like in the original game:
/// <list type="bullet">
///   <item>The Dark Elf which leads a group marches to the goal of the group and revives the dead members of its group.</item>
///   <item>The soldiers follow their leader and attack the players around them.</item>
///   <item>The ballistas bombard a point of the fortress.</item>
///   <item>Balgass marches to the statue. When his health is low, he tries to escape from his target.</item>
/// </list>
/// The monsters use their skills (<see cref="CrywolfEventDefinition.MonsterSkills"/>) instead of a normal attack with a chance.
/// The goals are too far away for the path finder, so the monsters walk along the waypoints of the map.
/// </remarks>
public sealed class CrywolfMonsterIntelligence : INpcIntelligence, IDisposable
{
    /// <summary>
    /// The number of the magic effect of a stun.
    /// </summary>
    internal const short StunnedMagicEffectNumber = 61;

    /// <summary>
    /// The distance to the goal, below which a monster walks directly to it.
    /// </summary>
    private const int DirectWalkDistance = 10;

    /// <summary>
    /// The maximum distance of a waypoint, which a monster walks to.
    /// </summary>
    private const int WaypointDistance = 20;

    private readonly CrywolfContext _context;
    private readonly CrywolfMonsterGroup? _group;
    private readonly CrywolfBallista? _ballista;
    private readonly ILogger _logger;
    private Timer? _timer;
    private Monster? _monster;
    private IAttackable? _attacker;
    private IReadOnlyList<CrywolfMonsterSkill>? _skills;
    private int _isRunning;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrywolfMonsterIntelligence"/> class.
    /// </summary>
    /// <param name="context">The context of the event.</param>
    /// <param name="role">The role of the monster.</param>
    /// <param name="group">The group of the monster.</param>
    /// <param name="ballista">The definition of the ballista, if the monster is a ballista.</param>
    /// <param name="logger">The logger.</param>
    public CrywolfMonsterIntelligence(CrywolfContext context, CrywolfMonsterRole role, CrywolfMonsterGroup? group, CrywolfBallista? ballista, ILogger logger)
    {
        this._context = context;
        this.Role = role;
        this._group = group;
        this._ballista = ballista;
        this._logger = logger;
    }

    /// <summary>
    /// Gets the role of the monster.
    /// </summary>
    public CrywolfMonsterRole Role { get; }

    /// <inheritdoc />
    public NonPlayerCharacter Npc
    {
        get => this.Monster;
        set => this.Monster = (Monster)value;
    }

    /// <summary>
    /// Gets or sets the monster.
    /// </summary>
    public Monster Monster
    {
        get => this._monster ?? throw new InvalidOperationException("Instance is not initialized with a Monster yet");
        set => this._monster = value;
    }

    /// <inheritdoc />
    public bool CanWalkOnSafezone => false;

    /// <inheritdoc />
    public void RegisterHit(IAttacker attacker)
    {
        if (attacker is IAttackable attackable)
        {
            this._attacker = attackable;
        }
    }

    /// <inheritdoc />
    public void Start()
    {
        var interval = this.Role == CrywolfMonsterRole.Balgass && this._context.Definition.BalgassActionInterval > TimeSpan.Zero
            ? this._context.Definition.BalgassActionInterval
            : this.Monster.Definition.AttackDelay;
        this._timer ??= new Timer(state => _ = this.SafeTickAsync(), null, interval, interval);
    }

    /// <inheritdoc />
    /// <remarks>
    /// The army keeps marching when no player observes it, so pausing is ignored.
    /// </remarks>
    public void Pause()
    {
        // intentionally left blank.
    }

    /// <inheritdoc />
    public bool CanWalkOn(Point target)
    {
        return (this.Monster.CurrentMap.Terrain.AIgrid[target.X, target.Y] & 1) == 1;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._timer?.Dispose();
        this._timer = null;
    }

    /// <summary>
    /// Gets the point, to which a monster walks next to reach its goal.
    /// When the goal is far away, it's the waypoint near the monster, which is the closest to the goal.
    /// </summary>
    /// <param name="position">The position of the monster.</param>
    /// <param name="goal">The goal.</param>
    /// <returns>The point to walk to.</returns>
    internal static Point GetNextWalkTarget(Point position, Point goal)
    {
        var distanceToGoal = position.EuclideanDistanceTo(goal);
        if (distanceToGoal <= DirectWalkDistance)
        {
            return goal;
        }

        var waypoint = CrywolfMovePath.Waypoints
            .Where(point => position.EuclideanDistanceTo(point) < WaypointDistance)
            .MinBy(point => point.EuclideanDistanceTo(goal));
        return waypoint != default && waypoint.EuclideanDistanceTo(goal) < distanceToGoal ? waypoint : goal;
    }

    /// <summary>
    /// Executes one step of the intelligence. It's called periodically by a timer.
    /// </summary>
    internal async ValueTask TickAsync()
    {
        if (this._monster is not { IsAlive: true } monster
            || this._context.ArmyStage == CrywolfArmyStage.Waiting
            || monster.Attributes[Stats.IsStunned] > 0
            || monster.Attributes[Stats.IsAsleep] > 0)
        {
            return;
        }

        if (this.Role == CrywolfMonsterRole.Ballista)
        {
            if (this._ballista is { } ballista)
            {
                await this._context.BombardAsync(monster, ballista).ConfigureAwait(false);
            }

            return;
        }

        if (this.Role == CrywolfMonsterRole.Leader && this._group is { } group)
        {
            await this._context.ReviveGroupMembersAsync(group.WaveNumber).ConfigureAwait(false);
        }

        if (monster.IsWalking)
        {
            return;
        }

        if (await this.SearchTargetAsync(monster).ConfigureAwait(false) is { } target)
        {
            await this.AttackAsync(monster, target).ConfigureAwait(false);
            return;
        }

        if (monster.Attributes[Stats.IsFrozen] > 0 || this.GetGoal(monster) is not { } goal || monster.Position == goal)
        {
            return;
        }

        var walkTarget = GetNextWalkTarget(monster.Position, goal);
        if (!await monster.WalkToAsync(walkTarget).ConfigureAwait(false) && walkTarget != goal)
        {
            // The waypoint can't be reached, e.g. because of the terrain. The goal might be reachable.
            await monster.WalkToAsync(goal).ConfigureAwait(false);
        }
    }

    private static bool IsChance(int percentage)
    {
        return percentage > 0 && Rand.NextInt(0, 100) < percentage;
    }

    private static async ValueTask StunAsync(Player player, TimeSpan duration)
    {
        if (player.Attributes is not { } attributes
            || player.GameContext.Configuration.MagicEffects.FirstOrDefault(m => m.Number == StunnedMagicEffectNumber) is not { } definition
            || definition.PowerUpDefinitions.FirstOrDefault(p => p.TargetAttribute == Stats.IsStunned) is not { } powerUpDefinition)
        {
            return;
        }

        var powerUp = attributes.CreateElement(powerUpDefinition);
        var effect = new MagicEffect(duration, definition, [new MagicEffect.ElementWithTarget(powerUp, Stats.IsStunned)]);
        await player.MagicEffectList.AddEffectAsync(effect).ConfigureAwait(false);
    }

    private static void Decrease(Player player, AttributeDefinition attribute, int percentage)
    {
        if (player.Attributes is { } attributes)
        {
            attributes[attribute] = attributes[attribute] * (100 - Math.Clamp(percentage, 0, 100)) / 100f;
        }
    }

    private static bool IsValidTarget(Monster monster, IAttackable target)
    {
        return target is Player { IsInvisible: false }
               && target.IsActive()
               && !target.IsAtSafezone()
               && target.IsInRange(monster.Position, monster.Definition.AttackRange)
               && monster.HasLineOfSightTo(target);
    }

    private async ValueTask AttackAsync(Monster monster, IAttackable target)
    {
        if (this.Role == CrywolfMonsterRole.Balgass && await this.TryEscapeAsync(monster, target).ConfigureAwait(false))
        {
            return;
        }

        var definition = this._context.Definition;
        this._skills ??= definition.MonsterSkills.Where(skill => skill.MonsterNumber == monster.Definition.Number).ToList();
        if (this._skills.Count == 0 || !IsChance(definition.SkillChance))
        {
            await monster.AttackAsync(target).ConfigureAwait(false);
            return;
        }

        await this.UseSkillAsync(monster, target, this._skills[Rand.NextInt(0, this._skills.Count)]).ConfigureAwait(false);
    }

    /// <summary>
    /// Lets Balgass try to escape from his target, when his health is low, like in the original game.
    /// </summary>
    private async ValueTask<bool> TryEscapeAsync(Monster monster, IAttackable target)
    {
        var definition = this._context.Definition;
        if (definition.BalgassEscapeHealth <= 0
            || monster.Health >= definition.BalgassEscapeHealth
            || !IsChance(definition.BalgassEscapeChance)
            || monster.Attributes[Stats.IsFrozen] > 0)
        {
            return false;
        }

        var escapeTarget = monster.CurrentMap.Terrain.GetPointAwayFrom(target.Position, monster.Position, definition.BalgassEscapeDistance, this.CanWalkOn);
        return escapeTarget != monster.Position && await monster.WalkToAsync(escapeTarget).ConfigureAwait(false);
    }

    private async ValueTask UseSkillAsync(Monster monster, IAttackable target, CrywolfMonsterSkill skill)
    {
        await monster.ForEachWorldObserverAsync<ICrywolfEventViewPlugIn>(p => p.ShowMonsterSkillAsync(monster, target, skill.SkillNumber), true).ConfigureAwait(false);

        IReadOnlyList<Player> players = skill.Radius > 0
            ? monster.CurrentMap.GetAttackablesInRange(monster.Position, skill.Radius)
                .OfType<Player>()
                .Where(player => player.IsActive() && !player.IsAtSafezone())
                .ToList()
            : target is Player targetPlayer ? [targetPlayer] : [];
        foreach (var player in players)
        {
            await player.AttackByAsync(monster, null, false).ConfigureAwait(false);
            if (!player.IsAlive)
            {
                continue;
            }

            // The push comes first, because a stunned player isn't pushed.
            if (IsChance(skill.PushChance))
            {
                await monster.PushAwayAsync(player, skill.PushDistance).ConfigureAwait(false);
            }

            if (IsChance(skill.StunChance))
            {
                await StunAsync(player, skill.StunDuration).ConfigureAwait(false);
            }

            if (skill.RemovedMagicEffectNumber > 0
                && IsChance(skill.RemoveEffectChance)
                && player.MagicEffectList.TryGetEffect(skill.RemovedMagicEffectNumber, out var effect))
            {
                await effect.DisposeAsync().ConfigureAwait(false);
            }

            if (IsChance(skill.ManaDecreaseChance))
            {
                Decrease(player, Stats.CurrentMana, skill.DecreasePercentage);
            }

            if (IsChance(skill.AbilityDecreaseChance))
            {
                Decrease(player, Stats.CurrentAbility, skill.DecreasePercentage);
            }
        }
    }

    private Point? GetGoal(Monster monster)
    {
        var definition = this._context.Definition;
        switch (this.Role)
        {
            case CrywolfMonsterRole.Balgass:
                return new Point(definition.BalgassGoalX, definition.BalgassGoalY);
            case CrywolfMonsterRole.Leader when this._group is { } group:
                return this._context.ArmyStage == CrywolfArmyStage.AttackingStatue
                    ? new Point(group.AttackGoalX, group.AttackGoalY)
                    : new Point(group.AdvanceGoalX, group.AdvanceGoalY);
            case CrywolfMonsterRole.Soldier when this._group is { } group:
                if (this._context.GetLeader(group.WaveNumber) is { } leader
                    && !leader.IsInRange(monster.Position, definition.FollowLeaderDistance))
                {
                    return leader.Position;
                }

                return null;
            default:
                return null;
        }
    }

    private async ValueTask<IAttackable?> SearchTargetAsync(Monster monster)
    {
        if (this._attacker is { } attacker && IsValidTarget(monster, attacker))
        {
            return attacker;
        }

        this._attacker = null;
        List<IAttackable> candidates;
        using (await monster.ObserverLock.ReaderLockAsync())
        {
            candidates = monster.Observers.OfType<IAttackable>().Where(candidate => IsValidTarget(monster, candidate)).ToList();
        }

        return candidates.MinBy(candidate => candidate.GetDistanceTo(monster));
    }

    private async Task SafeTickAsync()
    {
        if (Interlocked.Exchange(ref this._isRunning, 1) != 0)
        {
            return;
        }

        try
        {
            await this.TickAsync().ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // expected during shutdown.
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error in the crywolf intelligence of {monster}.", this._monster);
        }
        finally
        {
            Interlocked.Exchange(ref this._isRunning, 0);
        }
    }
}
