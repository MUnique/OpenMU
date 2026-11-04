// <copyright file="DiscordDeliveryRouteTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using Moq;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Verifies shared routing used by real notifications and delivery tests.
/// </summary>
[TestFixture]
public class DiscordDeliveryRouteTest
{
    /// <summary>
    /// A stopped bot must not capture notifications which a delivery test sends by webhook.
    /// </summary>
    [TestCase(ServerState.Stopped, null, "https://discord.example/webhook")]
    [TestCase(ServerState.Starting, null, "https://discord.example/webhook")]
    [TestCase(ServerState.Started, 42UL, null)]
    public void ResolvesConnectedBotOtherwiseWebhook(ServerState state, ulong? expectedChannel, string? expectedWebhook)
    {
        var messenger = new Mock<IDiscordMessenger>();
        messenger.As<IManageableServer>().SetupGet(server => server.ServerState).Returns(state);
        messenger.Setup(bot => bot.GetChannelId(DiscordChannelCategory.Events)).Returns(42);
        var settings = new DiscordSettings { Webhooks = { [DiscordChannelCategory.Events] = "https://discord.example/webhook" } };
        var route = DiscordDeliveryRoute.Resolve(settings, messenger.Object, DiscordChannelCategory.Events);
        Assert.That(route.ChannelId, Is.EqualTo(expectedChannel));
        Assert.That(route.WebhookUrl, Is.EqualTo(expectedWebhook));
    }

    /// <summary>
    /// Explicitly disabled categories ignore both their bot destination and webhook.
    /// </summary>
    [Test]
    public void DisabledCategoryHasNoRoute()
    {
        var settings = new DiscordSettings
        {
            DisabledNotifications = { DiscordChannelCategory.Events },
            Webhooks = { [DiscordChannelCategory.Events] = "https://discord.example/webhook" },
        };
        var route = DiscordDeliveryRoute.Resolve(settings, null, DiscordChannelCategory.Events);
        Assert.That(route.ChannelId, Is.Null);
        Assert.That(route.WebhookUrl, Is.Null);
    }
}
