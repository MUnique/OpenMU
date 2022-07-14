// <copyright file="MagicEffect.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Nito.AsyncEx.Synchronous;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="MagicEffect"/> class.
    /// </summary>
    /// <param name="powerUp">The power up.</param>
    /// <param name="definition">The definition.</param>
    /// <param name="duration">The duration.</param>
    public MagicEffect(IElement powerUp, MagicEffectDefinition definition, TimeSpan duration)
        : this(duration, definition, new ElementWithTarget(powerUp, definition.PowerUpDefinition?.TargetAttribute ?? throw new InvalidOperationException($"MagicEffectDefinition {definition.GetId()} has no target attribute.")))
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
        if (this.IsDisposed)
        {
            throw new ObjectDisposedException(nameof(MagicEffect));
        }

        this._finishTimer.Change((int)this.Duration.TotalMilliseconds, Timeout.Infinite);
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsyncCore()
    {
        await this._finishTimer.DisposeAsync();
        await this.OnEffectTimeOutAsync();
        this.EffectTimeOut = null;

        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnTimerTimeout(object? state)
    {
        try
        {
            await this.DisposeAsync();
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
                await eventHandler(this);
            }

            if (!this.IsDisposed && !this.IsDisposing)
            {
                await this.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
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