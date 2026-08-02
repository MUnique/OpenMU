// <copyright file="GuildAllianceTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Unit tests for guild alliance and hostility logic in <see cref="MUnique.OpenMU.GuildServer.GuildServer"/>.
/// </summary>
[TestFixture]
public class GuildAllianceTest : GuildTestBase
{
    private const string SecondGuildName = "SecondGuild";
    private const string ThirdGuildName = "ThirdGuild";

    private Character _secondGuildMaster = null!;
    private Character _thirdGuildMaster = null!;
    private uint _firstGuildId;
    private uint _secondGuildId;
    private uint _thirdGuildId;

    /// <inheritdoc />
    [SetUp]
    public override async ValueTask SetupAsync()
    {
        await base.SetupAsync().ConfigureAwait(false);

        var context = this.PersistenceContextProvider.CreateNewContext();

        this._secondGuildMaster = context.CreateNew<Character>();
        this._secondGuildMaster.Name = "SecondMaster";

        this._thirdGuildMaster = context.CreateNew<Character>();
        this._thirdGuildMaster.Name = "ThirdMaster";

        await this.GuildServer.CreateGuildAsync(SecondGuildName, this._secondGuildMaster.Name, this._secondGuildMaster.Id, new byte[16], 0).ConfigureAwait(false);
        await this.GuildServer.CreateGuildAsync(ThirdGuildName, this._thirdGuildMaster.Name, this._thirdGuildMaster.Id, new byte[16], 0).ConfigureAwait(false);

        // Bring the first guild master back online (base.SetupAsync takes them offline)
        // so that the first guild is in the in-memory dictionary and GetGuildIdByNameAsync can find it.
        await this.GuildServer.PlayerEnteredGameAsync(this.GuildMaster.Id, this.GuildMaster.Name, 0).ConfigureAwait(false);

        this._firstGuildId = await this.GuildServer.GetGuildIdByNameAsync(GuildName).ConfigureAwait(false);
        this._secondGuildId = await this.GuildServer.GetGuildIdByNameAsync(SecondGuildName).ConfigureAwait(false);
        this._thirdGuildId = await this.GuildServer.GetGuildIdByNameAsync(ThirdGuildName).ConfigureAwait(false);
    }

    // -------------------------------------------------------------------------
    // CreateAllianceAsync
    // -------------------------------------------------------------------------

    /// <summary>
    /// Two online guilds can successfully form an alliance.
    /// </summary>
    [Test]
    public async ValueTask CreateAlliance_Success()
    {
        var result = await this.GuildServer.CreateAllianceAsync(this._firstGuildId, this._secondGuildId).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(AllianceCreationResult.Success));
    }

    /// <summary>
    /// A guild that is already in an alliance cannot join another one.
    /// </summary>
    [Test]
    public async ValueTask CreateAlliance_TargetAlreadyInAlliance_Fails()
    {
        await this.GuildServer.CreateAllianceAsync(this._firstGuildId, this._secondGuildId).ConfigureAwait(false);

        // Try to add the second guild (already in an alliance) to the third guild
        var result = await this.GuildServer.CreateAllianceAsync(this._thirdGuildId, this._secondGuildId).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(AllianceCreationResult.TargetGuildAlreadyInAlliance));
    }

    /// <summary>
    /// The guild server does not enforce a maximum alliance size — that limit
    /// is now applied at the action layer (<c>GuildRelationshipChangeAction</c>)
    /// using the <c>Stats.MaximumAllianceSize</c> player attribute.
    /// This test verifies that the server allows building an alliance larger than 5
    /// (the old hard-coded constant).
    /// </summary>
    [Test]
    public async ValueTask CreateAlliance_ServerDoesNotEnforceMaxSize()
    {
        // Add 6 guilds beyond the old hard-coded limit of 5 to confirm no server limit
        const int beyondOldLimit = 6;
        for (var i = 0; i < beyondOldLimit - 1; i++)
        {
            var memberName = $"FillGuildMaster{i}";
            var fillContext = this.PersistenceContextProvider.CreateNewContext();
            var master = fillContext.CreateNew<Character>();
            master.Name = memberName;
            var guildName = $"FillGuild{i}";
            await this.GuildServer.CreateGuildAsync(guildName, memberName, master.Id, new byte[16], 0).ConfigureAwait(false);
            await this.GuildServer.PlayerEnteredGameAsync(master.Id, memberName, 0).ConfigureAwait(false);
            var fillGuildId = await this.GuildServer.GetGuildIdByNameAsync(guildName).ConfigureAwait(false);
            await this.GuildServer.CreateAllianceAsync(this._firstGuildId, fillGuildId).ConfigureAwait(false);
        }

        // Adding the sixth guild should still succeed — no server-side hard limit
        var result = await this.GuildServer.CreateAllianceAsync(this._firstGuildId, this._secondGuildId).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(AllianceCreationResult.Success));
    }

    // -------------------------------------------------------------------------
    // IsAllianceMasterAsync
    // -------------------------------------------------------------------------

    /// <summary>
    /// The guild that initiated the alliance is identified as the alliance master.
    /// </summary>
    [Test]
    public async ValueTask IsAllianceMaster_MasterGuild_ReturnsTrue()
    {
        await this.GuildServer.CreateAllianceAsync(this._firstGuildId, this._secondGuildId).ConfigureAwait(false);

        var isMaster = await this.GuildServer.IsAllianceMasterAsync(this._firstGuildId).ConfigureAwait(false);

        Assert.That(isMaster, Is.True);
    }

    /// <summary>
    /// A member guild is not identified as the alliance master.
    /// </summary>
    [Test]
    public async ValueTask IsAllianceMaster_MemberGuild_ReturnsFalse()
    {
        await this.GuildServer.CreateAllianceAsync(this._firstGuildId, this._secondGuildId).ConfigureAwait(false);

        var isMaster = await this.GuildServer.IsAllianceMasterAsync(this._secondGuildId).ConfigureAwait(false);

        Assert.That(isMaster, Is.False);
    }

    // -------------------------------------------------------------------------
    // GetAllianceGuildsAsync
    // -------------------------------------------------------------------------

    /// <summary>
    /// GetAllianceGuildsAsync returns all guilds that are in the alliance.
    /// </summary>
    [Test]
    public async ValueTask GetAllianceGuilds_ReturnsAllMembers()
    {
        await this.GuildServer.CreateAllianceAsync(this._firstGuildId, this._secondGuildId).ConfigureAwait(false);
        await this.GuildServer.CreateAllianceAsync(this._firstGuildId, this._thirdGuildId).ConfigureAwait(false);

        var guilds = await this.GuildServer.GetAllianceGuildsAsync(this._firstGuildId).ConfigureAwait(false);

        Assert.That(guilds.Count, Is.EqualTo(3));
        Assert.That(guilds.Select(g => g.Id), Is.EquivalentTo(new[] { this._firstGuildId, this._secondGuildId, this._thirdGuildId }));
    }

    /// <summary>
    /// GetAllianceGuildsAsync returns an empty list when the guild has no alliance.
    /// </summary>
    [Test]
    public async ValueTask GetAllianceGuilds_NoAlliance_ReturnsEmpty()
    {
        var guilds = await this.GuildServer.GetAllianceGuildsAsync(this._firstGuildId).ConfigureAwait(false);

        Assert.That(guilds, Is.Empty);
    }

    // -------------------------------------------------------------------------
    // RemoveAllianceGuildAsync
    // -------------------------------------------------------------------------

    /// <summary>
    /// The alliance master can successfully remove a member guild.
    /// </summary>
    [Test]
    public async ValueTask RemoveAllianceGuild_Member_Success()
    {
        await this.GuildServer.CreateAllianceAsync(this._firstGuildId, this._secondGuildId).ConfigureAwait(false);
        await this.GuildServer.CreateAllianceAsync(this._firstGuildId, this._thirdGuildId).ConfigureAwait(false);

        var removed = await this.GuildServer.RemoveAllianceAsync(this._secondGuildId).ConfigureAwait(false);
        var guilds = await this.GuildServer.GetAllianceGuildsAsync(this._firstGuildId).ConfigureAwait(false);

        Assert.That(removed, Is.True);
        Assert.That(guilds.Select(g => g.Id), Does.Not.Contain(this._secondGuildId));
    }

    /// <summary>
    /// The alliance master can successfully remove a member guild.
    /// </summary>
    [Test]
    public async ValueTask RemoveAllianceGuild_Master_Disbands_Success()
    {
        await this.GuildServer.CreateAllianceAsync(this._firstGuildId, this._secondGuildId).ConfigureAwait(false);
        await this.GuildServer.CreateAllianceAsync(this._firstGuildId, this._thirdGuildId).ConfigureAwait(false);

        var removed = await this.GuildServer.RemoveAllianceAsync(this._firstGuildId).ConfigureAwait(false);
        var guilds = await this.GuildServer.GetAllianceGuildsAsync(this._firstGuildId).ConfigureAwait(false);

        Assert.That(removed, Is.True);
        Assert.That(guilds, Is.Empty);
    }

    // -------------------------------------------------------------------------
    // SetHostilityAsync / GetGuildRelationshipAsync
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creating hostility between two guilds yields a Rival relationship.
    /// </summary>
    [Test]
    public async ValueTask SetHostility_Create_ReturnsRival()
    {
        var success = await this.GuildServer.SetHostilityAsync(this._firstGuildId, this._secondGuildId, true).ConfigureAwait(false);
        var relationship = await this.GuildServer.GetGuildRelationshipAsync(this._firstGuildId, this._secondGuildId).ConfigureAwait(false);

        Assert.That(success, Is.True);
        Assert.That(relationship, Is.EqualTo(GuildRelationship.Rival));
    }

    /// <summary>
    /// Cancelling a hostility between two solo guilds (no alliances) yields no relationship.
    /// </summary>
    [Test]
    public async ValueTask SetHostility_Cancel_ReturnsNone()
    {
        await this.GuildServer.SetHostilityAsync(this._firstGuildId, this._secondGuildId, true).ConfigureAwait(false);
        await this.GuildServer.SetHostilityAsync(this._firstGuildId, this._secondGuildId, false).ConfigureAwait(false);

        var relationship = await this.GuildServer.GetGuildRelationshipAsync(this._firstGuildId, this._secondGuildId).ConfigureAwait(false);

        Assert.That(relationship, Is.EqualTo(GuildRelationship.None));
    }

    /// <summary>
    /// Two guilds in the same alliance have a Union relationship.
    /// </summary>
    [Test]
    public async ValueTask GetGuildRelationship_SameAlliance_ReturnsUnion()
    {
        await this.GuildServer.CreateAllianceAsync(this._firstGuildId, this._secondGuildId).ConfigureAwait(false);

        var relationship = await this.GuildServer.GetGuildRelationshipAsync(this._firstGuildId, this._secondGuildId).ConfigureAwait(false);

        Assert.That(relationship, Is.EqualTo(GuildRelationship.Union));
    }

    /// <summary>
    /// Two guilds with no shared alliance and no hostility have no relationship.
    /// </summary>
    [Test]
    public async ValueTask GetGuildRelationship_NoRelation_ReturnsNone()
    {
        var relationship = await this.GuildServer.GetGuildRelationshipAsync(this._firstGuildId, this._secondGuildId).ConfigureAwait(false);

        Assert.That(relationship, Is.EqualTo(GuildRelationship.None));
    }

    // -------------------------------------------------------------------------
    // SetHostilityAsync — AreAlliancesStillHostile guard (key bug-fix scenario)
    // -------------------------------------------------------------------------

    /// <summary>
    /// When A↔X and B↔Y hostilities exist between two alliances, cancelling A↔X
    /// must NOT notify game servers to remove all rival pairs, because B↔Y still
    /// makes the alliances hostile.
    ///
    /// Alliance A: guilds 1 and 2.  Alliance X: guild 3.
    /// After SetHostility(1, 3, false) the guild-2 ↔ guild-3 link still exists.
    /// </summary>
    [Test]
    public async ValueTask SetHostility_Cancel_WithRemainingCrossAllianceHostility_DoesNotNotifyRemoval()
    {
        // Build Alliance A: guilds 1 and 2
        await this.GuildServer.CreateAllianceAsync(this._firstGuildId, this._secondGuildId).ConfigureAwait(false);

        // Hostility A↔X: guild 1 ↔ guild 3
        await this.GuildServer.SetHostilityAsync(this._firstGuildId, this._thirdGuildId, true).ConfigureAwait(false);
        // Hostility B↔Y: guild 2 ↔ guild 3
        await this.GuildServer.SetHostilityAsync(this._secondGuildId, this._thirdGuildId, true).ConfigureAwait(false);

        // Reset the call counts recorded during the two SetHostility(create) calls
        this.GameServer0.Invocations.Clear();
        this.GameServer1.Invocations.Clear();

        // Cancel only A↔X (guild 1 ↔ guild 3)
        await this.GuildServer.SetHostilityAsync(this._firstGuildId, this._thirdGuildId, false).ConfigureAwait(false);

        // Game servers must NOT receive a removal notification because B↔Y (guild 2 ↔ guild 3)
        // still makes all alliance members rivals.
        this.GameServer0.Verify(
            gs => gs.GuildHostilityChangedAsync(
                It.IsAny<uint>(),
                It.IsAny<IReadOnlyList<uint>>(),
                It.IsAny<uint>(),
                It.IsAny<IReadOnlyList<uint>>(),
                false),
            Times.Never);
    }

    /// <summary>
    /// When the last remaining cross-alliance hostility is cancelled, game servers
    /// ARE notified so they can remove the rival pairs from their caches.
    /// </summary>
    [Test]
    public async ValueTask SetHostility_Cancel_LastHostility_NotifiesRemoval()
    {
        // Single hostility between two standalone guilds
        await this.GuildServer.SetHostilityAsync(this._firstGuildId, this._secondGuildId, true).ConfigureAwait(false);

        this.GameServer0.Invocations.Clear();
        this.GameServer1.Invocations.Clear();

        await this.GuildServer.SetHostilityAsync(this._firstGuildId, this._secondGuildId, false).ConfigureAwait(false);

        // Game servers must be notified that all hostility ended
        this.GameServer0.Verify(
            gs => gs.GuildHostilityChangedAsync(
                It.IsAny<uint>(),
                It.IsAny<IReadOnlyList<uint>>(),
                It.IsAny<uint>(),
                It.IsAny<IReadOnlyList<uint>>(),
                false),
            Times.Once);
    }

    /// <inheritdoc />
    protected override void SetupGameServer(Mock<IGameServer> gameServer)
    {
        base.SetupGameServer(gameServer);
        gameServer.Setup(gs => gs.GuildHostilityChangedAsync(
                It.IsAny<uint>(),
                It.IsAny<IReadOnlyList<uint>>(),
                It.IsAny<uint>(),
                It.IsAny<IReadOnlyList<uint>>(),
                It.IsAny<bool>()))
            .Returns(ValueTask.CompletedTask);
    }
}

/// <summary>
/// Unit tests for the rival guild cache in <see cref="GameServerContext"/>.
/// </summary>
[TestFixture]
public class RivalGuildCacheTest
{
    private GameServerContext _gameServerContext = null!;

    /// <summary>
    /// Sets up a minimal <see cref="GameServerContext"/> for each test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._gameServerContext = CreateMinimalGameServerContext();
    }

    /// <summary>
    /// After adding a hostility the two guilds are reported as rivals.
    /// </summary>
    [Test]
    public void UpdateGuildHostility_Create_GuildsAreRivals()
    {
        const uint guildA = 1;
        const uint guildB = 2;

        this._gameServerContext.UpdateGuildHostility(guildA, [guildA], guildB, [guildB], true);

        Assert.That(this._gameServerContext.AreGuildsRival(guildA, guildB), Is.True);
    }

    /// <summary>
    /// After removing a hostility the two guilds are no longer rivals.
    /// </summary>
    [Test]
    public void UpdateGuildHostility_Remove_GuildsAreNoLongerRivals()
    {
        const uint guildA = 1;
        const uint guildB = 2;

        this._gameServerContext.UpdateGuildHostility(guildA, [guildA], guildB, [guildB], true);
        this._gameServerContext.UpdateGuildHostility(guildA, [guildA], guildB, [guildB], false);

        Assert.That(this._gameServerContext.AreGuildsRival(guildA, guildB), Is.False);
    }

    /// <summary>
    /// Guild ID order does not matter: (A, B) and (B, A) both return the same result.
    /// </summary>
    [Test]
    public void AreGuildsRival_IdOrderIsNormalized()
    {
        const uint guildA = 5;
        const uint guildB = 3; // B < A intentionally

        this._gameServerContext.UpdateGuildHostility(guildA, [guildA], guildB, [guildB], true);

        Assert.That(this._gameServerContext.AreGuildsRival(guildA, guildB), Is.True);
        Assert.That(this._gameServerContext.AreGuildsRival(guildB, guildA), Is.True);
    }

    /// <summary>
    /// When alliances are expanded, every cross-alliance pair is cached.
    /// Alliance A = {1, 2}, Alliance X = {3, 4}.
    /// All four cross-pairs (1↔3, 1↔4, 2↔3, 2↔4) should be rivals.
    /// </summary>
    [Test]
    public void UpdateGuildHostility_AllianceExpansion_AllPairsAreCached()
    {
        uint[] allianceA = [1, 2];
        uint[] allianceX = [3, 4];

        this._gameServerContext.UpdateGuildHostility(1, allianceA, 3, allianceX, true);

        foreach (var idA in allianceA)
        {
            foreach (var idX in allianceX)
            {
                Assert.That(this._gameServerContext.AreGuildsRival(idA, idX), Is.True,
                    $"Expected guilds {idA} and {idX} to be rivals.");
            }
        }
    }

    /// <summary>
    /// Guilds that are not in the rival cache are not considered rivals.
    /// </summary>
    [Test]
    public void AreGuildsRival_UnrelatedGuilds_ReturnsFalse()
    {
        Assert.That(this._gameServerContext.AreGuildsRival(100, 200), Is.False);
    }

    private static GameServerContext CreateMinimalGameServerContext()
    {
        var persistenceProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = new BasicModel.GameConfiguration();
        gameConfiguration.Maps.Add(new BasicModel.GameMapDefinition());
        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var ctx = new GameServerContext(
            new BasicModel.GameServerDefinition
            {
                GameConfiguration = gameConfiguration,
                ServerConfiguration = new BasicModel.GameServerConfiguration(),
            },
            new Mock<IGuildServer>().Object,
            new Mock<IEventPublisher>().Object,
            new Mock<ILoginServer>().Object,
            new Mock<IFriendServer>().Object,
            persistenceProvider,
            mapInitializer,
            new NullLoggerFactory(),
            new PlugInManager([], new NullLoggerFactory(), null, null),
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = ctx.PlugInManager;
        mapInitializer.PathFinderPool = ctx.PathFinderPool;
        return ctx;
    }
}

/// <summary>
/// Tests that rival guild members can fight each other without PK consequences and
/// without triggering self-defense.
/// </summary>
[TestFixture]
public class RivalGuildCombatTest
{
    private GameServerContext _gameServerContext = null!;
    private Player _killer = null!;
    private Player _victim = null!;

    private const uint KillerGuildId = 10;
    private const uint VictimGuildId = 20;

    /// <summary>
    /// Creates two players in a game server context for each test.
    /// </summary>
    [SetUp]
    public async ValueTask SetupAsync()
    {
        var persistenceProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = new BasicModel.GameConfiguration();
        gameConfiguration.Maps.Add(new BasicModel.GameMapDefinition());
        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        this._gameServerContext = new GameServerContext(
            new BasicModel.GameServerDefinition
            {
                GameConfiguration = gameConfiguration,
                ServerConfiguration = new BasicModel.GameServerConfiguration(),
            },
            new Mock<IGuildServer>().Object,
            new Mock<IEventPublisher>().Object,
            new Mock<ILoginServer>().Object,
            new Mock<IFriendServer>().Object,
            persistenceProvider,
            mapInitializer,
            new NullLoggerFactory(),
            new PlugInManager([], new NullLoggerFactory(), null, null),
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = this._gameServerContext.PlugInManager;
        mapInitializer.PathFinderPool = this._gameServerContext.PathFinderPool;

        this._killer = await PlayerTestHelper.CreatePlayerAsync(this._gameServerContext).ConfigureAwait(false);
        this._victim = await PlayerTestHelper.CreatePlayerAsync(this._gameServerContext).ConfigureAwait(false);
        await this._killer.CurrentMap!.AddAsync(this._victim).ConfigureAwait(false);

        this._killer.GuildStatus = new GuildMemberStatus(KillerGuildId, GuildPosition.GuildMaster);
        this._victim.GuildStatus = new GuildMemberStatus(VictimGuildId, GuildPosition.GuildMaster);
    }

    // -------------------------------------------------------------------------
    // PK state bypass
    // -------------------------------------------------------------------------

    /// <summary>
    /// Killing a rival guild member does not change the killer's hero state or PK count.
    /// </summary>
    [Test]
    public async ValueTask KillRivalGuildMember_DoesNotIncrementPkCount()
    {
        this._gameServerContext.UpdateGuildHostility(KillerGuildId, [KillerGuildId], VictimGuildId, [VictimGuildId], true);

        var initialState = this._killer.SelectedCharacter!.State;
        await InvokeAfterKilledPlayerAsync(this._killer, this._victim).ConfigureAwait(false);

        Assert.That(this._killer.SelectedCharacter.State, Is.EqualTo(initialState),
            "Hero state should not change when killing a rival guild member.");
        Assert.That(this._killer.SelectedCharacter.PlayerKillCount, Is.Zero,
            "PK count should not increase when killing a rival guild member.");
    }

    /// <summary>
    /// Killing a non-rival guild member DOES increment the killer's hero state.
    /// </summary>
    [Test]
    public async ValueTask KillNonRivalGuildMember_IncrementsHeroState()
    {
        // guilds are NOT rivals — no UpdateGuildHostility call
        var initialState = this._killer.SelectedCharacter!.State;
        await InvokeAfterKilledPlayerAsync(this._killer, this._victim).ConfigureAwait(false);

        Assert.That(this._killer.SelectedCharacter.State, Is.GreaterThan(initialState),
            "Hero state should increase when killing a non-rival player.");
    }

    // -------------------------------------------------------------------------
    // Self-defense bypass
    // -------------------------------------------------------------------------

    /// <summary>
    /// Hitting a rival guild member does not initiate a self-defense state on the victim.
    /// </summary>
    [Test]
    public void HitRivalGuildMember_DoesNotInitiateSelfDefense()
    {
        this._gameServerContext.UpdateGuildHostility(KillerGuildId, [KillerGuildId], VictimGuildId, [VictimGuildId], true);

        var plugIn = new SelfDefensePlugIn();
        plugIn.AttackableGotHit(this._victim, this._killer, new HitInfo(100, 0, DamageAttributes.Undefined));

        Assert.That(this._gameServerContext.SelfDefenseState, Is.Empty,
            "No self-defense should be initiated between rival guild members.");
    }

    /// <summary>
    /// Hitting a non-rival guild member DOES initiate a self-defense state on the victim.
    /// </summary>
    [Test]
    public void HitNonRivalGuildMember_InitiatesSelfDefense()
    {
        // guilds are NOT rivals
        var plugIn = new SelfDefensePlugIn();
        plugIn.AttackableGotHit(this._victim, this._killer, new HitInfo(100, 0, DamageAttributes.Undefined));

        Assert.That(this._gameServerContext.SelfDefenseState, Is.Not.Empty,
            "Self-defense should be initiated when hit by a non-rival player.");
    }

    /// <summary>
    /// Invokes <c>AfterKilledPlayerAsync</c> on <paramref name="killer"/>
    /// passing <paramref name="killedPlayer"/> as the argument.
    /// </summary>
    private static async ValueTask InvokeAfterKilledPlayerAsync(Player killer, Player killedPlayer)
    {
        await killer.AfterKilledPlayerAsync(killedPlayer).ConfigureAwait(false);
    }
}
