// <copyright file="MagicEffect.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Skill Effect, used by Skill Effect List in each
    /// Player Instance.
    /// Additional needed information should be get through the
    /// global list, to save memory.
    /// </summary>
    public sealed class MagicEffect : IDisposable
    {
        private Timer finishTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MagicEffect"/> class.
        /// </summary>
        /// <param name="powerUp">The power up.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="duration">The duration.</param>
        public MagicEffect(IElement powerUp, MagicEffectDefinition definition, TimeSpan duration)
        {
            this.PowerUpElements = Enumerable.Repeat(new ElementWithTarget(powerUp, definition.PowerUpDefinition.TargetAttribute), 1);
            this.Definition = definition;

            this.Duration = duration;
            this.finishTimer = new Timer(o => this.OnEffectTimeOut(), null, (int)this.Duration.TotalMilliseconds, Timeout.Infinite);
        }

        /// <summary>
        /// Occurs when the effect has been timed out.
        /// </summary>
        public event EventHandler EffectTimeOut;

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

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.finishTimer != null)
            {
                this.finishTimer.Dispose();
                this.finishTimer = null;
                this.OnEffectTimeOut();
            }
        }

        /// <summary>
        /// Resets the timer.
        /// </summary>
        public void ResetTimer()
        {
            if (this.finishTimer == null)
            {
                throw new ObjectDisposedException(nameof(MagicEffect));
            }

            this.finishTimer.Change((int)this.Duration.TotalMilliseconds, Timeout.Infinite);
        }

        private void OnEffectTimeOut()
        {
            this.EffectTimeOut?.Invoke(this, EventArgs.Empty);

            this.Dispose();
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
}
