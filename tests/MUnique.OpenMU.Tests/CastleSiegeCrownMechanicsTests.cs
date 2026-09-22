// <copyright file="CastleSiegeCrownMechanicsTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;
using RuntimeGuild = MUnique.OpenMU.Interfaces.Guild;

/// <summary>
/// Tests Castle Siege Crown capture, switch state, and final ownership mechanics.
/// </summary>
[TestFixture]
public class CastleSiegeCrownMechanicsTests
{
    private const uint DefenseGuildId = 10;
    private const uint AttackGuildId = 20;

    /// <summary>
    /// Verifies that an interrupted attempt keeps capped progress and reports the failure once.
    /// </summary>
    [Test]
    public async ValueTask InterruptedCaptureKeepsCappedProgressAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var crownUser = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "CrownUser",
                CastleSiegeJoinSide.Attack1,
                60,
                60)
            .ConfigureAwait(false);
        var firstSwitchUser = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "SwitchOne",
                CastleSiegeJoinSide.Attack1,
                70,
                60)
            .ConfigureAwait(false);
        var secondSwitchUser = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "SwitchTwo",
                CastleSiegeJoinSide.Attack1,
                80,
                60)
            .ConfigureAwait(false);
        fixture.Context.CrownUser = crownUser;
        fixture.Context.SwitchUsers[0] = firstSwitchUser;
        fixture.Context.SwitchUsers[1] = secondSwitchUser;

        await fixture.CheckCrownAsync().ConfigureAwait(false);
        fixture.Context.CrownAccumulatedTime = TimeSpan.FromSeconds(10);
        fixture.Context.SwitchUsers[1] = null;
        await fixture.CheckCrownAsync().ConfigureAwait(false);
        await fixture.CheckCrownAsync().ConfigureAwait(false);

        Assert.That(fixture.Context.CrownAccumulatedTime, Is.EqualTo(TimeSpan.FromSeconds(2)));
        Mock.Get(crownUser.ViewPlugIns.GetPlugIn<ICastleSiegeCrownAccessStatePlugIn>()!)
            .Verify(
                plugIn => plugIn.ShowCrownAccessStateAsync(
                    CastleSiegeCrownAccessState.Fail,
                    TimeSpan.FromSeconds(2)),
                Times.Once);
    }

    /// <summary>
    /// Verifies that only three alive and guilded players on the same attacking side can make progress.
    /// </summary>
    [Test]
    public async ValueTask CaptureRequiresThreeEligiblePlayersOnSameAttackingSideAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var crownUser = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "CrownUser",
                CastleSiegeJoinSide.Attack1,
                60,
                60)
            .ConfigureAwait(false);
        var firstSwitchUser = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "SwitchOne",
                CastleSiegeJoinSide.Attack1,
                70,
                60)
            .ConfigureAwait(false);
        var secondSwitchUser = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "SwitchTwo",
                CastleSiegeJoinSide.Attack2,
                80,
                60)
            .ConfigureAwait(false);
        fixture.Context.CrownUser = crownUser;
        fixture.Context.SwitchUsers[0] = firstSwitchUser;
        fixture.Context.SwitchUsers[1] = secondSwitchUser;
        await fixture.CheckCrownAsync().ConfigureAwait(false);
        Assert.That(fixture.Context.CrownAccumulatedTime, Is.EqualTo(TimeSpan.Zero), "Different attacking sides must not capture.");

        fixture.Context.PlayerJoinSides[secondSwitchUser.SelectedCharacter!.Id] = CastleSiegeJoinSide.Attack1;
        secondSwitchUser.IsAlive = false;
        await fixture.CheckCrownAsync().ConfigureAwait(false);
        Assert.That(fixture.Context.CrownAccumulatedTime, Is.EqualTo(TimeSpan.Zero), "Dead switch users must not capture.");

        secondSwitchUser.IsAlive = true;
        secondSwitchUser.GuildStatus = null;
        await fixture.CheckCrownAsync().ConfigureAwait(false);
        Assert.That(fixture.Context.CrownAccumulatedTime, Is.EqualTo(TimeSpan.Zero), "Guildless switch users must not capture.");

        secondSwitchUser.GuildStatus = new GuildMemberStatus(AttackGuildId, GuildPosition.NormalMember);
        fixture.Context.PlayerJoinSides[crownUser.SelectedCharacter!.Id] = CastleSiegeJoinSide.Defense;
        fixture.Context.PlayerJoinSides[firstSwitchUser.SelectedCharacter!.Id] = CastleSiegeJoinSide.Defense;
        fixture.Context.PlayerJoinSides[secondSwitchUser.SelectedCharacter.Id] = CastleSiegeJoinSide.Defense;
        await fixture.CheckCrownAsync().ConfigureAwait(false);
        Assert.That(fixture.Context.CrownAccumulatedTime, Is.EqualTo(TimeSpan.Zero), "The defending side must not capture.");

        fixture.Context.PlayerJoinSides[crownUser.SelectedCharacter.Id] = CastleSiegeJoinSide.Attack1;
        fixture.Context.PlayerJoinSides[firstSwitchUser.SelectedCharacter.Id] = CastleSiegeJoinSide.Attack1;
        fixture.Context.PlayerJoinSides[secondSwitchUser.SelectedCharacter.Id] = CastleSiegeJoinSide.Attack1;
        await fixture.CheckCrownAsync().ConfigureAwait(false);
        Assert.That(fixture.Context.CrownAccumulatedTime, Is.EqualTo(TimeSpan.FromSeconds(1)));
    }

    /// <summary>
    /// Verifies that changing the Crown user fails the previous attempt without resetting its progress.
    /// </summary>
    [Test]
    public async ValueTask ChangedCrownUserContinuesCappedProgressAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var previousCrownUser = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "PreviousUser",
                CastleSiegeJoinSide.Attack1,
                60,
                60)
            .ConfigureAwait(false);
        var newCrownUser = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "NewUser",
                CastleSiegeJoinSide.Attack1,
                61,
                60)
            .ConfigureAwait(false);
        var firstSwitchUser = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "SwitchOne",
                CastleSiegeJoinSide.Attack1,
                70,
                60)
            .ConfigureAwait(false);
        var secondSwitchUser = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "SwitchTwo",
                CastleSiegeJoinSide.Attack1,
                80,
                60)
            .ConfigureAwait(false);
        fixture.Context.CrownUser = previousCrownUser;
        fixture.Context.SwitchUsers[0] = firstSwitchUser;
        fixture.Context.SwitchUsers[1] = secondSwitchUser;
        await fixture.CheckCrownAsync().ConfigureAwait(false);

        fixture.Context.CrownUser = newCrownUser;
        await fixture.CheckCrownAsync().ConfigureAwait(false);

        Assert.That(fixture.Context.CrownAccumulatedTime, Is.EqualTo(TimeSpan.FromSeconds(2)));
        Mock.Get(previousCrownUser.ViewPlugIns.GetPlugIn<ICastleSiegeCrownAccessStatePlugIn>()!)
            .Verify(
                plugIn => plugIn.ShowCrownAccessStateAsync(
                    CastleSiegeCrownAccessState.Fail,
                    TimeSpan.FromSeconds(1)),
                Times.Once);
        Mock.Get(newCrownUser.ViewPlugIns.GetPlugIn<ICastleSiegeCrownAccessStatePlugIn>()!)
            .Verify(
                plugIn => plugIn.ShowCrownAccessStateAsync(
                    CastleSiegeCrownAccessState.Attempt,
                    TimeSpan.FromSeconds(2)),
                Times.Once);
    }

    /// <summary>
    /// Verifies a successful capture, side swap, participant update, respawn, and restart recovery.
    /// </summary>
    [Test]
    public async ValueTask SuccessfulCaptureChangesIntermediateOwnerAndSwapsSidesAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var crownUser = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "CrownUser",
                CastleSiegeJoinSide.Attack1,
                60,
                60)
            .ConfigureAwait(false);
        var firstSwitchUser = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "SwitchOne",
                CastleSiegeJoinSide.Attack1,
                70,
                60)
            .ConfigureAwait(false);
        var secondSwitchUser = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "SwitchTwo",
                CastleSiegeJoinSide.Attack1,
                80,
                60)
            .ConfigureAwait(false);
        var formerDefender = await fixture.CreatePlayerAsync(
                DefenseGuildId,
                "Defender",
                CastleSiegeJoinSide.Defense,
                200,
                200)
            .ConfigureAwait(false);
        fixture.Context.CrownUser = crownUser;
        fixture.Context.SwitchUsers[0] = firstSwitchUser;
        fixture.Context.SwitchUsers[1] = secondSwitchUser;
        fixture.Context.IsCrownAvailable = true;
        fixture.Context.FinalGuildList[AttackGuildId].IsAllianceMaster = false;

        await fixture.CheckCrownAsync().ConfigureAwait(false);
        await fixture.CheckCrownAsync().ConfigureAwait(false);
        await fixture.CheckCrownAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(fixture.Context.MiddleOwnerGuildId, Is.EqualTo(AttackGuildId));
            Assert.That(fixture.Context.FinalGuildList[AttackGuildId].Side, Is.EqualTo(CastleSiegeJoinSide.Defense));
            Assert.That(fixture.Context.FinalGuildList[DefenseGuildId].Side, Is.EqualTo(CastleSiegeJoinSide.Attack1));
            Assert.That(fixture.Context.GetPlayerJoinSide(crownUser), Is.EqualTo(CastleSiegeJoinSide.Defense));
            Assert.That(fixture.Context.GetPlayerJoinSide(formerDefender), Is.EqualTo(CastleSiegeJoinSide.Attack1));
            Assert.That(fixture.Context.IsCrownAvailable, Is.False);
            Assert.That(fixture.Context.CrownAccumulatedTime, Is.EqualTo(TimeSpan.Zero));
            Assert.That(fixture.Context.CrownUser, Is.Null);
            Assert.That(fixture.Context.SwitchUsers, Is.All.Null);
            Assert.That(fixture.Context.SiegeData.OwnerGuildId, Is.EqualTo(fixture.AttackPersistentGuildId));
            Assert.That(formerDefender.Position.X, Is.InRange((byte)35, (byte)40));
            Assert.That(formerDefender.Position.Y, Is.InRange((byte)11, (byte)16));
        });
        Mock.Get(crownUser.ViewPlugIns.GetPlugIn<ICastleSiegeCrownAccessStatePlugIn>()!)
            .Verify(
                plugIn => plugIn.ShowCrownAccessStateAsync(
                    CastleSiegeCrownAccessState.Success,
                    TimeSpan.FromSeconds(3)),
                Times.Once);
        Mock.Get(formerDefender.ViewPlugIns.GetPlugIn<ICastleSiegeOwnershipChangePlugIn>()!)
            .Verify(
                plugIn => plugIn.ShowOwnershipChangeAsync("Attackers"),
                Times.Once);

        using (var persistenceContext = fixture.PersistenceContextProvider.CreateNewTypedContext(
                   typeof(CastleSiegeData),
                   false,
                   fixture.GameServerContext.Configuration))
        {
            var persistedData = (await persistenceContext.GetAsync<CastleSiegeData>().ConfigureAwait(false)).Single();
            Assert.That(persistedData.OwnerGuildId, Is.EqualTo(fixture.AttackPersistentGuildId));
        }

        var restartedContext = new CastleSiegeContext(fixture.GameServerContext, fixture.Configuration);
        await restartedContext.InitializeAsync(fixture.InitializationTimeUtc).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(restartedContext.FinalGuildList[AttackGuildId].Side, Is.EqualTo(CastleSiegeJoinSide.Defense));
            Assert.That(restartedContext.FinalGuildList[DefenseGuildId].Side, Is.EqualTo(CastleSiegeJoinSide.Attack1));
            Assert.That(restartedContext.MiddleOwnerGuildId, Is.EqualTo(AttackGuildId));
        });
    }

    /// <summary>
    /// Verifies switch occupant broadcasts and Crown lock-state calculation.
    /// </summary>
    [Test]
    public async ValueTask SwitchInformationControlsCrownAvailabilityAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        try
        {
            await fixture.Context.NpcController.PrepareAsync().ConfigureAwait(false);
            var firstSwitch = fixture.Context.ActiveNpcs
                .Select(runtime => runtime.SpawnedInstance)
                .OfType<CastleSiegeSwitch>()
                .Single(siegeSwitch => siegeSwitch.SwitchIndex == 0);
            var crown = fixture.Context.ActiveNpcs
                .Select(runtime => runtime.SpawnedInstance)
                .OfType<CastleSiegeCrown>()
                .Single();
            var firstSwitchUser = await fixture.CreatePlayerAsync(
                    AttackGuildId,
                    "SwitchOne",
                    CastleSiegeJoinSide.Attack1,
                    firstSwitch.Position.X,
                    firstSwitch.Position.Y)
                .ConfigureAwait(false);
            var secondSwitch = fixture.Context.ActiveNpcs
                .Select(runtime => runtime.SpawnedInstance)
                .OfType<CastleSiegeSwitch>()
                .Single(siegeSwitch => siegeSwitch.SwitchIndex == 1);
            var secondSwitchUser = await fixture.CreatePlayerAsync(
                    AttackGuildId,
                    "SwitchTwo",
                    CastleSiegeJoinSide.Attack1,
                    secondSwitch.Position.X,
                    secondSwitch.Position.Y)
                .ConfigureAwait(false);
            fixture.Context.SwitchUsers[0] = firstSwitchUser;
            fixture.Context.SwitchUsers[1] = secondSwitchUser;

            await CastleSiegeSwitchMechanics.SendSwitchInfoAsync(fixture.Context).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(fixture.Context.IsCrownAvailable, Is.True);
                Assert.That(crown.State, Is.EqualTo(CastleSiegeCrownState.Idle));
            });
            Mock.Get(firstSwitchUser.ViewPlugIns.GetPlugIn<ICastleSiegeSwitchInfoPlugIn>()!)
                .Verify(
                    plugIn => plugIn.ShowSwitchInfoAsync(
                        It.Is<CastleSiegeSwitchInfo>(info =>
                            info.ObjectId == firstSwitch.Id
                            && info.IsOccupied
                            && info.JoinSide == CastleSiegeJoinSide.Attack1
                            && info.GuildName == "Attackers"
                            && info.CharacterName == "SwitchOne")),
                    Times.Once);
            Mock.Get(firstSwitchUser.ViewPlugIns.GetPlugIn<ICastleSiegeCrownStatePlugIn>()!)
                .Verify(plugIn => plugIn.ShowCrownStateAsync(true), Times.Once);

            await CastleSiegeSwitchMechanics.SendSwitchInfoAsync(fixture.Context).ConfigureAwait(false);
            Mock.Get(firstSwitchUser.ViewPlugIns.GetPlugIn<ICastleSiegeSwitchInfoPlugIn>()!)
                .Verify(plugIn => plugIn.ShowSwitchInfoAsync(It.IsAny<CastleSiegeSwitchInfo>()), Times.Exactly(2));
            Mock.Get(firstSwitchUser.ViewPlugIns.GetPlugIn<ICastleSiegeCrownStatePlugIn>()!)
                .Verify(plugIn => plugIn.ShowCrownStateAsync(true), Times.Once);

            var enteringPlayer = await fixture.CreatePlayerAsync(
                    AttackGuildId,
                    "EnteringPlayer",
                    CastleSiegeJoinSide.Attack1,
                    90,
                    60)
                .ConfigureAwait(false);
            fixture.Context.IsCrownAvailable = false;
            crown.State = CastleSiegeCrownState.Locked;
            await CastleSiegeSwitchMechanics
                .SynchronizePlayerAsync(fixture.Context, enteringPlayer)
                .ConfigureAwait(false);
            Mock.Get(enteringPlayer.ViewPlugIns.GetPlugIn<ICastleSiegeSwitchInfoPlugIn>()!)
                .Verify(plugIn => plugIn.ShowSwitchInfoAsync(It.IsAny<CastleSiegeSwitchInfo>()), Times.Exactly(2));
            Mock.Get(enteringPlayer.ViewPlugIns.GetPlugIn<ICastleSiegeCrownStatePlugIn>()!)
                .Verify(plugIn => plugIn.ShowCrownStateAsync(true), Times.Once);
            Assert.Multiple(() =>
            {
                Assert.That(fixture.Context.IsCrownAvailable, Is.False);
                Assert.That(crown.State, Is.EqualTo(CastleSiegeCrownState.Locked));
            });

            fixture.Context.PlayerJoinSides[secondSwitchUser.SelectedCharacter!.Id] = CastleSiegeJoinSide.Attack2;
            await CastleSiegeSwitchMechanics.SendSwitchInfoAsync(fixture.Context).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(fixture.Context.IsCrownAvailable, Is.False);
                Assert.That(crown.State, Is.EqualTo(CastleSiegeCrownState.Locked));
            });
            Mock.Get(firstSwitchUser.ViewPlugIns.GetPlugIn<ICastleSiegeCrownStatePlugIn>()!)
                .Verify(plugIn => plugIn.ShowCrownStateAsync(false), Times.Once);
            Mock.Get(firstSwitchUser.ViewPlugIns.GetPlugIn<ICastleSiegeSwitchInfoPlugIn>()!)
                .Verify(plugIn => plugIn.ShowSwitchInfoAsync(It.IsAny<CastleSiegeSwitchInfo>()), Times.Exactly(3));
        }
        finally
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies that duplicate configured switch spawns do not overflow the broadcast snapshot.
    /// </summary>
    [Test]
    public async ValueTask DuplicateSwitchSpawnsAreTrackedByObjectIdAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var firstSwitchDefinition = fixture.Configuration.NpcDefinitions
            .Single(definition => definition.MonsterDefinition?.Number == CastleSiegeSwitch.FirstMonsterNumber);
        fixture.Configuration.NpcDefinitions.Add(new BasicModel.CastleSiegeNpcDefinition
        {
            MonsterDefinition = firstSwitchDefinition.MonsterDefinition,
            InstanceId = 3,
            SpawnX = 71,
            SpawnY = 60,
            Direction = Direction.South,
        });

        try
        {
            await fixture.Context.NpcController.PrepareAsync().ConfigureAwait(false);
            var observer = await fixture.CreatePlayerAsync(
                    AttackGuildId,
                    "Observer",
                    CastleSiegeJoinSide.Attack1,
                    90,
                    60)
                .ConfigureAwait(false);

            await CastleSiegeSwitchMechanics.SendSwitchInfoAsync(fixture.Context).ConfigureAwait(false);
            await CastleSiegeSwitchMechanics.SendSwitchInfoAsync(fixture.Context).ConfigureAwait(false);

            Mock.Get(observer.ViewPlugIns.GetPlugIn<ICastleSiegeSwitchInfoPlugIn>()!)
                .Verify(
                    plugIn => plugIn.ShowSwitchInfoAsync(It.IsAny<CastleSiegeSwitchInfo>()),
                    Times.Exactly(3));
            Assert.That(fixture.Context.LastBroadcastSwitchInfos, Has.Count.EqualTo(3));
        }
        finally
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies that a new owner is persisted and the Castle Siege economy is reset.
    /// </summary>
    [Test]
    public async ValueTask FinalResultPersistsWinnerAndResetsEconomyAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var observer = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "Observer",
                CastleSiegeJoinSide.Attack1,
                100,
                100)
            .ConfigureAwait(false);
        fixture.Context.MiddleOwnerGuildId = AttackGuildId;
        fixture.Context.SiegeData.TaxChaos = 3;
        fixture.Context.SiegeData.TaxStore = 3;
        fixture.Context.SiegeData.TaxHunt = 300_000;
        fixture.Context.SiegeData.TributeMoney = 6_000;
        fixture.Context.SiegeData.IsHuntZoneEnabled = true;

        await CastleSiegeCrownMechanics
            .CheckResultAsync(fixture.Context)
            .ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(fixture.Context.SiegeData.OwnerGuildId, Is.EqualTo(fixture.AttackPersistentGuildId));
            Assert.That(fixture.Context.SiegeData.IsOccupied, Is.True);
            Assert.That(fixture.Context.SiegeData.TaxChaos, Is.Zero);
            Assert.That(fixture.Context.SiegeData.TaxStore, Is.Zero);
            Assert.That(fixture.Context.SiegeData.TaxHunt, Is.Zero);
            Assert.That(fixture.Context.SiegeData.TributeMoney, Is.Zero);
            Assert.That(fixture.Context.SiegeData.IsHuntZoneEnabled, Is.False);
        });
        using (var persistenceContext = fixture.PersistenceContextProvider.CreateNewTypedContext(
                   typeof(CastleSiegeData),
                   false,
                   fixture.GameServerContext.Configuration))
        {
            var persistedData = (await persistenceContext.GetAsync<CastleSiegeData>().ConfigureAwait(false)).Single();
            Assert.That(persistedData.OwnerGuildId, Is.EqualTo(fixture.AttackPersistentGuildId));
            Assert.That(persistedData.TributeMoney, Is.Zero);
        }

        Mock.Get(observer.ViewPlugIns.GetPlugIn<ICastleSiegeOwnershipChangePlugIn>()!)
            .Verify(
                plugIn => plugIn.ShowOwnershipChangeAsync("Attackers"),
                Times.Once);
    }

    /// <summary>
    /// Verifies that the current owner and economy are retained when the Crown was not captured.
    /// </summary>
    [Test]
    public async ValueTask FinalResultRetainsCurrentOwnerWithoutCaptureAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.MiddleOwnerGuildId = null;
        fixture.Context.SiegeData.TaxChaos = 3;
        fixture.Context.SiegeData.TaxStore = 2;
        fixture.Context.SiegeData.TaxHunt = 100_000;
        fixture.Context.SiegeData.TributeMoney = 6_000;

        await CastleSiegeCrownMechanics
            .CheckResultAsync(fixture.Context)
            .ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(fixture.Context.SiegeData.OwnerGuildId, Is.EqualTo(fixture.DefensePersistentGuildId));
            Assert.That(fixture.Context.SiegeData.TaxChaos, Is.EqualTo(3));
            Assert.That(fixture.Context.SiegeData.TaxStore, Is.EqualTo(2));
            Assert.That(fixture.Context.SiegeData.TaxHunt, Is.EqualTo(100_000));
            Assert.That(fixture.Context.SiegeData.TributeMoney, Is.EqualTo(6_000));
        });
    }

    /// <summary>
    /// Verifies that an unresolved intermediate owner does not abort End-state processing.
    /// </summary>
    [Test]
    public async ValueTask FinalResultRetainsPersistedOwnerWhenIntermediateOwnerIsMissingAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.MiddleOwnerGuildId = uint.MaxValue;

        await CastleSiegeCrownMechanics.CheckResultAsync(fixture.Context).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(fixture.Context.SiegeData.OwnerGuildId, Is.EqualTo(fixture.DefensePersistentGuildId));
            Assert.That(fixture.Context.SiegeData.IsOccupied, Is.True);
        });
    }

    /// <summary>
    /// Verifies that a completed ownership tenure's economy is not restored when its guild recaptures the Crown.
    /// </summary>
    [Test]
    public async ValueTask RecaptureDoesNotRestorePreviousOwnershipEconomyAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var attacker = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "Attacker",
                CastleSiegeJoinSide.Attack1,
                60,
                60)
            .ConfigureAwait(false);
        var formerDefender = await fixture.CreatePlayerAsync(
                DefenseGuildId,
                "FormerDefender",
                CastleSiegeJoinSide.Defense,
                61,
                60)
            .ConfigureAwait(false);
        fixture.Context.SiegeData.TaxChaos = 3;
        fixture.Context.SiegeData.TaxStore = 2;
        fixture.Context.SiegeData.TaxHunt = 100_000;
        fixture.Context.SiegeData.TributeMoney = 6_000;

        await CastleSiegeCrownMechanics
            .ChangeWinnerGuildAsync(fixture.Context, attacker, CastleSiegeJoinSide.Attack1)
            .ConfigureAwait(false);
        await CastleSiegeCrownMechanics
            .ChangeWinnerGuildAsync(fixture.Context, formerDefender, CastleSiegeJoinSide.Attack1)
            .ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(fixture.Context.SiegeData.OwnerGuildId, Is.EqualTo(fixture.DefensePersistentGuildId));
            Assert.That(fixture.Context.SiegeData.TaxChaos, Is.Zero);
            Assert.That(fixture.Context.SiegeData.TaxStore, Is.Zero);
            Assert.That(fixture.Context.SiegeData.TaxHunt, Is.Zero);
            Assert.That(fixture.Context.SiegeData.TributeMoney, Is.Zero);
        });
    }

    /// <summary>
    /// Verifies that Crown progress uses elapsed wall time when periodic ticks are delayed.
    /// </summary>
    [Test]
    public async ValueTask CrownProgressUsesElapsedTimeAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var crownUser = await fixture.CreatePlayerAsync(AttackGuildId, "CrownUser", CastleSiegeJoinSide.Attack1, 60, 60).ConfigureAwait(false);
        fixture.Context.CrownUser = crownUser;
        fixture.Context.SwitchUsers[0] = await fixture.CreatePlayerAsync(AttackGuildId, "SwitchOne", CastleSiegeJoinSide.Attack1, 70, 60).ConfigureAwait(false);
        fixture.Context.SwitchUsers[1] = await fixture.CreatePlayerAsync(AttackGuildId, "SwitchTwo", CastleSiegeJoinSide.Attack1, 80, 60).ConfigureAwait(false);

        await fixture.CheckCrownAsync(TimeSpan.FromSeconds(2)).ConfigureAwait(false);

        Assert.That(fixture.Context.CrownAccumulatedTime, Is.EqualTo(TimeSpan.FromSeconds(2)));
        Mock.Get(crownUser.ViewPlugIns.GetPlugIn<ICastleSiegeCrownAccessStatePlugIn>()!)
            .Verify(
                plugIn => plugIn.ShowCrownAccessStateAsync(
                    CastleSiegeCrownAccessState.Attempt,
                    TimeSpan.FromSeconds(2)),
                Times.Once);
    }

    /// <summary>
    /// Verifies that a delayed periodic tick cannot satisfy the Crown hold duration in one update.
    /// </summary>
    [Test]
    public async ValueTask CrownProgressClampsDelayedUpdatesAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var crownUser = await fixture.CreatePlayerAsync(AttackGuildId, "CrownUser", CastleSiegeJoinSide.Attack1, 60, 60).ConfigureAwait(false);
        fixture.Context.CrownUser = crownUser;
        fixture.Context.SwitchUsers[0] = await fixture.CreatePlayerAsync(AttackGuildId, "SwitchOne", CastleSiegeJoinSide.Attack1, 70, 60).ConfigureAwait(false);
        fixture.Context.SwitchUsers[1] = await fixture.CreatePlayerAsync(AttackGuildId, "SwitchTwo", CastleSiegeJoinSide.Attack1, 80, 60).ConfigureAwait(false);

        await fixture.CheckCrownAsync(TimeSpan.FromSeconds(30)).ConfigureAwait(false);

        Assert.That(fixture.Context.CrownAccumulatedTime, Is.EqualTo(TimeSpan.FromSeconds(2)));
        Assert.That(fixture.Context.SiegeData.OwnerGuildId, Is.EqualTo(fixture.DefensePersistentGuildId));
        Mock.Get(crownUser.ViewPlugIns.GetPlugIn<ICastleSiegeCrownAccessStatePlugIn>()!)
            .Verify(
                plugIn => plugIn.ShowCrownAccessStateAsync(
                    CastleSiegeCrownAccessState.Attempt,
                    TimeSpan.FromSeconds(2)),
                Times.Once);
    }

    private static async ValueTask<TestFixture> CreateFixtureAsync()
    {
        var persistenceContextProvider = new InMemoryPersistenceContextProvider();
        BasicModel.GameConfiguration gameConfiguration;
        BasicModel.CastleSiegeConfiguration configuration;
        BasicModel.GameMapDefinition siegeMap;
        Guid defensePersistentGuildId;
        Guid attackPersistentGuildId;
        using (var persistenceContext = persistenceContextProvider.CreateNewContext())
        {
            gameConfiguration = persistenceContext.CreateNew<BasicModel.GameConfiguration>();
            var normalMap = persistenceContext.CreateNew<BasicModel.GameMapDefinition>();
            normalMap.Number = 0;
            normalMap.TerrainData = new byte[65_539];
            gameConfiguration.Maps.Add(normalMap);

            siegeMap = persistenceContext.CreateNew<BasicModel.GameMapDefinition>();
            siegeMap.Number = 30;
            siegeMap.TerrainData = new byte[65_539];
            gameConfiguration.Maps.Add(siegeMap);

            configuration = persistenceContext.CreateNew<BasicModel.CastleSiegeConfiguration>();
            configuration.Enabled = true;
            configuration.CastleSiegeMapDefinition = siegeMap;
            configuration.CrownHoldTimeSeconds = 3;
            configuration.AttackRespawnArea = persistenceContext.CreateNew<BasicModel.CastleSiegeZoneDefinition>();
            configuration.AttackRespawnArea.X1 = 35;
            configuration.AttackRespawnArea.Y1 = 11;
            configuration.AttackRespawnArea.X2 = 40;
            configuration.AttackRespawnArea.Y2 = 16;
            configuration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry
            {
                State = CastleSiegeState.Ready,
                DayOfWeek = DayOfWeek.Monday,
            });
            configuration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry
            {
                State = CastleSiegeState.Start,
                DayOfWeek = DayOfWeek.Tuesday,
            });
            gameConfiguration.CastleSiegeConfiguration = configuration;

            AddNpc(CastleSiegeCrown.MonsterNumber, 1, 60, 60);
            AddNpc(CastleSiegeSwitch.FirstMonsterNumber, 1, 70, 60);
            AddNpc(CastleSiegeSwitch.SecondMonsterNumber, 2, 80, 60);

            var defenseGuild = persistenceContext.CreateNew<BasicModel.Guild>();
            defenseGuild.Name = "Defenders";
            defensePersistentGuildId = defenseGuild.Id;
            var attackGuild = persistenceContext.CreateNew<BasicModel.Guild>();
            attackGuild.Name = "Attackers";
            attackPersistentGuildId = attackGuild.Id;
            var siegeData = persistenceContext.CreateNew<BasicModel.CastleSiegeData>();
            siegeData.OwnerGuildId = defensePersistentGuildId;
            siegeData.IsOccupied = true;
            await persistenceContext.SaveChangesAsync().ConfigureAwait(false);

            void AddNpc(short monsterNumber, byte instanceId, byte x, byte y)
            {
                var monster = persistenceContext.CreateNew<BasicModel.MonsterDefinition>();
                monster.Number = monsterNumber;
                monster.ObjectKind = NpcObjectKind.PassiveNpc;
                gameConfiguration.Monsters.Add(monster);
                configuration.NpcDefinitions.Add(new BasicModel.CastleSiegeNpcDefinition
                {
                    MonsterDefinition = monster,
                    InstanceId = instanceId,
                    SpawnX = x,
                    SpawnY = y,
                    Direction = Direction.South,
                });
            }
        }

        var guildServer = new Mock<IGuildServer>();
        guildServer
            .Setup(server => server.GetGuildIdAsync(defensePersistentGuildId))
            .Returns(new ValueTask<uint>(DefenseGuildId));
        guildServer
            .Setup(server => server.GetGuildIdAsync(attackPersistentGuildId))
            .Returns(new ValueTask<uint>(AttackGuildId));
        guildServer
            .Setup(server => server.GetGuildAsync(DefenseGuildId))
            .Returns(new ValueTask<RuntimeGuild?>(new RuntimeGuild { Name = "Defenders" }));
        guildServer
            .Setup(server => server.GetGuildAsync(AttackGuildId))
            .Returns(new ValueTask<RuntimeGuild?>(new RuntimeGuild { Name = "Attackers" }));

        var plugInManager = new PlugInManager([], NullLoggerFactory.Instance, null, null);
        var mapInitializer = new MapInitializer(
            gameConfiguration,
            new NullLogger<MapInitializer>(),
            NullDropGenerator.Instance,
            null);
        var gameServerContext = new GameServerContext(
            new BasicModel.GameServerDefinition
            {
                GameConfiguration = gameConfiguration,
                ServerConfiguration = new BasicModel.GameServerConfiguration(),
            },
            guildServer.Object,
            new Mock<IEventPublisher>().Object,
            new Mock<ILoginServer>().Object,
            new Mock<IFriendServer>().Object,
            persistenceContextProvider,
            mapInitializer,
            NullLoggerFactory.Instance,
            plugInManager,
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameServerContext.PlugInManager;
        mapInitializer.PathFinderPool = gameServerContext.PathFinderPool;

        var initializationTimeUtc = new DateTime(2026, 8, 3, 12, 0, 0, DateTimeKind.Utc);
        var context = new CastleSiegeContext(gameServerContext, configuration);
        await context.InitializeAsync(initializationTimeUtc).ConfigureAwait(false);
        context.CurrentState = CastleSiegeState.Start;
        context.FinalGuildList[DefenseGuildId] = new CastleSiegeGuildParticipant
        {
            GuildId = DefenseGuildId,
            PersistentGuildId = defensePersistentGuildId,
            GuildName = "Defenders",
            Side = CastleSiegeJoinSide.Defense,
            IsAllianceMaster = true,
        };
        context.FinalGuildList[AttackGuildId] = new CastleSiegeGuildParticipant
        {
            GuildId = AttackGuildId,
            PersistentGuildId = attackPersistentGuildId,
            GuildName = "Attackers",
            Side = CastleSiegeJoinSide.Attack1,
            IsAllianceMaster = true,
        };
        var map = await gameServerContext.GetMapAsync(30).ConfigureAwait(false)
                  ?? throw new InvalidOperationException("The Castle Siege test map could not be initialized.");
        return new(
            persistenceContextProvider,
            configuration,
            gameServerContext,
            context,
            map,
            defensePersistentGuildId,
            attackPersistentGuildId,
            initializationTimeUtc);
    }

    private sealed record TestFixture(
        InMemoryPersistenceContextProvider PersistenceContextProvider,
        CastleSiegeConfiguration Configuration,
        GameServerContext GameServerContext,
        CastleSiegeContext Context,
        GameMap SiegeMap,
        Guid DefensePersistentGuildId,
        Guid AttackPersistentGuildId,
        DateTime InitializationTimeUtc)
    {
        private DateTime? _crownTimeUtc;

        /// <summary>
        /// Creates and tracks a Castle Siege participant.
        /// </summary>
        internal async ValueTask<Player> CreatePlayerAsync(
            uint guildId,
            string characterName,
            CastleSiegeJoinSide side,
            byte x,
            byte y)
        {
            var player = await PlayerTestHelper.CreatePlayerAsync(this.GameServerContext).ConfigureAwait(false);
            player.SelectedCharacter!.Id = Guid.NewGuid();
            player.SelectedCharacter.Name = characterName;
            player.GuildStatus = new GuildMemberStatus(guildId, GuildPosition.NormalMember);
            await this.GameServerContext.AddPlayerAsync(player).ConfigureAwait(false);
            await player.WarpToAsync(new ExitGate
            {
                Map = this.Configuration.CastleSiegeMapDefinition,
                X1 = x,
                X2 = x,
                Y1 = y,
                Y2 = y,
                Direction = Direction.South,
            }).ConfigureAwait(false);
            await player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
            player.IsAlive = true;
            this.Context.TrackPlayer(player, this.SiegeMap);
            this.Context.PlayerJoinSides[player.SelectedCharacter.Id] = side;
            return player;
        }

        /// <summary>
        /// Advances the Crown mechanics clock and executes one progress check.
        /// </summary>
        /// <param name="elapsed">The elapsed time since the previous check.</param>
        /// <returns>A task that represents the asynchronous check.</returns>
        internal ValueTask CheckCrownAsync(TimeSpan? elapsed = null)
        {
            if (this._crownTimeUtc is null)
            {
                this._crownTimeUtc = this.InitializationTimeUtc;
                this.Context.LastCrownUpdateUtc = this.InitializationTimeUtc;
            }

            this._crownTimeUtc += elapsed ?? TimeSpan.FromSeconds(1);
            return CastleSiegeCrownMechanics.CheckMiddleWinnerAsync(this.Context, this._crownTimeUtc.Value);
        }
    }
}
