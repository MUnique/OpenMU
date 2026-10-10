// <copyright file="DiscordIntegrationActionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using MUnique.OpenMU.GameLogic;
using Moq;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.GameLogic.Discord;
using MUnique.OpenMU.GameLogic.PlayerActions.Discord;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Tests for the <see cref="DiscordIntegrationAction"/>.
/// </summary>
[TestFixture]
public class DiscordIntegrationActionTest
{
    /// <summary>
    /// Tests that a server without active Discord integration tells the client nothing.
    /// </summary>
    [Test]
    public async Task InactiveIntegrationGivesNoInfoAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);

        var info = await new DiscordIntegrationAction().GetIntegrationInfoAsync(player).ConfigureAwait(false);

        Assert.That(info, Is.EqualTo(DiscordIntegrationInfo.None));
    }

    /// <summary>
    /// Tests that the info carries the configuration, and the Discord user once the account is linked.
    /// </summary>
    [Test]
    public async Task InfoCarriesConfigurationAndLinkAsync()
    {
        var player = await CreatePlayerWithIntegrationAsync().ConfigureAwait(false);
        var service = CreateLinkService(player);

        var unlinked = await new DiscordIntegrationAction().GetIntegrationInfoAsync(player).ConfigureAwait(false);
        var code = await service.CreateCodeAsync(player.Account!.GetId(), AccountLinkService.DiscordProvider, "Hero").ConfigureAwait(false);
        await service.LinkAsync(AccountLinkService.DiscordProvider, code, "42", "sven").ConfigureAwait(false);
        var linked = await new DiscordIntegrationAction().GetIntegrationInfoAsync(player).ConfigureAwait(false);

        Assert.That(unlinked.Configuration?.InviteUrl, Is.EqualTo("https://discord.gg/abc123"));
        Assert.That(unlinked.LinkedUserName, Is.Null);
        Assert.That(linked.LinkedUserName, Is.EqualTo("sven"));
    }

    /// <summary>
    /// Tests that unlinking removes the link which the info shows.
    /// </summary>
    [Test]
    public async Task UnlinkRemovesTheLinkAsync()
    {
        var player = await CreatePlayerWithIntegrationAsync().ConfigureAwait(false);
        var service = CreateLinkService(player);
        var code = await service.CreateCodeAsync(player.Account!.GetId(), AccountLinkService.DiscordProvider, "Hero").ConfigureAwait(false);
        await service.LinkAsync(AccountLinkService.DiscordProvider, code, "42", "sven").ConfigureAwait(false);

        var removed = await DiscordIntegrationAction.UnlinkAsync(player).ConfigureAwait(false);
        var info = await new DiscordIntegrationAction().GetIntegrationInfoAsync(player).ConfigureAwait(false);

        Assert.That(removed, Is.True);
        Assert.That(info.LinkedUserName, Is.Null);
    }

    /// <summary>
    /// Tests that a player gets a new link code only after the cooldown, so a client asking in a loop
    /// doesn't write to the database every time.
    /// </summary>
    [Test]
    public async Task LinkCodesAreThrottledAsync()
    {
        var player = await CreatePlayerWithIntegrationAsync().ConfigureAwait(false);
        var results = new List<DiscordLinkCodeResult>();
        Mock.Get(player.ViewPlugIns.GetPlugIn<IDiscordIntegrationViewPlugIn>()!)
            .Setup(v => v.ShowDiscordLinkCodeAsync(It.IsAny<DiscordLinkCodeResult>(), It.IsAny<string?>(), It.IsAny<TimeSpan>()))
            .Callback<DiscordLinkCodeResult, string?, TimeSpan>((result, _, _) => results.Add(result))
            .Returns(ValueTask.CompletedTask);
        var time = new ManualTimeProvider();
        var action = new DiscordIntegrationAction(time);

        await action.RequestLinkCodeAsync(player).ConfigureAwait(false);
        await action.RequestLinkCodeAsync(player).ConfigureAwait(false);
        time.Advance(DiscordIntegrationAction.LinkCodeCooldown);
        await action.RequestLinkCodeAsync(player).ConfigureAwait(false);

        Assert.That(results, Is.EqualTo(new[] { DiscordLinkCodeResult.Created, DiscordLinkCodeResult.TooSoon, DiscordLinkCodeResult.Created }));
    }

    private static async ValueTask<Player> CreatePlayerWithIntegrationAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.SelectedCharacter!.Name = "Hero";
        var configuration = new DiscordIntegrationConfiguration { InviteUrl = "https://discord.gg/abc123" };
        player.GameContext.FeaturePlugIns.AddPlugIn(new DiscordIntegrationFeaturePlugIn { Configuration = configuration }, true);
        return player;
    }

    private static AccountLinkService CreateLinkService(Player player)
    {
        return new AccountLinkService(() => player.GameContext.PersistenceContextProvider.CreateNewPlayerContext(player.GameContext.Configuration));
    }

    private sealed class ManualTimeProvider : TimeProvider
    {
        private DateTimeOffset _now = new(2026, 10, 10, 12, 0, 0, TimeSpan.Zero);

        public override DateTimeOffset GetUtcNow() => this._now;

        public void Advance(TimeSpan duration) => this._now += duration;
    }
}
