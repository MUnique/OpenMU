// <copyright file="MagicEffect.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Diagnostics;
using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A magic effect, usually given by an applied skill or consumed item.
/// </summary>
public class MagicEffect : AsyncDisposable
{
    private readonly Timer _finishTimer;
    private readonly Lock _timerSync = new();
    private long _finishAtTimestamp;

    /// <summary>
    /// Initializes a new instance of the <see cref="MagicEffect"/> class.
    /// </summary>
    /// <param name="powerUp">The power up.</param>
    /// <param name="definition">The definition.</param>
    /// <param name="duration">The duration.</param>
    public MagicEffect(IElement powerUp, MagicEffectDefinition definition, TimeSpan duration)
        : this(
            duration,
            definition,
            definition.PowerUpDefinitions
                .Select(def => new ElementWithTarget(powerUp, def.TargetAttribute ?? throw new InvalidOperationException($"MagicEffectDefinition {definition.GetId()} has no target attribute.")))
                .ToArray())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MagicEffect"/> class.
    /// </summary>
    /// <param name="duration">The duration.</param>
    /// <param name="definition">The definition.</param>
    /// <param name="powerUps">The power ups.</param>
    public MagicEffect(TimeSpan duration, MagicEffectDefinition definition, params ElementWithTarget[] powerUps)
    {
        this.PowerUpElements = powerUps;
        this.Definition = definition;
        this.Duration = duration;
        this._finishAtTimestamp = GetTimestampAfter(duration);
        this._finishTimer = new Timer(this.OnTimerTimeout, null, (int)this.Duration.TotalMilliseconds, Timeout.Infinite);
    }

    /// <summary>
    /// Occurs when the effect has been timed out.
    /// </summary>
    public event AsyncEventHandler<MagicEffect>? EffectTimeOut;

    /// <summary>
    /// Gets the identifier of the effect.
    /// </summary>
    public short Id => this.Definition.Number;

    /// <summary>
    /// Gets or sets the duration of the effect.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Gets the time remaining before the effect times out.
    /// </summary>
    public TimeSpan TimeLeft
    {
        get
        {
            var remainingTicks = Interlocked.Read(ref this._finishAtTimestamp) - Stopwatch.GetTimestamp();
            return remainingTicks > 0
                ? TimeSpan.FromSeconds((double)remainingTicks / Stopwatch.Frequency)
                : TimeSpan.Zero;
        }
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public float Value
    {
        get
        {
            if (!this.PowerUpElements.Any())
            {
                return 0;
            }

            return this.PowerUpElements.First().Element.Value;
        }
    }

    /// <summary>
    /// Gets or sets the power up elements.
    /// </summary>
    public IEnumerable<ElementWithTarget> PowerUpElements { get; set; }

    /// <summary>
    /// Gets the definition.
    /// </summary>
    public MagicEffectDefinition Definition { get; }

    /// <summary>
    /// Resets the timer.
    /// </summary>
    public void ResetTimer()
    {
        lock (this._timerSync)
        {
            if (this.IsDisposed || this.IsDisposing)
            {
                throw new ObjectDisposedException(nameof(MagicEffect));
            }

            this.ChangeTimer(this.Duration);
        }
    }

    /// <summary>
    /// Decreases the timer by a given amount of <paramref name="seconds"/>.
    /// </summary>
    /// <param name="seconds">The seconds to decrease the timer for.</param>
    public void DecreaseTimer(int seconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(seconds);

        lock (this._timerSync)
        {
            if (this.IsDisposed || this.IsDisposing)
            {
                throw new ObjectDisposedException(nameof(MagicEffect));
            }

            var remainingTicks = Interlocked.Read(ref this._finishAtTimestamp) - Stopwatch.GetTimestamp();
            var remainingTime = remainingTicks > 0
                ? TimeSpan.FromSeconds((double)remainingTicks / Stopwatch.Frequency)
                : TimeSpan.Zero;
            var shortenedTime = remainingTime - TimeSpan.FromSeconds(seconds);
            this.ChangeTimer(shortenedTime > TimeSpan.Zero ? shortenedTime : TimeSpan.Zero);
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsyncCore()
    {
        await this._finishTimer.DisposeAsync().ConfigureAwait(false);
        await this.OnEffectTimeOutAsync().ConfigureAwait(false);
        this.EffectTimeOut = null;

        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    private static long GetTimestampAfter(TimeSpan duration)
    {
        return Stopwatch.GetTimestamp() + (long)(duration.TotalSeconds * Stopwatch.Frequency);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnTimerTimeout(object? state)
    {
        try
        {
            await this.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
    }

    private async ValueTask OnEffectTimeOutAsync()
    {
        try
        {
            if (this.EffectTimeOut is { } eventHandler)
            {
                await eventHandler(this).ConfigureAwait(false);
            }

            if (!this.IsDisposed && !this.IsDisposing)
            {
                await this.DisposeAsync().ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
    }

    private void ChangeTimer(TimeSpan duration)
    {
        Interlocked.Exchange(ref this._finishAtTimestamp, GetTimestampAfter(duration));
        this._finishTimer.Change((int)duration.TotalMilliseconds, Timeout.Infinite);
    }

    /// <summary>
    /// Holds the element containing the boost value with its target attribute.
    /// </summary>
    public class ElementWithTarget
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementWithTarget"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="target">The target attribute.</param>
        public ElementWithTarget(IElement element, AttributeDefinition target)
        {
            this.Element = element;
            this.Target = target;
        }

        /// <summary>
        /// Gets the element containing the boost value.
        /// </summary>
        public IElement Element { get; }

        /// <summary>
        /// Gets the target attribute.
        /// </summary>
        public AttributeDefinition Target { get; }
    }
}