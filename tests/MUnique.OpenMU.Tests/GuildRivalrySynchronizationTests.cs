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
    private (Guid Id, string Name, string GuildName)[] _guildMasters = [];

    /// <inheritdoc />
    [SetUp]
    public override async ValueTask SetupAsync()
    {
        await base.SetupAsync().ConfigureAwait(false);
        await this.GuildServer.PlayerEnteredGameAsync(this.GuildMaster.Id, this.GuildMaster.Name, 0).ConfigureAwait(false);
        var guildIds = new List<uint> { await this.GuildServer.GetGuildIdByNameAsync(GuildName).ConfigureAwait(false) };
        var guildMasters = new List<(Guid Id, string Name, string GuildName)> { (this.GuildMaster.Id, this.GuildMaster.Name, GuildName) };
        using var context = this.PersistenceContextProvider.CreateNewContext();
        foreach (var name in new[] { "Member", "Enemy", "Other" })
        {
            var master = context.CreateNew<BasicModel.Character>();
            master.Name = name + "Master";
            await this.GuildServer.CreateGuildAsync(name, master.Name, master.Id, new byte[16], 0).ConfigureAwait(false);
            guildIds.Add(await this.GuildServer.GetGuildIdByNameAsync(name).ConfigureAwait(false));
            guildMasters.Add((master.Id, master.Name, name));
        }

        this._guildIds = guildIds.ToArray();
        this._guildMasters = guildMasters.ToArray();
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

    /// <summary>
    /// An offline hostile guild inherits membership changes when reloaded under a new runtime identifier.
    /// </summary>
    /// <param name="throughLogin">Whether the guild is reloaded by login instead of persistent-id lookup.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async ValueTask OfflineEnemyIsReconciledAfterAllianceJoinAndReconnectAsync(bool throughLogin)
    {
        var master = this._guildIds[0];
        var member = this._guildIds[1];
        Assert.That(await this.GuildServer.SetHostilityAsync(master, this._guildIds[2], true).ConfigureAwait(false), Is.True);
        var offlineGuild = await this.UnloadGuildAsync(2).ConfigureAwait(false);

        Assert.That(await this.GuildServer.CreateAllianceAsync(master, member).ConfigureAwait(false), Is.EqualTo(AllianceCreationResult.Success));

        await this.ReloadGuildAsync(2, offlineGuild.PersistentId, throughLogin).ConfigureAwait(false);
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);
        foreach (var server in this._gameServers)
        {
            Assert.That(server.Context.AreGuildsRival(master, this._guildIds[2]), Is.True);
            Assert.That(server.Context.AreGuildsRival(member, this._guildIds[2]), Is.True);
            Assert.That(server.Context.AreGuildsRival(master, offlineGuild.RuntimeId), Is.False);
            Assert.That(server.Context.AreGuildsRival(member, offlineGuild.RuntimeId), Is.False);
        }

        var secondLogout = await this.UnloadGuildAsync(2).ConfigureAwait(false);
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);
        await this.ReloadGuildAsync(2, secondLogout.PersistentId, throughLogin).ConfigureAwait(false);
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);
        foreach (var server in this._gameServers)
        {
            Assert.That(server.Context.AreGuildsRival(master, secondLogout.RuntimeId), Is.False);
            Assert.That(server.Context.AreGuildsRival(member, secondLogout.RuntimeId), Is.False);
        }
    }

    /// <summary>
    /// Removing inherited hostility while the enemy is offline must not restore it on reconnect.
    /// </summary>
    /// <param name="disband">Whether to disband the alliance instead of removing a member.</param>
    /// <param name="throughLogin">Whether to reload through login instead of persistent-id lookup.</param>
    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async ValueTask OfflineEnemyDoesNotRestoreRemovedAllianceRivalryAsync(bool disband, bool throughLogin)
    {
        var master = this._guildIds[0];
        var member = this._guildIds[1];
        Assert.That(await this.GuildServer.CreateAllianceAsync(master, member).ConfigureAwait(false), Is.EqualTo(AllianceCreationResult.Success));
        Assert.That(await this.GuildServer.SetHostilityAsync(master, this._guildIds[2], true).ConfigureAwait(false), Is.True);
        var offlineGuild = await this.UnloadGuildAsync(2).ConfigureAwait(false);

        Assert.That(await this.GuildServer.RemoveAllianceAsync(disband ? master : member).ConfigureAwait(false), Is.True);

        await this.ReloadGuildAsync(2, offlineGuild.PersistentId, throughLogin).ConfigureAwait(false);
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);
        foreach (var server in this._gameServers)
        {
            Assert.That(server.Context.AreGuildsRival(master, this._guildIds[2]), Is.True);
            Assert.That(server.Context.AreGuildsRival(member, this._guildIds[2]), Is.False);
            Assert.That(server.Context.AreGuildsRival(master, offlineGuild.RuntimeId), Is.False);
            Assert.That(server.Context.AreGuildsRival(member, offlineGuild.RuntimeId), Is.False);
        }
    }

    /// <summary>
    /// Persisted hostility is restored regardless of the reconnect order when both guilds unloaded.
    /// </summary>
    /// <param name="enemyFirst">Whether to reconnect the enemy before the declaring guild.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async ValueTask BothHostileGuildsCanUnloadAndReconnectAsync(bool enemyFirst)
    {
        Assert.That(await this.GuildServer.SetHostilityAsync(this._guildIds[0], this._guildIds[2], true).ConfigureAwait(false), Is.True);
        var declaringGuild = await this.UnloadGuildAsync(0).ConfigureAwait(false);
        var enemyGuild = await this.UnloadGuildAsync(2).ConfigureAwait(false);
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);

        await this.ReloadGuildAsync(enemyFirst ? 2 : 0, enemyFirst ? enemyGuild.PersistentId : declaringGuild.PersistentId, true).ConfigureAwait(false);
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);
        await this.ReloadGuildAsync(enemyFirst ? 0 : 2, enemyFirst ? declaringGuild.PersistentId : enemyGuild.PersistentId, true).ConfigureAwait(false);
        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);
        foreach (var server in this._gameServers)
        {
            Assert.That(server.Context.AreGuildsRival(this._guildIds[0], this._guildIds[2]), Is.True);
            Assert.That(server.Context.AreGuildsRival(declaringGuild.RuntimeId, enemyGuild.RuntimeId), Is.False);
        }
    }

    /// <summary>
    /// Cold-loading a persisted alliance reconciles every guild materialized with the requested member.
    /// </summary>
    /// <param name="loadMemberFirst">Whether to load through an allied member instead of its master.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async ValueTask ColdAllianceLoadRestoresPersistedRivalriesAsync(bool loadMemberFirst)
    {
        Assert.That(await this.GuildServer.CreateAllianceAsync(this._guildIds[0], this._guildIds[1]).ConfigureAwait(false), Is.EqualTo(AllianceCreationResult.Success));
        Assert.That(await this.GuildServer.SetHostilityAsync(this._guildIds[0], this._guildIds[2], true).ConfigureAwait(false), Is.True);
        var persistentGuildIds = new List<Guid>();
        foreach (var runtimeId in this._guildIds)
        {
            var persistentId = await this.GuildServer.GetPersistentGuildIdAsync(runtimeId).ConfigureAwait(false);
            Assert.That(persistentId, Is.Not.Null);
            persistentGuildIds.Add(persistentId!.Value);
        }

        foreach (var server in this._gameServers)
        {
            await server.DisposeAsync().ConfigureAwait(false);
        }

        this._gameServers = [];
        this.GuildServer = new MUnique.OpenMU.GuildServer.GuildServer(
            new MUnique.OpenMU.GuildServer.GuildChangeToGameServerPublisher(this.GameServers),
            this.PersistenceContextProvider,
            NullLogger<MUnique.OpenMU.GuildServer.GuildServer>.Instance);
        this._gameServers = [this.CreateGameServer(0), this.CreateGameServer(1)];
        foreach (var server in this._gameServers)
        {
            this.GameServers[server.Id] = server;
        }

        this._guildIds[2] = await this.GuildServer.GetGuildIdAsync(persistentGuildIds[2]).ConfigureAwait(false);
        var requestedIndex = loadMemberFirst ? 1 : 0;
        this._guildIds[requestedIndex] = await this.GuildServer.GetGuildIdAsync(persistentGuildIds[requestedIndex]).ConfigureAwait(false);
        var collateralIndex = loadMemberFirst ? 0 : 1;
        this._guildIds[collateralIndex] = await this.GuildServer.GetGuildIdByNameAsync(this._guildMasters[collateralIndex].GuildName).ConfigureAwait(false);
        Assert.That(this._guildIds[collateralIndex], Is.Not.Zero, "The other alliance guild must have been materialized by the same load.");
        this._guildIds[3] = await this.GuildServer.GetGuildIdAsync(persistentGuildIds[3]).ConfigureAwait(false);

        await this.AssertCachesMatchRelationshipsAsync().ConfigureAwait(false);
        foreach (var server in this._gameServers)
        {
            Assert.That(server.Context.AreGuildsRival(this._guildIds[0], this._guildIds[2]), Is.True);
            Assert.That(server.Context.AreGuildsRival(this._guildIds[1], this._guildIds[2]), Is.True);
        }
    }

    private async ValueTask<(uint RuntimeId, Guid PersistentId)> UnloadGuildAsync(int index)
    {
        var runtimeId = this._guildIds[index];
        var persistentId = await this.GuildServer.GetPersistentGuildIdAsync(runtimeId).ConfigureAwait(false);
        Assert.That(persistentId, Is.Not.Null);
        await this.GuildServer.GuildMemberLeftGameAsync(runtimeId, this._guildMasters[index].Id, 0).ConfigureAwait(false);
        Assert.That(await this.GuildServer.GetGuildAsync(runtimeId).ConfigureAwait(false), Is.Null);
        return (runtimeId, persistentId!.Value);
    }

    private async ValueTask ReloadGuildAsync(int index, Guid persistentId, bool throughLogin)
    {
        var previousRuntimeId = this._guildIds[index];
        if (throughLogin)
        {
            await this.GuildServer.PlayerEnteredGameAsync(this._guildMasters[index].Id, this._guildMasters[index].Name, 0).ConfigureAwait(false);
            this._guildIds[index] = await this.GuildServer.GetGuildIdByNameAsync(this._guildMasters[index].GuildName).ConfigureAwait(false);
        }
        else
        {
            this._guildIds[index] = await this.GuildServer.GetGuildIdAsync(persistentId).ConfigureAwait(false);
        }

        Assert.That(this._guildIds[index], Is.Not.Zero.And.Not.EqualTo(previousRuntimeId));
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
