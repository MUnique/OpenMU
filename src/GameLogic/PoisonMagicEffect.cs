// <copyright file="PoisonMagicEffect.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Timers;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The magic effect for poison, which will damage the character in an interval until the effect ends.
/// </summary>
public sealed class PoisonMagicEffect : MagicEffect
{
    private readonly Timer _damageTimer;

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
        this._damageTimer = new System.Timers.Timer(3000);
        this._damageTimer.Elapsed += this.OnDamageTimerElapsed;
        this._damageTimer.Start();
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
        this._damageTimer.Stop();
        this._damageTimer.Dispose();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnDamageTimerElapsed(object? sender, ElapsedEventArgs e)
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

            await this.Owner.ApplyPoisonDamageAsync(this.Attacker, (uint)damage);
        }
        catch (Exception ex)
        {
            (this.Owner as ILoggerOwner)?.Logger.LogError(ex, "Error when applying posion damage");
        }
    }
}