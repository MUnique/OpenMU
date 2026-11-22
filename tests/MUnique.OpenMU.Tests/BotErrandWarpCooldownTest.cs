// <copyright file="BotErrandWarpCooldownTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Tests that the free warp home for an errand (a shopping or buff trip) holds the bot in town only
/// until the errand is done, not for the whole warp cooldown.
/// </summary>
[TestFixture]
public class BotErrandWarpCooldownTest
{
    /// <summary>
    /// Once the errand is done, the bot may warp back to its hunting ground right away.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask DoneErrandEndsTheCooldownAsync()
    {
        var (navigator, timeProvider) = await CreateNavigatorAsync().ConfigureAwait(false);

        navigator.StartErrandHomeWarp();
        timeProvider.Advance(TimeSpan.FromSeconds(20));
        Assert.That(navigator.IsInWarpCooldown(), Is.True, "the errand is not done yet");

        navigator.EndErrandAway(navigator.TakeErrandHomeWarp());
        Assert.That(navigator.IsInWarpCooldown(), Is.False);
    }

    /// <summary>
    /// A bot which warped home but has not done its errand yet keeps the cooldown, so it can't warp
    /// back and forth, paying the fare every time.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask PendingErrandKeepsTheCooldownAsync()
    {
        var (navigator, timeProvider) = await CreateNavigatorAsync().ConfigureAwait(false);

        navigator.StartErrandHomeWarp();
        timeProvider.Advance(TimeSpan.FromMinutes(1));

        Assert.That(navigator.IsInWarpCooldown(), Is.True);
    }

    /// <summary>
    /// An errand on the bot's own map, without a warp home, leaves the cooldown of its last warp alone:
    /// the bot did not leave its hunting ground, so going shopping must not let it hop maps sooner.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask ErrandWithoutHomeWarpKeepsTheCooldownAsync()
    {
        var (navigator, timeProvider) = await CreateNavigatorAsync().ConfigureAwait(false);

        // A completed errand away clears its home warp, so it stands for "the last warp was some other one".
        navigator.StartErrandHomeWarp();
        Assert.That(navigator.TakeErrandHomeWarp(), Is.True);
        timeProvider.Advance(TimeSpan.FromMinutes(1));

        var isAway = navigator.TakeErrandHomeWarp();
        navigator.EndErrandAway(isAway);

        Assert.That(isAway, Is.False);
        Assert.That(navigator.IsInWarpCooldown(), Is.True);
    }

    /// <summary>
    /// The cooldown still runs out on its own after the usual time.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask CooldownEndsAfterItsTimeAsync()
    {
        var (navigator, timeProvider) = await CreateNavigatorAsync().ConfigureAwait(false);

        navigator.StartErrandHomeWarp();
        timeProvider.Advance(TimeSpan.FromMinutes(4));

        Assert.That(navigator.IsInWarpCooldown(), Is.False);
    }

    private static async ValueTask<(BotNavigator Navigator, ManualTimeProvider TimeProvider)> CreateNavigatorAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 11, 20, 18, 0, 0, TimeSpan.Zero));
        return (new BotNavigator(bot, timeProvider), timeProvider);
    }

    private sealed class ManualTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        private DateTimeOffset _utcNow = utcNow;

        public override DateTimeOffset GetUtcNow() => this._utcNow;

        public void Advance(TimeSpan timeSpan)
        {
            this._utcNow += timeSpan;
        }
    }
}
