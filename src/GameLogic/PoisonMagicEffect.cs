// <copyright file="PoisonMagicEffect.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Timers;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// The magic effect for poison, which will damage the character in an interval until the effect ends.
    /// </summary>
    public sealed class PoisonMagicEffect : MagicEffect
    {
        private readonly Timer damageTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoisonMagicEffect"/> class.
        /// </summary>
        /// <param name="powerUp">The power up.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="attacker">The attacker.</param>
        /// <param name="owner">The owner.</param>
        public PoisonMagicEffect(IElement powerUp, MagicEffectDefinition definition, TimeSpan duration, IAttacker attacker, IAttackable owner)
            : base(powerUp, definition, duration)
        {
            this.Attacker = attacker;
            this.Owner = owner;
            this.damageTimer = new System.Timers.Timer(3000);
            this.damageTimer.Elapsed += this.OnDamageTimerElapsed;
            this.damageTimer.Start();
        }

        /// <summary>
        /// Gets the owner of the effect.
        /// </summary>
        public IAttackable Owner { get; }

        /// <summary>
        /// Gets the attacker which applied the effect.
        /// </summary>
        public IAttacker Attacker { get; }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.damageTimer.Stop();
            this.damageTimer.Dispose();
        }

        private void OnDamageTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!this.Owner.IsAlive || this.IsDisposed || this.IsDisposing)
                {
                    return;
                }

                var damage = this.Owner.Attributes[Stats.CurrentHealth] * this.Attacker.Attributes[Stats.PoisonDamageMultiplier];
                if (damage <= 0)
                {
                    return;
                }

                this.Owner.ApplyPoisonDamage(this.Attacker, (uint)damage);
            }
            catch (Exception ex)
            {
                (this.Owner as ILoggerOwner)?.Logger.LogError(ex, "Error when applying posion damage");
            }
        }
    }
}