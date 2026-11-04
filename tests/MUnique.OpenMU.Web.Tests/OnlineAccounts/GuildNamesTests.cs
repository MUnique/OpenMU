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
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();
        var guildServer = new Mock<IGuildServer>();
        guildServer.Setup(g => g.GetGuildAsync(1)).Returns(new ValueTask<Guild?>(new Guild { Name = "Knights" }));
        guildServer.Setup(g => g.GetGuildAsync(2)).Returns(new ValueTask<Guild?>(new Guild { Name = "Mages" }));
        guildServer.Setup(g => g.GetPersistentGuildIdAsync(1)).Returns(new ValueTask<Guid?>(firstId));
        guildServer.Setup(g => g.GetPersistentGuildIdAsync(2)).Returns(new ValueTask<Guid?>(secondId));

        var result = await GuildNames.ResolveAsync(guildServer.Object, new uint[] { 1, 2, 1 });

        Assert.That(result, Is.EqualTo(new Dictionary<uint, GuildNames.GuildInfo>
        {
            [1] = new GuildNames.GuildInfo("Knights", firstId),
            [2] = new GuildNames.GuildInfo("Mages", secondId),
        }));
        guildServer.Verify(g => g.GetGuildAsync(It.IsAny<uint>()), Times.Exactly(2));
        guildServer.Verify(g => g.GetPersistentGuildIdAsync(It.IsAny<uint>()), Times.Exactly(2));
    }

    /// <summary>
    /// Unknown guilds are absent, so the row renders as guild-less.
    /// </summary>
    [Test]
    public async Task UnknownGuild_IsAbsent()
    {
        var guildServer = new Mock<IGuildServer>();
        guildServer.Setup(g => g.GetGuildAsync(It.IsAny<uint>())).Returns(new ValueTask<Guild?>(result: null));
        guildServer.Setup(g => g.GetPersistentGuildIdAsync(It.IsAny<uint>())).Returns(new ValueTask<Guid?>(result: null));

        var result = await GuildNames.ResolveAsync(guildServer.Object, new uint[] { 7 });

        Assert.That(result, Is.Empty);
    }

    /// <summary>
    /// A single failed lookup doesn't fail the whole table.
    /// </summary>
    [Test]
    public async Task FailedLookup_DoesNotFailOthers()
    {
        var secondId = Guid.NewGuid();
        var guildServer = new Mock<IGuildServer>();
        guildServer.Setup(g => g.GetGuildAsync(1)).Throws(new InvalidOperationException("guild server down"));
        guildServer.Setup(g => g.GetPersistentGuildIdAsync(1)).Throws(new InvalidOperationException("guild server down"));
        guildServer.Setup(g => g.GetGuildAsync(2)).Returns(new ValueTask<Guild?>(new Guild { Name = "Mages" }));
        guildServer.Setup(g => g.GetPersistentGuildIdAsync(2)).Returns(new ValueTask<Guid?>(secondId));

        var result = await GuildNames.ResolveAsync(guildServer.Object, new uint[] { 1, 2 });

        Assert.That(result, Is.EqualTo(new Dictionary<uint, GuildNames.GuildInfo>
        {
            [2] = new GuildNames.GuildInfo("Mages", secondId),
        }));
    }

    /// <summary>
    /// A partially failed guild still resolves the working half, so the row keeps its name or link.
    /// </summary>
    [Test]
    public async Task PartiallyFailedGuild_ResolvesWorkingHalf()
    {
        var guildServer = new Mock<IGuildServer>();
        guildServer.Setup(g => g.GetGuildAsync(1)).Throws(new InvalidOperationException("guild server down"));
        var persistentId = Guid.NewGuid();
        guildServer.Setup(g => g.GetPersistentGuildIdAsync(1)).Returns(new ValueTask<Guid?>(persistentId));

        var result = await GuildNames.ResolveAsync(guildServer.Object, new uint[] { 1 });

        Assert.That(result, Is.EqualTo(new Dictionary<uint, GuildNames.GuildInfo>
        {
            [1] = new GuildNames.GuildInfo(null, persistentId),
        }));
    }

    /// <summary>
    /// Empty persistent identifiers are treated as unknown, so no dead link is rendered.
    /// </summary>
    [Test]
    public async Task EmptyPersistentId_IsAbsent()
    {
        var guildServer = new Mock<IGuildServer>();
        guildServer.Setup(g => g.GetGuildAsync(1)).Returns(new ValueTask<Guild?>(result: null));
        guildServer.Setup(g => g.GetPersistentGuildIdAsync(1)).Returns(new ValueTask<Guid?>(Guid.Empty));

        var result = await GuildNames.ResolveAsync(guildServer.Object, new uint[] { 1 });

        Assert.That(result, Is.Empty);
    }
}
