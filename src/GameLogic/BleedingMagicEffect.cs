// <copyright file="BleedingMagicEffect.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Timers;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// The magic effect for bleeding, which will damage the character every second until the effect ends.
/// </summary>
public sealed class BleedingMagicEffect : MagicEffect
{
    private readonly Timer _damageTimer;
    private readonly float _damage;

    /// <summary>
    /// Initializes a new instance of the <see cref="BleedingMagicEffect"/> class.
    /// </summary>
    /// <param name="powerUp">The power up.</param>
    /// <param name="definition">The definition.</param>
    /// <param name="duration">The duration.</param>
    /// <param name="attacker">The attacker.</param>
    /// <param name="owner">The owner.</param>
    /// <param name="damage">The bleeding damage.</param>
    public BleedingMagicEffect(IElement powerUp, MagicEffectDefinition definition, TimeSpan duration, IAttacker attacker, IAttackable owner, float damage)
        : base(powerUp, definition, duration)
    {
        this.Attacker = attacker;
        this.Owner = owner;
        this._damage = damage;
        this._damageTimer = new Timer(1000);
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
            if (!this.Owner.IsAlive || this.IsDisposed || this.IsDisposing || this._damage <= 0)
            {
                return;
            }

            await this.Owner.ApplyBleedingDamageAsync(this.Attacker, (uint)this._damage).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            (this.Owner as ILoggerOwner)?.Logger.LogError(ex, "Error when applying bleeding damage");
        }
    }
}