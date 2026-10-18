// <copyright file="DiscordFloodGuardTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using MUnique.OpenMU.Discord.ChatBridge;

/// <summary>
/// Tests for the <see cref="DiscordFloodGuard"/>.
/// </summary>
[TestFixture]
public class DiscordFloodGuardTest
{
    /// <summary>
    /// Tests that a user can send the maximum number of messages per minute, and again after a minute.
    /// </summary>
    [Test]
    public void MessagesAreLimitedPerMinute()
    {
        var time = new ManualTimeProvider();
        var guard = new DiscordFloodGuard(2, time);

        Assert.That(guard.TryRegister(1), Is.True);
        Assert.That(guard.TryRegister(1), Is.True);
        Assert.That(guard.TryRegister(1), Is.False);
        Assert.That(guard.TryRegister(2), Is.True);

        time.Advance(TimeSpan.FromMinutes(1));
        Assert.That(guard.TryRegister(1), Is.True);
    }

    /// <summary>
    /// Tests that the users who didn't send a message within a minute are forgotten, so that the guard doesn't grow forever.
    /// </summary>
    [Test]
    public void InactiveUsersAreForgotten()
    {
        var time = new ManualTimeProvider();
        var guard = new DiscordFloodGuard(10, time);
        for (ulong userId = 0; userId < 99; userId++)
        {
            guard.TryRegister(userId);
        }

        time.Advance(TimeSpan.FromMinutes(2));
        guard.TryRegister(1000);

        Assert.That(guard.TrackedUserCount, Is.EqualTo(1));
    }

    private sealed class ManualTimeProvider : TimeProvider
    {
        private DateTimeOffset _now = new(2026, 10, 18, 12, 0, 0, TimeSpan.Zero);

        public override DateTimeOffset GetUtcNow() => this._now;

        public void Advance(TimeSpan timeSpan) => this._now += timeSpan;
    }
}
