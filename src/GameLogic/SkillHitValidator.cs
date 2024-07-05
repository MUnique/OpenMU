// <copyright file="SkillHitValidator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.Network;

/// <summary>
/// Validator for skill hits, which were done by skills with type <see cref="SkillType.AreaSkillExplicitHits"/>.
/// </summary>
public class SkillHitValidator
{
    private const int MaximumCounterValue = 0x32;

    private const byte TwisterSkillId = 8;
    private const byte EvilSpiritSkillId = 9;
    private const byte MultishotSkillId = 235;

    private static readonly TimeSpan MaxAnimationToHitDelay = TimeSpan.FromSeconds(10);

    private readonly ILogger _logger;

    /// <summary>
    /// The counter which keeps the expected count of the next animation and hit.
    /// </summary>
    private readonly Counter _counter = new(1, MaximumCounterValue);

    private readonly HitEntry[] _hits = new HitEntry[MaximumCounterValue + 1];

    private byte _lastTwisterIndex;

    private byte _lastMultishotIndex;

    /// <summary>
    /// The game client doesn't reset its counter when it connects with a new account.
    /// So, the simplest way for us is, to accept the first counter we get with
    /// an animation.
    /// </summary>
    private bool _isFirstAfterConnectionEstablished = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillHitValidator"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public SkillHitValidator(ILogger logger)
    {
        this._logger = logger;
        this._counter.Reset();
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
        if (skillId == TwisterSkillId)
        {
            // Twister is a implemented wrong at the client side. It sends a counter of the animations here, but not in the hit packets.
            this._lastTwisterIndex = animationCounter;
        }

        if (skillId == MultishotSkillId)
        {
            // Multishot is a implemented wrong at the client side. It sends a counter of the animations here, but not in the hit packets.
            this._lastMultishotIndex = animationCounter;
        }

        if (skillId == EvilSpiritSkillId
            || skillId == TwisterSkillId
            || animationCounter > 0)
        {
            if (this._isFirstAfterConnectionEstablished)
            {
                this._counter.Count = animationCounter;
                this._isFirstAfterConnectionEstablished = false;
            }

            if (this._counter.Count != animationCounter)
            {
                this._logger.LogWarning($"Animation count out of sync - hacker? Expected: {this._counter.Count}, Actual: {animationCounter}.");
                return false;
            }

            this._counter.Increase();
        }

        this.LastRegisteredSkillId = skillId;
        this._hits[animationCounter] = new HitEntry(skillId, DateTime.UtcNow, true, 0);
        return true;
    }

    /// <summary>
    /// Increases the counter by one, after a hit was done.
    /// It must just be called once after a combined hit.
    /// </summary>
    public void IncreaseCounterAfterHit()
    {
        this._counter.Increase();
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
        if (animationCounter == 0 && skillId == TwisterSkillId)
        {
            // Twister is implemented wrong at the client side. It doesn't send an animation counter in hit packets.
            animationCounter = this._lastTwisterIndex;
        }

        if (animationCounter == 0 && skillId == MultishotSkillId)
        {
            // Multishot is implemented wrong at the client side. It doesn't send an animation counter in hit packets.
            animationCounter = this._lastMultishotIndex;
        }

        if (this._hits[animationCounter] is { } animationEntry)
        {
            /*
            if (this._hits[hitCounter] is { } currentHitEntry && currentHitEntry.TimeStamp >= DateTime.UtcNow.AddSeconds(-1))
            {
                // probably speed too high to allow any check
                this._hits[animationCounter] = animationEntry with
                {
                    Skill = skillId,
                    IsAnimation = true,
                    HitCount = animationEntry.HitCount + 1,
                };
                this._logger.LogDebug($"Hits are too fast, skipping validity check.");
                this._hits[hitCounter] = new HitEntry(skillId, DateTime.UtcNow, false, 0);
                return (true, true);
            }

            if (!animationEntry.IsAnimation)
            {
                this._logger.LogWarning("Possible Hacker - Skill Hit Invalid because the given animation counter wasn't registered as animation.");
                return (false, false);
            }

            var expectedCount = this._counter.Count;
            if (expectedCount != hitCounter)
            {
                this._logger.LogWarning($"Hit count out of sync - hacker? Expected: {this._counter.Count}, Actual: {hitCounter}.");
                return (false, false);
            }

            if (animationEntry.Skill != skillId)
            {
                this._logger.LogWarning($"Wrong skill in referenced animation - hacker?");
                return (false, false);
            }

            var newCount = animationEntry.HitCount + 1;

            this._hits[animationCounter] = animationEntry with { HitCount = newCount };
            this._hits[hitCounter] = new HitEntry(skillId, DateTime.UtcNow, false, 0);

            var timestampDiff = DateTime.UtcNow - animationEntry.TimeStamp;
            if (timestampDiff > MaxAnimationToHitDelay)
            {
                this._logger.LogWarning("Possible Hacker - Skill Hit Invalid because of too high time difference between animation and hit");
                return (false, true);
            }
            */
            return (true, true);
        }
        else
        {
            this._logger.LogWarning("Possible Hacker - Skill Hit Invalid because of missing previous animation.");
            return (false, false);
        }
    }


    private record struct HitEntry(ushort Skill, DateTime TimeStamp, bool IsAnimation, int HitCount);
}