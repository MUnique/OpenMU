// <copyright file="SkillHitValidator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.AnimationCounters;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Validator for skill hits, which were done by skills with type <see cref="SkillType.AreaSkillExplicitHits"/>.
/// It checks the animation sequence, replayed hit counters, and that the target is inside the area of the preceding attack.
/// </summary>
public class SkillHitValidator
{
    private const int MinimumCounterValue = 1;
    private const int MaximumCounterValue = 0x32;

    private static readonly TimeSpan MaxAnimationToHitDelay = TimeSpan.FromSeconds(10);

    private readonly ILogger _logger;

    private readonly PlugInManager? _plugInManager;

    private readonly TimeProvider _timeProvider;

    private readonly SpeedHackDetectConfiguration _configuration;

    private readonly Dictionary<(ushort SkillId, byte AnimationCounter), HitEntry> _hits = new();

    private readonly Dictionary<(ushort SkillId, byte AnimationCounter), AreaEntry> _areas = new();

    private readonly Dictionary<(ushort SkillId, byte AnimationCounter), HitCounterEntry> _hitCounters = new();

    /// <summary>
    /// The counters of the last animations per skill.
    /// </summary>
    private readonly Dictionary<ushort, byte> _lastAnimationCounters = new();

    /// <summary>
    /// The game client doesn't reset its counter when it connects with a new account.
    /// So, the simplest way for us is, to accept the first counter we get with
    /// an animation.
    /// </summary>
    private bool _isFirstAfterConnectionEstablished = true;

    /// <summary>
    /// The expected count of the next animation and hit.
    /// </summary>
    private int _expectedCount = MinimumCounterValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillHitValidator"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="plugInManager">The plug in manager, which provides the <see cref="ISkillAnimationCounterStrategy"/>s.</param>
    /// <param name="timeProvider">The time provider.</param>
    /// <param name="configuration">The anti-cheat configuration which provides the hit validation thresholds.</param>
    public SkillHitValidator(
        ILogger logger,
        PlugInManager? plugInManager = null,
        TimeProvider? timeProvider = null,
        SpeedHackDetectConfiguration? configuration = null)
    {
        this._logger = logger;
        this._plugInManager = plugInManager;
        this._timeProvider = timeProvider ?? TimeProvider.System;
        this._configuration = configuration ?? new SpeedHackDetectConfiguration();
    }

    /// <summary>
    /// Gets the last registered skill identifier.
    /// </summary>
    public ushort LastRegisteredSkillId { get; private set; }

    /// <summary>
    /// Tries to register an animation with the specified counter.
    /// </summary>
    /// <param name="skillId">The skill identifier.</param>
    /// <param name="animationCounter">The animation counter.</param>
    /// <returns>
    /// True, if successful.
    /// </returns>
    /// <remarks>Basically, only skills with overlapping animations and hits send an animation counter greater than 0.</remarks>
    public bool TryRegisterAnimation(ushort skillId, byte animationCounter)
    {
        if (animationCounter > MaximumCounterValue)
        {
            this._logger.LogWarning(
                "Possible Hacker - Skill Animation Invalid because of an out of range animation counter (skill {0}, animation {1}).",
                skillId,
                animationCounter);
            return false;
        }

        this._lastAnimationCounters[skillId] = animationCounter;
        if (this.GetStrategy(skillId)?.IsAnimationCounted(animationCounter) ?? animationCounter > 0)
        {
            if (this._isFirstAfterConnectionEstablished)
            {
                this._expectedCount = animationCounter;
                this._isFirstAfterConnectionEstablished = false;
            }

            if (this._expectedCount != animationCounter)
            {
                this._logger.LogWarning($"Animation count out of sync - hacker? Expected: {this._expectedCount}, Actual: {animationCounter}.");
                return false;
            }

            this.IncreaseExpectedCount();
        }

        this.LastRegisteredSkillId = skillId;
        this._hits[(skillId, animationCounter)] = new HitEntry(this._timeProvider.GetTimestamp(), true, 0);
        return true;
    }

    /// <summary>
    /// Increases the counter by one, after a hit was done.
    /// It must just be called once after a combined hit.
    /// </summary>
    public void IncreaseCounterAfterHit()
    {
        this.IncreaseExpectedCount();
    }

    /// <summary>
    /// Determines whether a hit counter is valid, i.e. it was not just used for the same animation.
    /// </summary>
    /// <param name="skillId">The skill identifier.</param>
    /// <param name="animationCounter">The animation counter of the hit.</param>
    /// <param name="hitCounter">The hit counter.</param>
    /// <returns><c>true</c> if the hit counter is not a replay; otherwise, <c>false</c>.</returns>
    public bool IsHitCounterValid(ushort skillId, byte animationCounter, byte hitCounter)
    {
        animationCounter = this.GetAnimationCounterOfHit(skillId, animationCounter);
        if (animationCounter > MaximumCounterValue)
        {
            this._logger.LogWarning(
                "Possible Hacker - Skill Hit Invalid because of an out of range animation counter (skill {0}, animation {1}, hit {2}).",
                skillId,
                animationCounter,
                hitCounter);
            return false;
        }

        var now = this._timeProvider.GetTimestamp();
        var replayWindow = TimeSpan.FromMilliseconds(Math.Max(0, this._configuration.SkillHitCounterReplayWindowMs));
        var key = (skillId, animationCounter);
        if (replayWindow > TimeSpan.Zero
            && this._hitCounters.TryGetValue(key, out var entry)
            && entry.Counter == hitCounter
            && this._timeProvider.GetElapsedTime(entry.TimeStamp, now) < replayWindow)
        {
            this._logger.LogWarning(
                "Possible Hacker - Skill Hit Invalid because of a repeated hit counter (skill {0}, animation {1}, hit {2}).",
                skillId,
                animationCounter,
                hitCounter);
            return false;
        }

        this._hitCounters[key] = new HitCounterEntry(hitCounter, now);
        return true;
    }

    /// <summary>
    /// Determines whether a hit request is valid for the specified skill identifier.
    /// </summary>
    /// <param name="skillId">The skill identifier.</param>
    /// <param name="animationCounter">The animation counter.</param>
    /// <param name="hitCounter">The hit counter.</param>
    /// <returns>
    ///   <c>true</c> if a hit request is valid for the specified skill identifier; otherwise, <c>false</c>.
    /// </returns>
    public (bool IsValid, bool IncreaseCounter) IsHitValid(ushort skillId, byte animationCounter, byte hitCounter)
    {
        animationCounter = this.GetAnimationCounterOfHit(skillId, animationCounter);
        if (animationCounter > MaximumCounterValue)
        {
            this._logger.LogWarning(
                "Possible Hacker - Skill Hit Invalid because of an out of range animation counter (skill {0}, animation {1}, hit {2}).",
                skillId,
                animationCounter,
                hitCounter);
            return (false, false);
        }

        var key = (skillId, animationCounter);
        if (!this._hits.TryGetValue(key, out var animationEntry))
        {
            this._logger.LogWarning(
                "Possible Hacker - Skill Hit Invalid because of missing previous animation (skill {0}, animation {1}, hit {2}).",
                skillId,
                animationCounter,
                hitCounter);
            return (false, false);
        }

        if (!animationEntry.IsAnimation)
        {
            this._logger.LogWarning("Possible Hacker - Skill Hit Invalid because the given animation counter wasn't registered as animation.");
            return (false, false);
        }

        var timestampDiff = this._timeProvider.GetElapsedTime(animationEntry.TimeStamp, this._timeProvider.GetTimestamp());
        if (timestampDiff > MaxAnimationToHitDelay)
        {
            this._logger.LogWarning("Possible Hacker - Skill Hit Invalid because of too high time difference between animation and hit.");
            return (false, true);
        }

        this._hits[key] = animationEntry with { HitCount = animationEntry.HitCount + 1 };
        return (true, true);
    }

    /// <summary>
    /// Registers the target area of an area skill attack, so that its explicit hits can be checked against it.
    /// </summary>
    /// <param name="skillId">The skill identifier.</param>
    /// <param name="skill">The skill.</param>
    /// <param name="casterPosition">The position of the caster when the attack started.</param>
    /// <param name="targetAreaCenter">The target area center requested by the client.</param>
    public void RegisterArea(ushort skillId, Skill skill, Point casterPosition, Point targetAreaCenter)
    {
        var (center, range) = AreaSkillTargetSelector.GetQueryCenterAndRange(casterPosition, targetAreaCenter, skill);
        var timestamp = this._timeProvider.GetTimestamp();
        var maxDistance = AreaSkillTargetSelector.GetTargetAreaRadius(skill) is { } targetAreaRadius
            ? targetAreaRadius
            : range;
        var animationCounter = this._lastAnimationCounters.GetValueOrDefault(skillId);
        var rangeSlack = Math.Max(0, this._configuration.SkillHitAreaRangeSlack);
        this._areas[(skillId, animationCounter)] = new AreaEntry(center, maxDistance + rangeSlack, timestamp);
    }

    /// <summary>
    /// Determines whether the target of an explicit area skill hit is inside the area of the matching attack.
    /// </summary>
    /// <param name="skillId">The skill identifier.</param>
    /// <param name="targetPosition">The target position.</param>
    /// <param name="animationCounter">The animation counter of the hit, or zero if the client doesn't send one.</param>
    /// <returns><c>true</c> if the hit is plausible; otherwise, <c>false</c>.</returns>
    public bool IsTargetInArea(ushort skillId, Point targetPosition, byte animationCounter = 0)
    {
        var key = (skillId, this.GetAnimationCounterOfHit(skillId, animationCounter));
        if (!this._areas.TryGetValue(key, out var area))
        {
            this._logger.LogWarning("Possible Hacker - Skill Hit Invalid because of missing previous area skill attack.");
            return false;
        }

        var maxDelay = TimeSpan.FromSeconds(Math.Max(0, this._configuration.SkillHitMaxAreaToHitDelaySeconds));
        if (maxDelay > TimeSpan.Zero
            && this._timeProvider.GetElapsedTime(area.TimeStamp, this._timeProvider.GetTimestamp()) > maxDelay)
        {
            this._areas.Remove(key);
            this._logger.LogWarning("Possible Hacker - Skill Hit Invalid because of too high time difference between area skill attack and hit.");
            return false;
        }

        double distance = area.Center.EuclideanDistanceTo(targetPosition);
        if (distance <= area.MaxDistance)
        {
            return true;
        }

        this._logger.LogWarning(
            "Possible Hacker - Skill Hit Invalid because the target is too far away from the area center (distance {0:F1}, allowed {1}).",
            distance,
            area.MaxDistance);
        return false;
    }

    private ISkillAnimationCounterStrategy? GetStrategy(ushort skillId)
    {
        return this._plugInManager?.GetStrategy<short, ISkillAnimationCounterStrategy>((short)skillId);
    }

    private byte GetAnimationCounterOfHit(ushort skillId, byte animationCounter)
    {
        return this.GetStrategy(skillId) is { } strategy
            ? strategy.GetAnimationCounterOfHit(animationCounter, this._lastAnimationCounters.GetValueOrDefault(skillId))
            : animationCounter;
    }

    private void IncreaseExpectedCount()
    {
        this._expectedCount = this._expectedCount == MaximumCounterValue ? MinimumCounterValue : this._expectedCount + 1;
    }

    private readonly record struct HitEntry(long TimeStamp, bool IsAnimation, int HitCount);

    private readonly record struct AreaEntry(Point Center, double MaxDistance, long TimeStamp);

    private readonly record struct HitCounterEntry(byte Counter, long TimeStamp);
}
