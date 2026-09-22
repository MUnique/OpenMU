// <copyright file="GuildNamesTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.OnlineAccounts;

using Moq;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests for the bulk guild name resolution.
/// </summary>
[TestFixture]
public class GuildNamesTests
{
    /// <summary>
    /// Without a guild server (distributed deployment) there is nothing to resolve.
    /// </summary>
    [Test]
    public async Task WithoutGuildServer_ReturnsEmpty()
    {
        var result = await GuildNames.ResolveAsync(null, new uint[] { 1, 2 });

        Assert.That(result, Is.Empty);
    }

    /// <summary>
    /// Each distinct guild is looked up exactly once, no matter how many members it has.
    /// </summary>
    [Test]
    public async Task DistinctGuilds_LookedUpOnce()
    {
        var guildServer = new Mock<IGuildServer>();
        guildServer.Setup(g => g.GetGuildAsync(1)).Returns(new ValueTask<Guild?>(new Guild { Name = "Knights" }));
        guildServer.Setup(g => g.GetGuildAsync(2)).Returns(new ValueTask<Guild?>(new Guild { Name = "Mages" }));

        var result = await GuildNames.ResolveAsync(guildServer.Object, new uint[] { 1, 2, 1 });

        Assert.That(result, Is.EqualTo(new Dictionary<uint, string> { [1] = "Knights", [2] = "Mages" }));
        guildServer.Verify(g => g.GetGuildAsync(It.IsAny<uint>()), Times.Exactly(2));
    }

    /// <summary>
    /// Unknown guilds are absent, so the row renders as guild-less.
    /// </summary>
    [Test]
    public async Task UnknownGuild_IsAbsent()
    {
        var guildServer = new Mock<IGuildServer>();
        guildServer.Setup(g => g.GetGuildAsync(It.IsAny<uint>())).Returns(new ValueTask<Guild?>(result: null));

        var result = await GuildNames.ResolveAsync(guildServer.Object, new uint[] { 7 });

        Assert.That(result, Is.Empty);
    }

    /// <summary>
    /// A single failed lookup doesn't fail the whole table.
    /// </summary>
    [Test]
    public async Task FailedLookup_DoesNotFailOthers()
    {
        var guildServer = new Mock<IGuildServer>();
        guildServer.Setup(g => g.GetGuildAsync(1)).Throws(new InvalidOperationException("guild server down"));
        guildServer.Setup(g => g.GetGuildAsync(2)).Returns(new ValueTask<Guild?>(new Guild { Name = "Mages" }));

        var result = await GuildNames.ResolveAsync(guildServer.Object, new uint[] { 1, 2 });

        Assert.That(result, Is.EqualTo(new Dictionary<uint, string> { [2] = "Mages" }));
    }
}
