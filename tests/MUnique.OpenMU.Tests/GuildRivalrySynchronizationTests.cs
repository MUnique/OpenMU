// <copyright file="GuildRivalrySynchronizationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests that published guild relationship changes reach the actual game-server combat caches.
/// </summary>
[TestFixture]
public class GuildRivalrySynchronizationTests : GuildTestBase
{
    private GameServer[] _gameServers = [];
    private uint[] _guildIds = [];

    /// <inheritdoc />
    [SetUp]
    public override async ValueTask SetupAsync()
    {
        await base.SetupAsync().ConfigureAwait(false);
        await this.GuildServer.PlayerEnteredGameAsync(this.GuildMaster.Id, this.GuildMaster.Name, 0).ConfigureAwait(false);
        var guildIds = new List<uint> { await this.GuildServer.GetGuildIdByNameAsync(GuildName).ConfigureAwait(false) };
        using var context = this.PersistenceContextProvider.CreateNewContext();
        foreach (var name in new[] { "Member", "Enemy", "Other" })
        {
            var master = context.CreateNew<BasicModel.Character>();
            master.Name = name + "Master";
            await this.GuildServer.CreateGuildAsync(name, master.Name, master.Id, new byte[16], 0).ConfigureAwait(false);
            guildIds.Add(await this.GuildServer.GetGuildIdByNameAsync(name).ConfigureAwait(false));
        }

        this._guildIds = guildIds.ToArray();
        this._gameServers = [this.CreateGameServer(0), this.CreateGameServer(1)];
        foreach (var server in this._gameServers)
        {
            // The real publisher holds this dictionary, so mutations exercise its actual delivery path.
            this.GameServers[server.Id] = server;
        }
    }

    /// <summary>
    /// Disposes the game contexts without starting a listener or connecting a client.
    /// </summary>
    [TearDown]
    public async ValueTask TearDownAsync()
    {
        foreach (var server in this._gameServers)
        {
            await server.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Hostility created before an alliance must be inherited on join and reconciled on leave or disband.
    /// </summary>
    /// <param name="memberStartsHostility">Whether the joining guild, rather than the master, has the direct hostility.</param>
    /// <param name="disband">Whether the master disbands the alliance instead of removing its member.</param>
    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async ValueTask ExistingHostilityFollowsAllianceMembershipAsync(bool memberStartsHostility, bool disband)
    {
        var master = this._guildIds[0];
        var member = this._guildIds[1];
        var enemy = this._guildIds[2];
        Assert.That(await this.GuildServer.SetHostilityAsync(memberStartsHostility ? member : master, enemy, true).ConfigureAwait(false), Is.True);
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);

        Assert.That(await this.GuildServer.CreateAllianceAsync(master, member).ConfigureAwait(false), Is.EqualTo(AllianceCreationResult.Success));
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);
        foreach (var server in this._gameServers)
        {
            Assert.That(server.Context.AreGuildsRival(master, enemy), Is.True);
            Assert.That(server.Context.AreGuildsRival(member, enemy), Is.True);
        }

        Assert.That(await this.GuildServer.RemoveAllianceAsync(disband ? master : member).ConfigureAwait(false), Is.True);
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);
        foreach (var server in this._gameServers)
        {
            Assert.That(server.Context.AreGuildsRival(master, enemy), Is.EqualTo(!memberStartsHostility));
            Assert.That(server.Context.AreGuildsRival(member, enemy), Is.EqualTo(memberStartsHostility));
        }
    }

    /// <summary>
    /// Reconciliation handles both hostile alliances and preserves independent direct hostility edges.
    /// </summary>
    /// <param name="disband">Whether the first alliance is disbanded instead of removing one member.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async ValueTask BothAlliancesReconcileWithoutLosingDirectHostilityAsync(bool disband)
    {
        var master = this._guildIds[0];
        var member = this._guildIds[1];
        var enemy = this._guildIds[2];
        var other = this._guildIds[3];
        Assert.That(await this.GuildServer.SetHostilityAsync(master, enemy, true).ConfigureAwait(false), Is.True);
        Assert.That(await this.GuildServer.SetHostilityAsync(member, other, true).ConfigureAwait(false), Is.True);
        Assert.That(await this.GuildServer.CreateAllianceAsync(master, member).ConfigureAwait(false), Is.EqualTo(AllianceCreationResult.Success));
        Assert.That(await this.GuildServer.CreateAllianceAsync(enemy, other).ConfigureAwait(false), Is.EqualTo(AllianceCreationResult.Success));
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);

        Assert.That(await this.GuildServer.RemoveAllianceAsync(disband ? master : member).ConfigureAwait(false), Is.True);
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);
        Assert.That(await this.GuildServer.RemoveAllianceAsync(enemy).ConfigureAwait(false), Is.True);
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);
        foreach (var server in this._gameServers)
        {
            Assert.That(server.Context.AreGuildsRival(master, enemy), Is.True);
            Assert.That(server.Context.AreGuildsRival(member, other), Is.True);
            Assert.That(server.Context.AreGuildsRival(master, other), Is.False);
            Assert.That(server.Context.AreGuildsRival(member, enemy), Is.False);
        }
    }

    /// <summary>
    /// Removing the only directly hostile member removes inherited rivalry from the remaining alliance.
    /// </summary>
    [Test]
    public async ValueTask DepartingHostileMemberClearsRemainingAllianceCacheAsync()
    {
        var master = this._guildIds[0];
        var member = this._guildIds[1];
        var enemy = this._guildIds[2];
        var other = this._guildIds[3];
        Assert.That(await this.GuildServer.SetHostilityAsync(member, enemy, true).ConfigureAwait(false), Is.True);
        Assert.That(await this.GuildServer.CreateAllianceAsync(master, member).ConfigureAwait(false), Is.EqualTo(AllianceCreationResult.Success));
        Assert.That(await this.GuildServer.CreateAllianceAsync(master, other).ConfigureAwait(false), Is.EqualTo(AllianceCreationResult.Success));
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);

        Assert.That(await this.GuildServer.RemoveAllianceAsync(member).ConfigureAwait(false), Is.True);
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);
        foreach (var server in this._gameServers)
        {
            Assert.That(server.Context.AreGuildsRival(master, enemy), Is.False);
            Assert.That(server.Context.AreGuildsRival(other, enemy), Is.False);
            Assert.That(server.Context.AreGuildsRival(member, enemy), Is.True);
        }
    }

    /// <summary>
    /// Union takes precedence over a direct hostility and leaving restores the direct rivalry.
    /// </summary>
    [Test]
    public async ValueTask FormerRivalsAreNotCachedAsHostileWhileAlliedAsync()
    {
        var master = this._guildIds[0];
        var member = this._guildIds[1];
        Assert.That(await this.GuildServer.SetHostilityAsync(master, member, true).ConfigureAwait(false), Is.True);
        Assert.That(await this.GuildServer.CreateAllianceAsync(master, member).ConfigureAwait(false), Is.EqualTo(AllianceCreationResult.Success));
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);

        Assert.That(await this.GuildServer.RemoveAllianceAsync(member).ConfigureAwait(false), Is.True);
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);
        foreach (var server in this._gameServers)
        {
            Assert.That(server.Context.AreGuildsRival(master, member), Is.True);
        }
    }

    /// <summary>
    /// Guild deletion also removes alliance membership and must not leave inherited or deleted-guild rivalries cached.
    /// </summary>
    /// <param name="deleteMaster">Whether the alliance master is deleted instead of its member.</param>
    /// <param name="memberStartsHostility">Whether the joining member owns the direct hostility.</param>
    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async ValueTask GuildDeletionReconcilesRivalriesAsync(bool deleteMaster, bool memberStartsHostility)
    {
        var master = this._guildIds[0];
        var member = this._guildIds[1];
        var enemy = this._guildIds[2];
        Assert.That(await this.GuildServer.SetHostilityAsync(memberStartsHostility ? member : master, enemy, true).ConfigureAwait(false), Is.True);
        Assert.That(await this.GuildServer.CreateAllianceAsync(master, member).ConfigureAwait(false), Is.EqualTo(AllianceCreationResult.Success));
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);

        await this.GuildServer.KickMemberAsync(deleteMaster ? master : member, deleteMaster ? this.GuildMaster.Name : "MemberMaster").ConfigureAwait(false);

        Assert.That(await this.GuildServer.GetGuildAsync(deleteMaster ? master : member).ConfigureAwait(false), Is.Null);
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);
    }

    private async ValueTask AssertCachesMatchRelationshipsAsync()
    {
        foreach (var first in this._guildIds)
        {
            foreach (var second in this._guildIds.Where(id => id != first))
            {
                var relationship = await this.GuildServer.GetGuildRelationshipAsync(first, second).ConfigureAwait(false);
                foreach (var server in this._gameServers)
                {
                    Assert.That(server.Context.AreGuildsRival(first, second), Is.EqualTo(relationship == GuildRelationship.Rival),
                        $"Game server {server.Id}: rivalry for guilds {first} and {second} disagrees with the guild server.");
                }
            }
        }
    }

    private GameServer CreateGameServer(byte serverId)
    {
        return new GameServer(
            new BasicModel.GameServerDefinition
            {
                ServerID = serverId,
                GameConfiguration = new BasicModel.GameConfiguration(),
                ServerConfiguration = new BasicModel.GameServerConfiguration(),
            },
            this.GuildServer,
            new Mock<IEventPublisher>().Object,
            new Mock<ILoginServer>().Object,
            this.PersistenceContextProvider,
            new Mock<IFriendServer>().Object,
            NullLoggerFactory.Instance,
            new PlugInManager([], NullLoggerFactory.Instance, null, null),
            new ConfigurationChangeMediator());
    }
}
