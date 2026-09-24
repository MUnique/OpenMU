// <copyright file="SelupanIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Raklion;

using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// The intelligence of Selupan, the boss of the raklion event.
/// </summary>
/// <remarks>
/// Selupan uses one of its skills in a fixed interval. Which skills are available, depends on its
/// pattern, which is determined by its remaining health. With each pattern, Selupan also gets
/// stronger (berserk) and uses more of its skills.
/// </remarks>
public sealed class SelupanIntelligence : INpcIntelligence, IDisposable
{
    private const short StunnedMagicEffectNumber = 61;
    private const int TeleportTries = 10;

    private readonly RaklionContext _context;
    private readonly RaklionEventDefinition _definition;
    private readonly ILogger _logger;
    private readonly List<(IElement Element, AttributeDefinition Target)> _berserkElements = new();
    private readonly SimpleElement _invincibilityElement = new(0, AggregateType.Multiplicate);
    private Timer? _timer;
    private Monster? _monster;
    private IAttackable? _attacker;
    private int _pattern;
    private int _isRunning;
    private DateTime _invincibleUntil = DateTime.MinValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelupanIntelligence"/> class.
    /// </summary>
    /// <param name="context">The context of the raklion event.</param>
    /// <param name="definition">The definition of the raklion event.</param>
    /// <param name="logger">The logger.</param>
    public SelupanIntelligence(RaklionContext context, RaklionEventDefinition definition, ILogger logger)
    {
        this._context = context;
        this._definition = definition;
        this._logger = logger;
    }

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

    /// <summary>
    /// Gets the current pattern (1 to 7) of Selupan, or 0 if it didn't start yet.
    /// </summary>
    public int Pattern => this._pattern;

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
        this._timer ??= new Timer(state => _ = this.SafeTickAsync(), null, this._definition.SelupanSkillDelay, this._definition.SelupanSkillDelay);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Selupan keeps its state when no player observes it, so pausing is ignored.
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
    /// Executes one step of the intelligence. It's called periodically by a timer.
    /// </summary>
    internal async ValueTask TickAsync()
    {
        if (this._monster is not { IsAlive: true } monster)
        {
            return;
        }

        if (this._pattern == 0)
        {
            // Selupan falls from the sky when it appears.
            await this.ShowSkillAsync(monster, null, SelupanSkill.Fall).ConfigureAwait(false);
            await this.UpdatePatternAsync(monster, 1).ConfigureAwait(false);
            return;
        }

        var pattern = this._definition.GetPattern(GetHealthPercentage(monster));
        if (pattern != this._pattern)
        {
            await this.UpdatePatternAsync(monster, pattern).ConfigureAwait(false);
        }

        this.UpdateInvincibility(monster);
        if (monster.Attributes[Stats.IsStunned] > 0
            || monster.Attributes[Stats.IsAsleep] > 0
            || monster.Attributes[Stats.IsFrozen] > 0)
        {
            return;
        }

        if (await this.SearchTargetAsync(monster).ConfigureAwait(false) is not { } target)
        {
            return;
        }

        var skill = this.ChooseSkill(monster);
        await this.ExecuteSkillAsync(monster, target, skill).ConfigureAwait(false);
    }

    private static double GetHealthPercentage(Monster monster)
    {
        var maximumHealth = monster.Attributes[Stats.MaximumHealth];
        return maximumHealth > 0 ? monster.Health * 100.0 / maximumHealth : 0;
    }

    private static int GetWeight(SelupanSkill skill)
    {
        return skill switch
        {
            SelupanSkill.Poison or SelupanSkill.IceStorm or SelupanSkill.IceStrike => 4,
            SelupanSkill.Teleport => 1,
            _ => 2,
        };
    }

    private static bool IsValidTarget(Monster monster, IAttackable target)
    {
        return target.IsActive()
               && target is not Player { IsInvisible: true }
               && !target.IsAtSafezone()
               && target.IsInRange(monster.Position, monster.Definition.AttackRange);
    }

    private static async ValueTask FreezeAsync(Player target, TimeSpan duration)
    {
        if (target.Attributes is not { } attributes
            || target.GameContext.Configuration.MagicEffects.FirstOrDefault(m => m.Number == StunnedMagicEffectNumber) is not { } effectDefinition
            || effectDefinition.PowerUpDefinitions.FirstOrDefault(pu => pu.TargetAttribute == Stats.IsStunned) is not { } powerUpDefinition)
        {
            return;
        }

        var powerUp = attributes.CreateElement(powerUpDefinition);
        var magicEffect = new MagicEffect(duration, effectDefinition, [new MagicEffect.ElementWithTarget(powerUp, Stats.IsStunned)]);
        await target.MagicEffectList.AddEffectAsync(magicEffect).ConfigureAwait(false);
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
            this._logger.LogError(ex, "Unexpected error in the intelligence of {monster}.", this._monster);
        }
        finally
        {
            Interlocked.Exchange(ref this._isRunning, 0);
        }
    }

    private async ValueTask UpdatePatternAsync(Monster monster, int pattern)
    {
        this._pattern = pattern;
        foreach (var (element, target) in this._berserkElements)
        {
            monster.Attributes.RemoveElement(element, target);
        }

        this._berserkElements.Clear();
        var additionalDamage = this._definition.GetBerserkLevel(pattern) * this._definition.BerserkDamagePerLevel;
        if (additionalDamage > 0)
        {
            foreach (var target in new[] { Stats.MinimumPhysBaseDmg, Stats.MaximumPhysBaseDmg })
            {
                var element = new SimpleElement(additionalDamage, AggregateType.AddRaw);
                monster.Attributes.AddElement(element, target);
                this._berserkElements.Add((element, target));
            }
        }

        await this._context.ChangeSelupanStateAsync((SelupanState)(pattern + (int)SelupanState.Standby)).ConfigureAwait(false);
    }

    private void UpdateInvincibility(Monster monster)
    {
        if (this._invincibleUntil != DateTime.MinValue && DateTime.UtcNow >= this._invincibleUntil)
        {
            this._invincibleUntil = DateTime.MinValue;
            monster.Attributes.RemoveElement(this._invincibilityElement, Stats.DamageReceiveDecrement);
        }
    }

    private SelupanSkill ChooseSkill(Monster monster)
    {
        var skills = this._definition.GetSkills(this._pattern)
            .Where(skill => skill switch
            {
                SelupanSkill.Heal => monster.Health < monster.Attributes[Stats.MaximumHealth],
                SelupanSkill.Summon => this._context.CanSummon,
                SelupanSkill.Invincibility => this._invincibleUntil == DateTime.MinValue,
                _ => true,
            })
            .ToList();

        // The attacks are used more often than the other skills, the teleport is used rarely.
        var totalWeight = skills.Sum(GetWeight);
        var value = Rand.NextInt(0, totalWeight);
        foreach (var skill in skills)
        {
            value -= GetWeight(skill);
            if (value < 0)
            {
                return skill;
            }
        }

        return skills[^1];
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

    private async ValueTask ExecuteSkillAsync(Monster monster, IAttackable target, SelupanSkill skill)
    {
        switch (skill)
        {
            case SelupanSkill.Poison:
            case SelupanSkill.IceStorm:
            case SelupanSkill.IceStrike:
                await this.ShowSkillAsync(monster, target, skill).ConfigureAwait(false);
                await this.AttackAreaAsync(monster, target).ConfigureAwait(false);
                break;
            case SelupanSkill.Freeze:
                await this.ShowSkillAsync(monster, target, skill).ConfigureAwait(false);
                await target.AttackByAsync(monster, null, false).ConfigureAwait(false);
                if (target is Player player && player.IsAlive)
                {
                    await FreezeAsync(player, this._definition.FreezeDuration).ConfigureAwait(false);
                }

                break;
            case SelupanSkill.Heal:
                await this.ShowSkillAsync(monster, null, skill).ConfigureAwait(false);
                var maximumHealth = (int)monster.Attributes[Stats.MaximumHealth];
                monster.Health = Math.Min(maximumHealth, monster.Health + (monster.Health * this._definition.HealPercentage / 100));
                break;
            case SelupanSkill.Summon:
                await this.ShowSkillAsync(monster, null, skill).ConfigureAwait(false);
                await this._context.SummonMonstersAsync().ConfigureAwait(false);
                break;
            case SelupanSkill.Invincibility:
                await this.ShowSkillAsync(monster, null, skill).ConfigureAwait(false);
                this._invincibleUntil = DateTime.UtcNow + this._definition.InvincibilityDuration;
                monster.Attributes.AddElement(this._invincibilityElement, Stats.DamageReceiveDecrement);
                break;
            case SelupanSkill.Teleport:
                await this.TeleportAsync(monster).ConfigureAwait(false);
                break;
            default:
                await monster.AttackAsync(target).ConfigureAwait(false);
                break;
        }
    }

    private async ValueTask AttackAreaAsync(Monster monster, IAttackable target)
    {
        var targets = monster.CurrentMap.GetAttackablesInRange(target.Position, this._definition.AreaSkillRadius)
            .OfType<Player>()
            .Where(player => IsValidTarget(monster, player) || player == target)
            .ToList();
        foreach (var player in targets)
        {
            await player.AttackByAsync(monster, null, false).ConfigureAwait(false);
        }
    }

    private async ValueTask TeleportAsync(Monster monster)
    {
        for (var i = 0; i < TeleportTries; i++)
        {
            var distance = Rand.NextInt(this._definition.TeleportMinimumDistance, this._definition.TeleportMaximumDistance + 1);
            var x = monster.Position.X + Rand.NextInt(-distance, distance + 1);
            var y = monster.Position.Y + Rand.NextInt(-distance, distance + 1);
            if (x is < byte.MinValue or > byte.MaxValue || y is < byte.MinValue or > byte.MaxValue)
            {
                continue;
            }

            var target = new Point((byte)x, (byte)y);
            if (this.CanWalkOn(target) && !monster.CurrentMap.Terrain.SafezoneMap[target.X, target.Y])
            {
                await this.ShowSkillAsync(monster, null, SelupanSkill.Teleport).ConfigureAwait(false);
                await monster.TeleportAsync(target).ConfigureAwait(false);
                return;
            }
        }
    }

    private ValueTask ShowSkillAsync(Monster monster, IAttackable? target, SelupanSkill skill)
    {
        return monster.ForEachWorldObserverAsync<IRaklionEventViewPlugIn>(p => p.ShowSelupanSkillAsync(monster, target, skill), true);
    }
}
