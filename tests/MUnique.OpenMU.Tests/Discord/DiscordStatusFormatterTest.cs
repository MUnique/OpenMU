// <copyright file="DiscordStatusFormatterTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.Globalization;
using MUnique.OpenMU.Discord;

/// <summary>
/// Tests for the <see cref="DiscordStatusFormatter"/>.
/// </summary>
[TestFixture]
public class DiscordStatusFormatterTest
{
    private static readonly IReadOnlyList<GameServerStatus> Servers = new[]
    {
        new GameServerStatus(0, "Server 1", true, 12, 100),
        new GameServerStatus(1, "Server 2", true, 3, 100),
        new GameServerStatus(2, "Server 3", false, 0, 100),
    };

    /// <summary>
    /// Tests the status message.
    /// </summary>
    [Test]
    public void StatusListsAllServers()
    {
        var status = new DiscordStatusFormatter(CultureInfo.GetCultureInfo("en")).CreateStatus(Servers, DateTime.UtcNow);

        Assert.That(status.Description, Is.EqualTo("🟢 **Server 1**: 12 / 100 players\n🟢 **Server 2**: 3 / 100 players\n🔴 **Server 3**: offline"));
        Assert.That(status.Color, Is.EqualTo(DiscordStatusFormatter.OnlineColor));
    }

    /// <summary>
    /// Tests the status message without any server online.
    /// </summary>
    [Test]
    public void StatusWithoutOnlineServerIsRed()
    {
        var status = new DiscordStatusFormatter(CultureInfo.GetCultureInfo("en")).CreateStatus(Servers.Where(s => !s.IsOnline).ToList(), DateTime.UtcNow);

        Assert.That(status.Color, Is.EqualTo(DiscordStatusFormatter.OfflineColor));
    }

    /// <summary>
    /// Tests the presence, which counts the players of the online servers.
    /// </summary>
    [Test]
    public void PresenceCountsPlayers()
    {
        var presence = new DiscordStatusFormatter(CultureInfo.GetCultureInfo("de")).CreatePresence(Servers);

        Assert.That(presence, Is.EqualTo("15 Spieler online"));
    }
}
