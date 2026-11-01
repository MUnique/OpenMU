// <copyright file="AccountAttributeGrantPowerUpsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Tests for the <see cref="AccountAttributeGrantPowerUps"/>.
/// </summary>
[TestFixture]
public class AccountAttributeGrantPowerUpsTest
{
    private static readonly AttributeDefinition Vip = new(Guid.NewGuid(), "VIP", string.Empty);

    private ManualTimeProvider _timeProvider = null!;

    /// <summary>
    /// Sets up the time provider.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 10, 31, 12, 0, 0, TimeSpan.Zero));
    }

    /// <summary>
    /// Tests if the valid grants are added up and applied, and the expired ones are not.
    /// </summary>
    [Test]
    public void ApplyAddsValidGrants()
    {
        var account = this.CreateAccount(
            (1, null),
            (2, this._timeProvider.UtcNow.AddDays(1)),
            (4, this._timeProvider.UtcNow.AddDays(-1)));
        var attributes = CreateAttributeSystem();
        using var powerUps = new AccountAttributeGrantPowerUps(this._timeProvider);

        powerUps.Apply(account, attributes);

        Assert.That(attributes[Vip], Is.EqualTo(3));
    }

    /// <summary>
    /// Tests if a grant is removed from the attribute system when it expires, and the other ones stay.
    /// </summary>
    [Test]
    public void ExpiredGrantIsRemoved()
    {
        var account = this.CreateAccount(
            (1, null),
            (2, this._timeProvider.UtcNow.AddHours(1)),
            (4, this._timeProvider.UtcNow.AddHours(2)));
        var attributes = CreateAttributeSystem();
        using var powerUps = new AccountAttributeGrantPowerUps(this._timeProvider);
        powerUps.Apply(account, attributes);

        this._timeProvider.Advance(TimeSpan.FromHours(1));
        Assert.That(attributes[Vip], Is.EqualTo(5));

        this._timeProvider.Advance(TimeSpan.FromHours(1));
        Assert.That(attributes[Vip], Is.EqualTo(1));
    }

    /// <summary>
    /// Tests if removing the power-ups removes the values and stops the expiration timer.
    /// </summary>
    [Test]
    public void RemoveRemovesGrants()
    {
        var account = this.CreateAccount((2, this._timeProvider.UtcNow.AddHours(1)));
        var attributes = CreateAttributeSystem();
        using var powerUps = new AccountAttributeGrantPowerUps(this._timeProvider);
        powerUps.Apply(account, attributes);

        powerUps.Remove();

        Assert.That(attributes[Vip], Is.Zero);
        Assert.That(this._timeProvider.ActiveTimerCount, Is.Zero);
    }

    private static AttributeSystem CreateAttributeSystem() => new([], [], []);

    private Account CreateAccount(params (float Value, DateTime? ValidUntil)[] grants)
    {
        var account = new Persistence.BasicModel.Account();
        foreach (var (value, validUntil) in grants)
        {
            account.AttributeGrants.Add(new Persistence.BasicModel.AccountAttributeGrant { Attribute = Vip, Value = value, ValidUntil = validUntil });
        }

        return account;
    }

    /// <summary>
    /// A time provider whose time is advanced manually, and which fires the timers which are due.
    /// </summary>
    private sealed class ManualTimeProvider : TimeProvider
    {
        private readonly List<ManualTimer> _timers = new();
        private DateTimeOffset _utcNow;

        public ManualTimeProvider(DateTimeOffset utcNow)
        {
            this._utcNow = utcNow;
        }

        public DateTime UtcNow => this._utcNow.UtcDateTime;

        public int ActiveTimerCount => this._timers.Count(timer => timer.DueAt is not null);

        public override DateTimeOffset GetUtcNow() => this._utcNow;

        public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        {
            var timer = new ManualTimer(this, callback, state);
            ((ITimer)timer).Change(dueTime, period);
            this._timers.Add(timer);
            return timer;
        }

        public void Advance(TimeSpan timeSpan)
        {
            this._utcNow += timeSpan;
            foreach (var timer in this._timers.Where(timer => timer.DueAt <= this._utcNow).ToList())
            {
                timer.Fire();
            }
        }

        private sealed class ManualTimer : ITimer
        {
            private readonly ManualTimeProvider _provider;
            private readonly TimerCallback _callback;
            private readonly object? _state;
            private TimeSpan _period = Timeout.InfiniteTimeSpan;

            public ManualTimer(ManualTimeProvider provider, TimerCallback callback, object? state)
            {
                this._provider = provider;
                this._callback = callback;
                this._state = state;
            }

            public DateTimeOffset? DueAt { get; private set; }

            bool ITimer.Change(TimeSpan dueTime, TimeSpan period)
            {
                this._period = period;
                this.DueAt = dueTime == Timeout.InfiniteTimeSpan ? null : this._provider._utcNow + dueTime;
                return true;
            }

            public void Fire()
            {
                this.DueAt = this._period == Timeout.InfiniteTimeSpan ? null : this._provider._utcNow + this._period;
                this._callback(this._state);
            }

            public void Dispose() => this.DueAt = null;

            public ValueTask DisposeAsync()
            {
                this.Dispose();
                return ValueTask.CompletedTask;
            }
        }
    }
}
