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

    /// <summary>
    /// Without a guild server there are no persistent identifiers to resolve.
    /// </summary>
    [Test]
    public async Task PersistentIds_WithoutGuildServer_ReturnsEmpty()
    {
        var result = await GuildNames.ResolvePersistentIdsAsync(null, new uint[] { 1, 2 });

        Assert.That(result, Is.Empty);
    }

    /// <summary>
    /// Each distinct guild resolves its persistent identifier exactly once.
    /// </summary>
    [Test]
    public async Task PersistentIds_DistinctGuilds_LookedUpOnce()
    {
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();
        var guildServer = new Mock<IGuildServer>();
        guildServer.Setup(g => g.GetPersistentGuildIdAsync(1)).Returns(new ValueTask<Guid?>(firstId));
        guildServer.Setup(g => g.GetPersistentGuildIdAsync(2)).Returns(new ValueTask<Guid?>(secondId));

        var result = await GuildNames.ResolvePersistentIdsAsync(guildServer.Object, new uint[] { 1, 2, 1 });

        Assert.That(result, Is.EqualTo(new Dictionary<uint, Guid> { [1] = firstId, [2] = secondId }));
        guildServer.Verify(g => g.GetPersistentGuildIdAsync(It.IsAny<uint>()), Times.Exactly(2));
    }

    /// <summary>
    /// Unknown guilds and failed lookups are absent, so the row renders without a link.
    /// </summary>
    [Test]
    public async Task PersistentIds_UnknownOrFailedGuilds_AreAbsent()
    {
        var guildServer = new Mock<IGuildServer>();
        guildServer.Setup(g => g.GetPersistentGuildIdAsync(1)).Returns(new ValueTask<Guid?>(result: null));
        guildServer.Setup(g => g.GetPersistentGuildIdAsync(2)).Returns(new ValueTask<Guid?>(Guid.Empty));
        guildServer.Setup(g => g.GetPersistentGuildIdAsync(3)).Throws(new InvalidOperationException("guild server down"));

        var result = await GuildNames.ResolvePersistentIdsAsync(guildServer.Object, new uint[] { 1, 2, 3 });

        Assert.That(result, Is.Empty);
    }
}
