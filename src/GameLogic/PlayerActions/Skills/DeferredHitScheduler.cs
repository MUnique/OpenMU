// <copyright file="DeferredHitScheduler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Diagnostics.CodeAnalysis;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Executes the hits of one area skill attack over time. Immediate hits are applied
/// right away; projectile hits are applied when they arrive at the target, and
/// wavefront ticks catch targets which moved into the skill area after the attack started.
/// </summary>
public sealed class DeferredHitScheduler
{
    /// <summary>
    /// Extra tiles tolerated when a deferred hit lands. The target may have
    /// moved while the projectile was flying; teleports still exceed it.
    /// </summary>
    private const int DeferredLandingSlack = 2;

    private readonly Player _player;
    private readonly SkillEntry _skillEntry;
    private readonly Skill _skill;
    private readonly AreaSkillSettings _settings;
    private readonly Point _targetAreaCenter;
    private readonly byte _rotation;
    private readonly ushort _extraTargetId;
    private readonly Func<IAttackable, ValueTask> _applyHitAsync;
    private readonly SkillCasterOrigin _origin;
    private readonly FrustumBasedTargetFilter? _laneFilter;
    private readonly int _extraProjectiles;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeferredHitScheduler"/> class.
    /// </summary>
    /// <param name="player">The player who performed the skill.</param>
    /// <param name="skillEntry">The used skill.</param>
    /// <param name="skill">The skill.</param>
    /// <param name="settings">The area skill settings.</param>
    /// <param name="targetAreaCenter">The coordinates of the center of the target area.</param>
    /// <param name="rotation">The rotation of the attack.</param>
    /// <param name="extraTargetId">The extra target identifier.</param>
    /// <param name="extraProjectiles">The number of extra projectiles from the attacker's attributes.</param>
    /// <param name="applyHitAsync">Applies a hit to a target.</param>
    public DeferredHitScheduler(Player player, SkillEntry skillEntry, Skill skill, AreaSkillSettings settings, Point targetAreaCenter, byte rotation, ushort extraTargetId, int extraProjectiles, Func<IAttackable, ValueTask> applyHitAsync)
    {
        this._player = player;
        this._skillEntry = skillEntry;
        this._skill = skill;
        this._settings = settings;
        this._targetAreaCenter = targetAreaCenter;
        this._rotation = rotation;
        this._extraTargetId = extraTargetId;
        this._extraProjectiles = extraProjectiles;
        this._applyHitAsync = applyHitAsync;
        this._origin = new SkillCasterOrigin(player.Position, player.CurrentMap);
        this._laneFilter = settings is { UseFrustumFilter: true } ? AreaSkillTargetSelector.GetLaneFilter(settings) : null;
    }

    /// <summary>
    /// Delivers a scheduled hit: immediately when no delay applies, otherwise
    /// when the projectile arrives.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="attackDelay">The delay until the hit lands.</param>
    /// <param name="projectileIndex">The index of the projectile.</param>
    public async ValueTask DeliverHitAsync(IAttackable target, TimeSpan attackDelay, int projectileIndex)
    {
        if (attackDelay == TimeSpan.Zero)
        {
            // Check if target is still alive and in valid state before attacking.
            if (!target.IsAtSafezone() && target.IsActive())
            {
                await this._applyHitAsync(target).ConfigureAwait(false);
            }
        }
        else
        {
            this.ScheduleHit(target, attackDelay, projectileIndex);
        }
    }

    /// <summary>
    /// Schedules wavefront ticks which hit newcomers at the distance ring the
    /// projectile reaches at that moment, so nothing behind the wavefront is hit late.
    /// </summary>
    /// <param name="excludedIds">Identifiers of targets already evaluated at cast time, whether their roll succeeded or not.</param>
    /// <param name="attackRound">The attack round the ticks belong to.</param>
    /// <param name="remainingAttacks">Maximum additional hits of this round.</param>
    /// <param name="roundDelay">Delay of the round start.</param>
    public void ScheduleWavefrontSweeps(HashSet<ushort> excludedIds, int attackRound, int remainingAttacks, TimeSpan roundDelay)
    {
        if (remainingAttacks <= 0)
        {
            return;
        }

        var ticks = this.CollectWavefrontTicks(excludedIds, attackRound, remainingAttacks, roundDelay);
        if (ticks.Count == 0)
        {
            return;
        }

        _ = Task.Run(() => this.RunWavefrontSweepAsync(ticks, remainingAttacks));
    }

    /// <summary>
    /// Determines the number of extra projectiles of the given skill from its
    /// <see cref="Stats.SkillExtraProjectiles"/> attribute, e.g. from a bow or from the
    /// passive power-up of the Triple Shot Mastery which is applied to the player.
    /// </summary>
    /// <param name="player">The attacking player.</param>
    /// <param name="skillEntry">The cast skill.</param>
    /// <param name="settings">The area skill settings.</param>
    /// <returns>The number of extra projectiles.</returns>
    internal static int GetExtraProjectiles(Player player, SkillEntry skillEntry, AreaSkillSettings settings)
    {
        if (settings is not { UseFrustumFilter: true, ProjectileCount: > 1 })
        {
            return 0;
        }

        return skillEntry.EnsureSkillAttributes(player.Attributes!) is { } skillAttributes
            ? (int)skillAttributes[Stats.SkillExtraProjectiles]
            : 0;
    }

    private List<WavefrontTick> CollectWavefrontTicks(HashSet<ushort> excludedIds, int attackRound, int remainingAttacks, TimeSpan roundDelay)
    {
        var reach = AreaSkillTargetSelector.GetQueryCenterAndRange(this._origin.Position, this._targetAreaCenter, this._skill).Range;
        var perDistance = this._settings.DelayPerOneDistance;
        var steps = perDistance > TimeSpan.Zero ? Math.Max(reach, 1) : 1;
        var state = new SweepState();
        var ticks = new List<WavefrontTick>();
        for (var step = 1; step <= steps; step++)
        {
            var minDistance = perDistance > TimeSpan.Zero ? step - 1.0 : 0.0;
            var maxDistance = perDistance > TimeSpan.Zero ? (double)step : double.MaxValue;
            var tickDelay = perDistance > TimeSpan.Zero ? roundDelay + (perDistance * step) : roundDelay;
            if (tickDelay == TimeSpan.Zero)
            {
                // No time passed since the snapshot; newcomers cannot exist yet.
                continue;
            }

            ticks.Add(new WavefrontTick(excludedIds, state, attackRound, remainingAttacks, minDistance, maxDistance, tickDelay));
        }

        return ticks;
    }

    private async Task RunWavefrontSweepAsync(List<WavefrontTick> ticks, int remainingAttacks)
    {
        try
        {
            // One task walks all rings of the round instead of one task per ring.
            var previousDelay = TimeSpan.Zero;
            foreach (var tick in ticks)
            {
                if (tick.State.IsExhausted(remainingAttacks))
                {
                    break;
                }

                await Task.Delay(tick.Delay - previousDelay).ConfigureAwait(false);
                previousDelay = tick.Delay;
                await this.ApplyWavefrontTickAsync(tick).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this._player.Logger.LogError(ex, "Error during area skill wavefront sweep.");
        }
    }

    private void ScheduleHit(IAttackable target, TimeSpan delay, int projectileIndex)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(delay).ConfigureAwait(false);
                await this.ApplyDeferredHitAsync(target, projectileIndex).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._player.Logger.LogError(ex, "Error during deferred area skill hit.");
            }
        });
    }

    private async ValueTask ApplyDeferredHitAsync(IAttackable target, int projectileIndex)
    {
        if (!this.CanExecute(out var skill))
        {
            return;
        }

        if (!this.IsTargetValid(target, skill, projectileIndex))
        {
            return;
        }

        await this._applyHitAsync(target).ConfigureAwait(false);
    }

    private bool IsTargetValid(IAttackable target, Skill skill, int projectileIndex)
    {
        if (target.CurrentMap is null || target.CurrentMap != this._player.CurrentMap)
        {
            this.LogDrop(skill, target, "different map");
            return false;
        }

        if (!target.IsAlive || target.IsAtSafezone() || !target.IsActive())
        {
            this.LogDrop(skill, target, "inactive");
            return false;
        }

        if (!target.CheckSkillTargetRestrictions(this._player, skill))
        {
            this.LogDrop(skill, target, "target restriction");
            return false;
        }

        var range = this._settings.EffectRange > 0 ? this._settings.EffectRange : skill.Range;
        if (target.Id == this._extraTargetId)
        {
            // Like the check at cast time (Chebyshev metric), plus landing slack.
            if (!this._origin.IsInRange(target, range + DeferredLandingSlack))
            {
                this.LogDrop(skill, target, "extra target out of range");
                return false;
            }
        }
        else if (this._settings.UseTargetAreaFilter)
        {
            // Mirrors the check at cast time: strictly inside the target area.
            if (target.GetDistanceTo(this._targetAreaCenter) >= this._settings.TargetAreaDiameter * 0.5f)
            {
                this.LogDrop(skill, target, "left target area");
                return false;
            }
        }
        else if (this._laneFilter is { } laneFilter)
        {
            // The direction is re-checked here with the corrected rotation, so
            // targets behind or beside the caster are never hit. Position drift
            // during the projectile flight can still drop a strafing target,
            // but that is geometrically honest: it left the projectile lane.
            // For a single projectile this falls back to the plain lane check.
            if (!laneFilter.IsTargetWithinBounds(this._origin, target, this._rotation, projectileIndex, this._extraProjectiles))
            {
                this.LogDrop(skill, target, "left projectile lane");
                return false;
            }
        }
        else
        {
            // Unshaped areas are centered on the caster, like the query at cast time.
            if (!target.IsInRange(this._origin.Position, range))
            {
                this.LogDrop(skill, target, "out of range");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Determines whether a deferred hit may still be executed. The skill must be
    /// known, the player alive, and still on the map where the attack started.
    /// </summary>
    /// <param name="skill">The skill, if available.</param>
    /// <returns><c>true</c> if the deferred hit may be executed.</returns>
    private bool CanExecute([NotNullWhen(true)] out Skill? skill)
    {
        skill = this._skillEntry.Skill;
        if (skill is null || !this._player.IsAlive)
        {
            return false;
        }

        var casterMap = this._origin.CurrentMap;
        return casterMap is not null && this._player.CurrentMap is not null && this._player.CurrentMap == casterMap;
    }

    private void LogDrop(Skill skill, IAttackable target, string reason)
    {
        this._player.Logger.LogDebug("Deferred hit dropped: skill={Skill}, target={TargetId}, reason={Reason}", skill.Name, target.Id, reason);
    }

    private async ValueTask ApplyWavefrontTickAsync(WavefrontTick tick)
    {
        if (!this.CanExecute(out var skill))
        {
            return;
        }

        if (tick.State.IsExhausted(tick.RemainingAttacks))
        {
            return;
        }

        // Same query center and radius as at cast time. The guard above ensures
        // the player is still on the cast map.
        var (queryCenter, queryRange) = AreaSkillTargetSelector.GetQueryCenterAndRange(this._origin.Position, this._targetAreaCenter, skill);
        if (this._origin.CurrentMap is not { } map)
        {
            return;
        }

        var candidates = AreaSkillTargetSelector.WhereAttackableByDefault(
            map.GetAttackablesInRange(queryCenter, queryRange),
            this._player,
            skill);

        foreach (var target in candidates)
        {
            if (tick.ExcludedIds.Contains(target.Id) || !target.IsAlive)
            {
                continue;
            }

            // Only the ring the wavefront reaches right now. Anything closer was
            // already passed, anything farther is not reached yet.
            var distance = this._origin.Position.EuclideanDistanceTo(target.Position);
            if (distance <= tick.MinDistance || distance > tick.MaxDistance)
            {
                continue;
            }

            // Sweeps only run for single-projectile skills, so the projectile
            // index is inert here.
            if (!this.IsTargetValid(target, skill, 0))
            {
                continue;
            }

            double hitChance = tick.AttackRound >= this._settings.MinimumNumberOfHitsPerTarget
                ? Math.Pow(this._settings.HitChancePerDistanceMultiplier, distance)
                : 1.0;
            if (hitChance < 1.0 && !Rand.NextRandomBool(hitChance))
            {
                continue;
            }

            if (!tick.State.TryClaim(target.Id, tick.RemainingAttacks))
            {
                continue;
            }

            await this._applyHitAsync(target).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// The captured origin of an area skill attack. Deferred hits validate their
    /// targets against this position instead of the attacker's current position,
    /// which may have changed while the projectile was flying.
    /// </summary>
    private sealed class SkillCasterOrigin : ILocateable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkillCasterOrigin"/> class.
        /// </summary>
        /// <param name="position">The position of the caster when the attack started.</param>
        /// <param name="map">The map of the caster when the attack started.</param>
        public SkillCasterOrigin(Point position, GameMap? map)
        {
            this.Position = position;
            this.CurrentMap = map;
        }

        /// <inheritdoc />
        public ushort Id => 0;

        /// <inheritdoc />
        public GameMap? CurrentMap { get; }

        /// <inheritdoc />
        public Point Position { get; set; }
    }

    /// <summary>
    /// Shared state of the wavefront ticks of one attack round.
    /// </summary>
    private sealed class SweepState
    {
        private readonly object _syncRoot = new();
        private readonly HashSet<ushort> _hitIds = new();
        private int _hits;

        /// <summary>
        /// Determines whether the round already reached its hit cap.
        /// </summary>
        /// <param name="remainingAttacks">Maximum additional hits of this round.</param>
        /// <returns><c>true</c> if no further hit may be claimed.</returns>
        public bool IsExhausted(int remainingAttacks)
        {
            lock (this._syncRoot)
            {
                return this._hits >= remainingAttacks;
            }
        }

        /// <summary>
        /// Tries to claim one hit for the specified target.
        /// </summary>
        /// <param name="targetId">The target identifier.</param>
        /// <param name="remainingAttacks">Maximum additional hits of this round.</param>
        /// <returns><c>true</c> if the hit was claimed.</returns>
        public bool TryClaim(ushort targetId, int remainingAttacks)
        {
            lock (this._syncRoot)
            {
                if (this._hits >= remainingAttacks || !this._hitIds.Add(targetId))
                {
                    return false;
                }

                this._hits++;
                return true;
            }
        }
    }

    /// <summary>
    /// One wavefront tick: hits newcomers at the distance ring the projectile
    /// reaches after <see cref="Delay"/>.
    /// </summary>
    /// <param name="ExcludedIds">Identifiers of targets already evaluated at cast time; the sweep only hits newcomers.</param>
    /// <param name="State">Shared state of the ticks of one attack round.</param>
    /// <param name="AttackRound">The attack round the tick belongs to.</param>
    /// <param name="RemainingAttacks">Maximum additional hits of this round.</param>
    /// <param name="MinDistance">Exclusive minimum distance from the cast origin.</param>
    /// <param name="MaxDistance">Inclusive maximum distance from the cast origin.</param>
    /// <param name="Delay">Delay until the wavefront reaches the ring.</param>
    private sealed record WavefrontTick(
        HashSet<ushort> ExcludedIds,
        SweepState State,
        int AttackRound,
        int RemainingAttacks,
        double MinDistance,
        double MaxDistance,
        TimeSpan Delay);
}
