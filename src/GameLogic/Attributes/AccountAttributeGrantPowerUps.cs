// <copyright file="AccountAttributeGrantPowerUps.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Attributes;

using System.Threading;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// The power-ups of the valid <see cref="Account.AttributeGrants"/> of an account, which are applied to
/// the attribute system of the character which is in the game. They are removed when the grant expires.
/// </summary>
/// <remarks>
/// Grants which are added or extended while the character is in the game are applied when a character
/// enters the game the next time.
/// </remarks>
internal sealed class AccountAttributeGrantPowerUps : IDisposable
{
    /// <summary>
    /// The maximum due time of a <see cref="ITimer"/>. A grant which expires later is checked again after this time.
    /// </summary>
    private static readonly TimeSpan MaximumTimerDueTime = TimeSpan.FromMilliseconds(uint.MaxValue - 1);

    private readonly TimeProvider _timeProvider;
    private readonly List<(AccountAttributeGrant Grant, IElement Element)> _powerUps = new();
    private AttributeSystem? _attributes;
    private ITimer? _expirationTimer;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountAttributeGrantPowerUps"/> class.
    /// </summary>
    /// <param name="timeProvider">The time provider.</param>
    public AccountAttributeGrantPowerUps(TimeProvider? timeProvider = null)
    {
        this._timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <summary>
    /// Applies the power-ups of the valid grants of the account to the attribute system,
    /// replacing the ones which were applied before.
    /// </summary>
    /// <param name="account">The account.</param>
    /// <param name="attributes">The attribute system of the character which entered the game.</param>
    public void Apply(Account account, AttributeSystem attributes)
    {
        lock (this._powerUps)
        {
            this.RemoveWithinLock();
            if (account.AttributeGrants is not { Count: > 0 } grants)
            {
                return;
            }

            var utcNow = this._timeProvider.GetUtcNow().UtcDateTime;
            foreach (var grant in grants)
            {
                if (grant.Attribute is not { } attribute || !grant.IsValidAt(utcNow))
                {
                    continue;
                }

                var element = new SimpleElement(grant.Value, AggregateType.AddRaw);
                attributes.AddElement(element, attribute);
                this._powerUps.Add((grant, element));
            }

            this._attributes = attributes;
            this.ScheduleNextExpirationWithinLock(utcNow);
        }
    }

    /// <summary>
    /// Removes the applied power-ups from the attribute system.
    /// </summary>
    public void Remove()
    {
        lock (this._powerUps)
        {
            this.RemoveWithinLock();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.Remove();
    }

    private void OnExpirationTimerElapsed(object? state)
    {
        lock (this._powerUps)
        {
            if (this._attributes is not { } attributes)
            {
                return;
            }

            var utcNow = this._timeProvider.GetUtcNow().UtcDateTime;
            for (var i = this._powerUps.Count - 1; i >= 0; i--)
            {
                var (grant, element) = this._powerUps[i];
                if (!grant.IsValidAt(utcNow))
                {
                    attributes.RemoveElement(element, grant.Attribute!);
                    this._powerUps.RemoveAt(i);
                }
            }

            this.ScheduleNextExpirationWithinLock(utcNow);
        }
    }

    private void ScheduleNextExpirationWithinLock(DateTime utcNow)
    {
        this._expirationTimer?.Dispose();
        this._expirationTimer = null;

        var nextExpiration = this._powerUps
            .Select(powerUp => powerUp.Grant.ValidUntil)
            .Where(validUntil => validUntil.HasValue)
            .Min();
        if (nextExpiration is not { } expiration)
        {
            return;
        }

        var dueTime = expiration - utcNow;
        if (dueTime < TimeSpan.Zero)
        {
            dueTime = TimeSpan.Zero;
        }
        else if (dueTime > MaximumTimerDueTime)
        {
            dueTime = MaximumTimerDueTime;
        }

        this._expirationTimer = this._timeProvider.CreateTimer(this.OnExpirationTimerElapsed, null, dueTime, Timeout.InfiniteTimeSpan);
    }

    private void RemoveWithinLock()
    {
        this._expirationTimer?.Dispose();
        this._expirationTimer = null;
        if (this._attributes is { } attributes)
        {
            foreach (var (grant, element) in this._powerUps)
            {
                attributes.RemoveElement(element, grant.Attribute!);
            }
        }

        this._powerUps.Clear();
        this._attributes = null;
    }
}
