// <copyright file="DiscordChannelRoutingTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using MUnique.OpenMU.Discord;
using MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// Tests for the <see cref="DiscordChannelRouting"/>.
/// </summary>
[TestFixture]
public class DiscordChannelRoutingTest
{
    /// <summary>
    /// Tests that the notifications, the status and the world chat are routed to the existing channels of the layout.
    /// </summary>
    [Test]
    public void LayoutChannelsAreUsed()
    {
        var channelIds = new Dictionary<string, ulong> { { "events", 10 }, { "server-status", 20 }, { "world-chat", 30 } };

        var routing = DiscordChannelRouting.Create(new DiscordBotSettings(), DiscordServerLayout.LoadDefault(), channelIds);

        Assert.That(routing.GetChannelId(DiscordChannelCategory.Events), Is.EqualTo(10));
        Assert.That(routing.GetChannelId(DiscordChannelCategory.CastleSiege), Is.EqualTo(10));
        Assert.That(routing.GetChannelId(DiscordChannelCategory.WorldNews), Is.Null);
        Assert.That(routing.StatusChannelId, Is.EqualTo(20));
        Assert.That(routing.WorldChatChannelId, Is.EqualTo(30));
    }

    /// <summary>
    /// Tests that the configured channels take precedence over the channels of the layout.
    /// </summary>
    [Test]
    public void ConfiguredChannelsTakePrecedence()
    {
        var settings = new DiscordBotSettings { StatusChannelId = 2 };
        settings.Channels[DiscordChannelCategory.Events] = 1;
        var channelIds = new Dictionary<string, ulong> { { "events", 10 }, { "server-status", 20 } };

        var routing = DiscordChannelRouting.Create(settings, DiscordServerLayout.LoadDefault(), channelIds);

        Assert.That(routing.GetChannelId(DiscordChannelCategory.Events), Is.EqualTo(1));
        Assert.That(routing.GetChannelId(DiscordChannelCategory.CastleSiege), Is.EqualTo(10));
        Assert.That(routing.StatusChannelId, Is.EqualTo(2));
    }
}
