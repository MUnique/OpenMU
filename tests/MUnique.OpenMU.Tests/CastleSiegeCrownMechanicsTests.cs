// <copyright file="CastleSiegeCrownMechanicsTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.World;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Party;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.MessageHandler;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
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
        fixture.Context.GetSelectedGuild(AttackGuildId).IsAllianceMaster = false;

        await fixture.CheckCrownAsync().ConfigureAwait(false);
        await fixture.CheckCrownAsync().ConfigureAwait(false);
        await fixture.CheckCrownAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(fixture.Context.MiddleOwnerGuildId, Is.EqualTo(fixture.AttackPersistentGuildId));
            Assert.That(fixture.Context.GetSelectedGuild(AttackGuildId).Side, Is.EqualTo(CastleSiegeJoinSide.Defense));
            Assert.That(fixture.Context.GetSelectedGuild(DefenseGuildId).Side, Is.EqualTo(CastleSiegeJoinSide.Attack1));
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
            Assert.That(restartedContext.GetSelectedGuild(AttackGuildId).Side, Is.EqualTo(CastleSiegeJoinSide.Defense));
            Assert.That(restartedContext.GetSelectedGuild(DefenseGuildId).Side, Is.EqualTo(CastleSiegeJoinSide.Attack1));
            Assert.That(restartedContext.MiddleOwnerGuildId, Is.EqualTo(fixture.AttackPersistentGuildId));
        });
    }

    /// <summary>
    /// Verifies that a Crown seal by a member of an attacking alliance hands the castle to the alliance master guild,
    /// because the tax exemption and the owner alliance checks compare against the alliance master.
    /// </summary>
    [Test]
    public async ValueTask CaptureByAllianceMemberTransfersOwnershipToAllianceMasterAsync()
    {
        const uint allianceMemberGuildId = 21;
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.AddSelectedGuild(new CastleSiegeGuildParticipant
        {
            GuildId = allianceMemberGuildId,
            PersistentGuildId = Guid.NewGuid(),
            GuildName = "AttackerAllies",
            Side = CastleSiegeJoinSide.Attack1,
            IsAllianceMaster = false,
        });
        var crownUser = await fixture.CreatePlayerAsync(
                allianceMemberGuildId,
                "AllyCrownUser",
                CastleSiegeJoinSide.Attack1,
                60,
                60)
            .ConfigureAwait(false);
        var firstSwitchUser = await fixture.CreatePlayerAsync(
                allianceMemberGuildId,
                "AllySwitchOne",
                CastleSiegeJoinSide.Attack1,
                70,
                60)
            .ConfigureAwait(false);
        var secondSwitchUser = await fixture.CreatePlayerAsync(
                AttackGuildId,
                "MasterSwitchTwo",
                CastleSiegeJoinSide.Attack1,
                80,
                60)
            .ConfigureAwait(false);
        fixture.Context.CrownUser = crownUser;
        fixture.Context.SwitchUsers[0] = firstSwitchUser;
        fixture.Context.SwitchUsers[1] = secondSwitchUser;
        fixture.Context.IsCrownAvailable = true;

        await fixture.CheckCrownAsync().ConfigureAwait(false);
        await fixture.CheckCrownAsync().ConfigureAwait(false);
        await fixture.CheckCrownAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(fixture.Context.MiddleOwnerGuildId, Is.EqualTo(fixture.AttackPersistentGuildId));
            Assert.That(fixture.Context.SiegeData.OwnerGuildId, Is.EqualTo(fixture.AttackPersistentGuildId));
            Assert.That(fixture.Context.GetSelectedGuild(AttackGuildId).Side, Is.EqualTo(CastleSiegeJoinSide.Defense));
            Assert.That(fixture.Context.GetSelectedGuild(allianceMemberGuildId).Side, Is.EqualTo(CastleSiegeJoinSide.Defense));
            Assert.That(fixture.Context.GetSelectedGuild(DefenseGuildId).Side, Is.EqualTo(CastleSiegeJoinSide.Attack1));
        });
        Mock.Get(crownUser.ViewPlugIns.GetPlugIn<ICastleSiegeOwnershipChangePlugIn>()!)
            .Verify(
                plugIn => plugIn.ShowOwnershipChangeAsync("Attackers"),
                Times.Once);
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
        fixture.Context.MiddleOwnerGuildId = fixture.AttackPersistentGuildId;
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
        fixture.Context.MiddleOwnerGuildId = Guid.NewGuid();

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

    /// <summary>
    /// Verifies every partial Crown/switch occupancy combination independently of packets.
    /// </summary>
    /// <param name="hasCrownUser">Whether the Crown is occupied.</param>
    /// <param name="hasFirstSwitchUser">Whether the first switch is occupied.</param>
    /// <param name="hasSecondSwitchUser">Whether the second switch is occupied.</param>
    [TestCase(false, false, false)]
    [TestCase(false, false, true)]
    [TestCase(false, true, false)]
    [TestCase(false, true, true)]
    [TestCase(true, false, false)]
    [TestCase(true, false, true)]
    [TestCase(true, true, false)]
    [TestCase(true, true, true)]
    public async ValueTask CrownProgressRequiresCompleteOccupancyAsync(bool hasCrownUser, bool hasFirstSwitchUser, bool hasSecondSwitchUser)
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var crownUser = await fixture.CreatePlayerAsync(AttackGuildId, "Crown", CastleSiegeJoinSide.Attack1, 60, 60).ConfigureAwait(false);
        var firstSwitchUser = await fixture.CreatePlayerAsync(AttackGuildId, "First", CastleSiegeJoinSide.Attack1, 70, 60).ConfigureAwait(false);
        var secondSwitchUser = await fixture.CreatePlayerAsync(AttackGuildId, "Second", CastleSiegeJoinSide.Attack1, 80, 60).ConfigureAwait(false);
        fixture.Context.CrownUser = hasCrownUser ? crownUser : null;
        fixture.Context.SwitchUsers[0] = hasFirstSwitchUser ? firstSwitchUser : null;
        fixture.Context.SwitchUsers[1] = hasSecondSwitchUser ? secondSwitchUser : null;

        await fixture.CheckCrownAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(fixture.Context.CrownAccumulatedTime, Is.EqualTo(hasCrownUser && hasFirstSwitchUser && hasSecondSwitchUser ? TimeSpan.FromSeconds(1) : TimeSpan.Zero));
            Assert.That(fixture.Context.SiegeData.OwnerGuildId, Is.EqualTo(fixture.DefensePersistentGuildId));
        });
    }

    /// <summary>A real Crown capture can relocate a newer client out of an ordinary pending interaction.</summary>
    /// <param name="interaction">The interaction established through normal request-owner funnels.</param>
    [TestCase("party")]
    [TestCase("requested")]
    [TestCase("opened")]
    [TestCase("pressed")]
    public async Task CrownRelocationClosesBlockingInteractionAsync(string interaction)
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var winner = await fixture.CreatePlayerAsync(AttackGuildId, "Winner", CastleSiegeJoinSide.Attack1, 60, 60).ConfigureAwait(false);
        var victim = await fixture.CreatePlayerAsync(DefenseGuildId, "Victim", CastleSiegeJoinSide.Defense, 61, 60, useRespawnPlugIn: false).ConfigureAwait(false);
        var peer = await fixture.CreatePlayerAsync(AttackGuildId, "Peer", CastleSiegeJoinSide.Attack1, 62, 60).ConfigureAwait(false);
        victim.Attributes![Stats.CurrentHealth] = 100;
        peer.Attributes![Stats.CurrentHealth] = 100;
        victim.Money = 200;
        fixture.GameServerContext.Configuration.MaximumInventoryMoney = 1_000_000;
        try
        {
            if (interaction == "party")
            {
                await peer.RunPersistenceExclusiveAsync(() => new PartyRequestAction().HandlePartyRequestAsync(peer, victim)).ConfigureAwait(false);
                Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.PartyRequest));
            }
            else
            {
                Assert.That(await peer.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(peer, victim)).ConfigureAwait(false), Is.True);
                if (interaction != "requested")
                {
                    await victim.RunPersistenceExclusiveAsync(() => new TradeAcceptAction().HandleTradeAcceptAsync(victim, true)).ConfigureAwait(false);
                    await victim.RunPersistenceExclusiveAsync(() => new TradeMoneyAction().TradeMoneyAsync(victim, 60)).ConfigureAwait(false);
                    if (interaction == "pressed")
                    {
                        await victim.RunPersistenceExclusiveAsync(() => new TradeButtonAction().TradeButtonChangedAsync(victim, TradeButtonState.Checked)).ConfigureAwait(false);
                    }
                }
            }

            Mock.Get(victim.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Invocations.Clear();
            await Task.Run(async () => await winner.RunPersistenceExclusiveAsync(
                () => CastleSiegeCrownMechanics.ChangeWinnerGuildAsync(fixture.Context, winner, CastleSiegeJoinSide.Attack1)).ConfigureAwait(false)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.ChangingMap));
                Assert.That(victim.LastPartyRequester, Is.Null);
                Assert.That(victim.TradingPartner, Is.Null);
                Assert.That(peer.TradingPartner, Is.Null);
                Assert.That(victim.BackupInventory, Is.Null);
                Assert.That(victim.TradingMoney, Is.Zero);
                Assert.That(victim.Money, Is.EqualTo(200));
            });
            Mock.Get(victim.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Verify(v => v.MapChangeAsync(), Times.Once);
            await victim.RunPersistenceExclusiveAsync(() => victim.ClientReadyAfterMapChangeAsync()).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(victim.IsAlive, Is.True);
                Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(victim.CurrentMap, Is.SameAs(fixture.SiegeMap));
                Assert.That(fixture.SiegeMap.GetPlayers().Count(p => ReferenceEquals(p, victim)), Is.EqualTo(1));
            });
        }
        finally
        {
            await victim.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
            await winner.DisposeAsync().ConfigureAwait(false);
            fixture.GameServerContext.Dispose();
        }
    }

    /// <summary>Living Crown victims cannot defeat placement by reopening Chaos Goblin during normal close/save.</summary>
    /// <param name="reopen">Whether the victim requests the same dialog after its normal World publication.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task CrownRelocationCannotBeDefeatedByReopenedNpcAsync(bool reopen)
    {
        var beforeSave = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var release = new ManualResetEventSlim();
        var gateHeld = 0;
        var logger = new Mock<ILogger>();
        logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(i =>
            {
                if ((LogLevel)i.Arguments[0] == LogLevel.Information
                    && string.Equals(i.Arguments[2].ToString(), "Saving changes after closing the chaos goblin ...", StringComparison.Ordinal))
                {
                    Volatile.Write(ref gateHeld, 1);
                    beforeSave.TrySetResult();
                    try
                    {
                        if (!release.Wait(TimeSpan.FromSeconds(5)))
                        {
                            throw new TimeoutException("Crown test did not release the pre-save close barrier.");
                        }
                    }
                    finally
                    {
                        Volatile.Write(ref gateHeld, 0);
                    }
                }
            }));
        var factory = new Mock<ILoggerFactory>();
        factory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        var fixture = await CreateFixtureAsync(factory.Object).ConfigureAwait(false);
        var winner = await fixture.CreatePlayerAsync(AttackGuildId, "Winner", CastleSiegeJoinSide.Attack1, 60, 60).ConfigureAwait(false);
        var victim = await fixture.CreatePlayerAsync(DefenseGuildId, "Victim", CastleSiegeJoinSide.Defense, 61, 60, useRespawnPlugIn: false).ConfigureAwait(false);
        var definition = new Mock<MonsterDefinition>();
        definition.SetupAllProperties();
        definition.Setup(d => d.ItemCraftings).Returns(new List<MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting> { new() });
        definition.Object.Number = 238;
        definition.Object.NpcWindow = NpcWindow.ChaosMachine;
        var goblin = new NonPlayerCharacter(new MonsterSpawnArea(), definition.Object, fixture.SiegeMap) { Position = victim.Position };
        await fixture.SiegeMap.AddAsync(goblin).ConfigureAwait(false);
        var packet = new byte[MUnique.OpenMU.Network.Packets.ClientToServer.TalkToNpcRequestRef.Length];
        _ = new MUnique.OpenMU.Network.Packets.ClientToServer.TalkToNpcRequestRef(packet) { NpcId = goblin.Id };
        victim.Attributes![Stats.CurrentHealth] = 100;
        victim.Money = 200;
        await victim.RunPersistenceExclusiveAsync(() => new TalkNpcHandlerPlugIn().HandlePacketAsync(victim, packet)).ConfigureAwait(false);
        Mock.Get(victim.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Invocations.Clear();
        var relocation = Task.Run(async () => await winner.RunPersistenceExclusiveAsync(
            () => CastleSiegeCrownMechanics.ChangeWinnerGuildAsync(fixture.Context, winner, CastleSiegeJoinSide.Attack1)).ConfigureAwait(false));
        try
        {
            await beforeSave.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(Volatile.Read(ref gateHeld), Is.EqualTo(1));
            Assert.That(relocation.IsCompleted, Is.False);
            if (reopen)
            {
                // Even a continuation delayed beyond the old one-second window must hit the held placement.
                await Task.Delay(1100).ConfigureAwait(false);
                for (var attempt = 0; attempt < 8; attempt++)
                {
                    Assert.That(Volatile.Read(ref gateHeld), Is.EqualTo(1));
                    Assert.That(release.IsSet, Is.False);
                    Assert.That(relocation.IsCompleted, Is.False);
                    Assert.That(victim.CurrentMap, Is.SameAs(fixture.SiegeMap));
                    Assert.That(victim.CurrentMap!.GetObject(goblin.Id), Is.SameAs(goblin));
                    // The real pre-save log is outside victim state and persistence exclusion.
                    await victim.RunPersistenceExclusiveAsync(() => new TalkNpcHandlerPlugIn().HandlePacketAsync(victim, packet)).ConfigureAwait(false);
                    Assert.That(Volatile.Read(ref gateHeld), Is.EqualTo(1));
                    Assert.That(relocation.IsCompleted, Is.False);
                }
            }

            release.Set();
            await relocation.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.ChangingMap));
                Assert.That(victim.OpenedNpc, Is.Null);
                Assert.That(victim.BackupInventory, Is.Null);
                Assert.That(victim.Money, Is.EqualTo(200));
            });
            Mock.Get(victim.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Verify(v => v.MapChangeAsync(), Times.Once);
            await victim.RunPersistenceExclusiveAsync(() => victim.ClientReadyAfterMapChangeAsync()).ConfigureAwait(false);
            Assert.That(victim.IsAlive, Is.True);
        }
        finally
        {
            release.Set();
            await relocation.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await fixture.SiegeMap.RemoveAsync(goblin).ConfigureAwait(false);
            await goblin.DisposeAsync().ConfigureAwait(false);
            await victim.DisposeAsync().ConfigureAwait(false);
            await winner.DisposeAsync().ConfigureAwait(false);
            fixture.GameServerContext.Dispose();
        }
    }

    /// <summary>Crown cleanup of a pending trade preserves items left by a closed real Chaos Goblin.</summary>
    [Test]
    public async Task CrownPendingTradePreservesClosedChaosItemsAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var winner = await fixture.CreatePlayerAsync(AttackGuildId, "Winner", CastleSiegeJoinSide.Attack1, 60, 60).ConfigureAwait(false);
        var victim = await fixture.CreatePlayerAsync(DefenseGuildId, "Victim", CastleSiegeJoinSide.Defense, 61, 60, useRespawnPlugIn: false).ConfigureAwait(false);
        var peer = await fixture.CreatePlayerAsync(DefenseGuildId, "Peer", CastleSiegeJoinSide.Defense, 61, 61).ConfigureAwait(false);
        var definition = new Mock<MonsterDefinition>();
        definition.SetupAllProperties();
        definition.Setup(d => d.ItemCraftings).Returns(new List<MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting>
        {
            new() { SimpleCraftingSettings = victim.PersistenceContext.CreateNew<MUnique.OpenMU.DataModel.Configuration.ItemCrafting.SimpleCraftingSettings>() },
        });
        definition.Object.Number = 238;
        definition.Object.NpcWindow = NpcWindow.ChaosMachine;
        var goblin = new NonPlayerCharacter(new MonsterSpawnArea(), definition.Object, fixture.SiegeMap) { Position = victim.Position };
        await fixture.SiegeMap.AddAsync(goblin).ConfigureAwait(false);
        var inventory = victim.Inventory!;
        var temporary = victim.TemporaryStorage!;
        var item = victim.PersistenceContext.CreateNew<Item>();
        item.Definition = new MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition { Width = 1, Height = 1, Durability = 9, IsTradable = true };
        item.Level = 3;
        item.Durability = 7;
        var id = item.GetId();
        Assert.That(id, Is.Not.EqualTo(Guid.Empty));
        var talk = new byte[MUnique.OpenMU.Network.Packets.ClientToServer.TalkToNpcRequestRef.Length];
        _ = new MUnique.OpenMU.Network.Packets.ClientToServer.TalkToNpcRequestRef(talk) { NpcId = goblin.Id };
        try
        {
            victim.Attributes![Stats.CurrentHealth] = 100;
            peer.Attributes![Stats.CurrentHealth] = 100;
            Assert.That(await inventory.AddItemAsync(20, item).ConfigureAwait(false), Is.True);
            await victim.RunPersistenceExclusiveAsync(() => new TalkNpcHandlerPlugIn().HandlePacketAsync(victim, talk)).ConfigureAwait(false);
            await victim.RunPersistenceExclusiveAsync(() => new MUnique.OpenMU.GameLogic.PlayerActions.Items.MoveItemAction().MoveItemAsync(victim, 20, Storages.Inventory, 0, Storages.ChaosMachine)).ConfigureAwait(false);
            Assert.That(temporary.Items.Single(), Is.SameAs(item));
            await victim.RunPersistenceExclusiveAsync(() => new CloseNpcDialogAction().CloseNpcDialogAsync(victim)).ConfigureAwait(false);
            victim.Money = 1234;
            Assert.That(victim.BackupInventory, Is.Null);
            Assert.That(await peer.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(peer, victim)).ConfigureAwait(false), Is.True);
            await winner.RunPersistenceExclusiveAsync(() => CastleSiegeCrownMechanics.ChangeWinnerGuildAsync(fixture.Context, winner, CastleSiegeJoinSide.Attack1)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.ChangingMap));
                Assert.That(victim.TradingPartner, Is.Null);
                Assert.That(peer.TradingPartner, Is.Null);
                Assert.That(inventory.Items.Concat(temporary.Items).Count(i => i.GetId() == id), Is.EqualTo(1));
                Assert.That(victim.Money, Is.EqualTo(1234));
                Assert.That(item.Level, Is.EqualTo(3));
                Assert.That(item.Durability, Is.EqualTo(7));
            });
            await victim.RunPersistenceExclusiveAsync(() => victim.ClientReadyAfterMapChangeAsync()).ConfigureAwait(false);
            await victim.RemoveFromGameAsync().ConfigureAwait(false);
            Assert.That(inventory.Items.Count(i => i.GetId() == id), Is.EqualTo(1));
        }
        finally
        {
            await fixture.SiegeMap.RemoveAsync(goblin).ConfigureAwait(false);
            await goblin.DisposeAsync().ConfigureAwait(false);
            await victim.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
            await winner.DisposeAsync().ConfigureAwait(false);
            fixture.GameServerContext.Dispose();
        }
    }

    /// <summary>A real death timer cannot consume its death before a Crown preparation permits actual placement.</summary>
    /// <param name="overlap">Whether the normal close remains held while the original three-second timer runs.</param>
    /// <param name="legacy">Whether the victim completes placement without a Ready packet.</param>
    [TestCase(false, true)]
    [TestCase(true, true)]
    [TestCase(true, false)]
    public async Task CrownPreparationPreservesDueDeathUntilPlacementClaimAsync(bool overlap, bool legacy)
    {
        var closing = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var releaseClose = new ManualResetEventSlim();
        var held = 0;
        var logger = new Mock<ILogger>();
        logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(i =>
            {
                if ((LogLevel)i.Arguments[0] == LogLevel.Information && string.Equals(i.Arguments[2].ToString(), "Saving changes after closing the chaos goblin ...", StringComparison.Ordinal))
                {
                    Volatile.Write(ref held, 1);
                    closing.TrySetResult();
                    try
                    {
                        if (!releaseClose.Wait(TimeSpan.FromSeconds(10)))
                        {
                            throw new TimeoutException("Crown preparation close was not released.");
                        }
                    }
                    finally
                    {
                        Volatile.Write(ref held, 0);
                    }
                }
            }));
        var factory = new Mock<ILoggerFactory>();
        factory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        var fixture = await CreateFixtureAsync(factory.Object).ConfigureAwait(false);
        var winner = await fixture.CreatePlayerAsync(AttackGuildId, "Winner", CastleSiegeJoinSide.Attack1, 60, 60).ConfigureAwait(false);
        var victim = await fixture.CreatePlayerAsync(DefenseGuildId, "Victim", CastleSiegeJoinSide.Defense, 61, 60, useRespawnPlugIn: legacy).ConfigureAwait(false);
        var npcDefinition = new Mock<MonsterDefinition>();
        npcDefinition.SetupAllProperties();
        npcDefinition.Setup(n => n.ItemCraftings).Returns(new List<MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting> { new() });
        npcDefinition.Object.Number = 238;
        npcDefinition.Object.NpcWindow = NpcWindow.ChaosMachine;
        var npc = new NonPlayerCharacter(new MonsterSpawnArea(), npcDefinition.Object, fixture.SiegeMap) { Position = victim.Position };
        await fixture.SiegeMap.AddAsync(npc).ConfigureAwait(false);
        var packet = new byte[MUnique.OpenMU.Network.Packets.ClientToServer.TalkToNpcRequestRef.Length];
        _ = new MUnique.OpenMU.Network.Packets.ClientToServer.TalkToNpcRequestRef(packet) { NpcId = npc.Id };
        await victim.RunPersistenceExclusiveAsync(() => new TalkNpcHandlerPlugIn().HandlePacketAsync(victim, packet)).ConfigureAwait(false);
        victim.Attributes![Stats.CurrentHealth] = 10;
        var hitEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseHit = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Mock.Get(victim.ViewPlugIns.GetPlugIn<IShowHitPlugIn>()!).Setup(v => v.ShowHitAsync(victim, It.IsAny<HitInfo>())).Returns(async () =>
        {
            hitEntered.TrySetResult();
            await releaseHit.Task.ConfigureAwait(false);
        });
        var timerGate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var gateSelection = new Mock<IPlayerSpawnGateSelectionPlugIn>();
        gateSelection.Setup(p => p.SelectSpawnGateAsync(victim, It.IsAny<SpawnGateSelectionArgs>())).Returns((Player _, SpawnGateSelectionArgs args) =>
        {
            args.Gate = fixture.Context.GetRespawnGate(CastleSiegeJoinSide.Attack1);
            timerGate.TrySetResult();
            return ValueTask.CompletedTask;
        });
        fixture.GameServerContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerSpawnGateSelectionPlugIn>(gateSelection.Object);
        var deaths = 0;
        victim.Died += (_, _) => Interlocked.Increment(ref deaths);
        var death = Task.Run(async () => await winner.RunPersistenceExclusiveAsync(() => victim.ApplyPoisonDamageAsync(winner, 10)).ConfigureAwait(false));
        var crown = Task.CompletedTask;
        try
        {
            await hitEntered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            var owner = victim.LastDeath;
            crown = Task.Run(async () => await CastleSiegeCrownMechanics.ChangeWinnerGuildAsync(fixture.Context, winner, CastleSiegeJoinSide.Attack1).ConfigureAwait(false));
            await closing.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(victim.IsPreparingForcedPlacement, Is.True);
            if (!overlap)
            {
                releaseClose.Set();
                await crown.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            releaseHit.TrySetResult();
            await death.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (overlap)
            {
                // Observe the REAL timer's preparation outcome, not an invented respawn call or veto.
                await timerGate.Task.WaitAsync(TimeSpan.FromSeconds(6)).ConfigureAwait(false);
                // Public HP/reservation cannot prove the real timer reached its defer-or-commit decision.
                // Copy private phase values under state; never retain or mutate the accessor refs.
                CancellationTokenSource? observedSource = null;
                var observed = false;
                var started = Stopwatch.StartNew();
                while (!observed && started.Elapsed < TimeSpan.FromSeconds(5))
                {
                    await using (var context = await victim.PlayerState.TryBeginAdvanceToAsync(victim.PlayerState.CurrentState).ConfigureAwait(false))
                    {
                        context.Allowed = false;
                        var pending = PendingDeathRespawn(victim);
                        observedSource = DeathRespawnCancellation(victim);
                        observed = !victim.GetPlacementDeathSnapshot().Reserved || pending is { Claimed: false };
                    }

                    if (!observed)
                    {
                        await Task.Delay(10).ConfigureAwait(false);
                    }
                }

                Assert.That(observed, Is.True, "Timer must either defer its claim or expose the unsafe early ownership release.");
                Assert.That(Volatile.Read(ref held), Is.EqualTo(1));
                TestContext.Progress.WriteLine($"Held Crown: state={victim.PlayerState.CurrentState}, alive={victim.IsAlive}, HP={victim.Attributes[Stats.CurrentHealth]}, reserved={victim.GetPlacementDeathSnapshot().Reserved}, CTS={observedSource is not null}");
                Assert.Multiple(() =>
                {
                    Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                    Assert.That(victim.IsAlive, Is.False);
                    Assert.That(victim.GetPlacementDeathSnapshot(), Is.EqualTo((owner, true)), "No death consumption before a placement owns its claim.");
                    Assert.That(observedSource, Is.Not.Null);
                    Assert.That(victim.Attributes[Stats.CurrentHealth], Is.Zero);
                });
                releaseClose.Set();
                await crown.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            var complete = Stopwatch.StartNew();
            while ((!victim.IsAlive || victim.PlayerState.CurrentState == PlayerState.ChangingMap) && complete.Elapsed < TimeSpan.FromSeconds(6))
            {
                if (victim.PlayerState.CurrentState == PlayerState.ChangingMap && victim.CurrentMap is null)
                {
                    await victim.RunPersistenceExclusiveAsync(() => victim.ClientReadyAfterMapChangeAsync()).ConfigureAwait(false);
                }

                await Task.Delay(10).ConfigureAwait(false);
            }

            Assert.Multiple(() =>
            {
                Assert.That(victim.IsAlive, Is.True);
                Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(victim.Attributes[Stats.CurrentHealth], Is.EqualTo(victim.Attributes[Stats.MaximumHealth]));
                Assert.That(victim.LastDeath, Is.SameAs(owner));
                Assert.That(deaths, Is.EqualTo(1));
                Assert.That(victim.IsPreparingForcedPlacement, Is.False);
            });
        }
        finally
        {
            releaseHit.TrySetResult();
            releaseClose.Set();
            await Task.WhenAll(death, crown).WaitAsync(TimeSpan.FromSeconds(6)).ConfigureAwait(false);
            await fixture.SiegeMap.RemoveAsync(npc).ConfigureAwait(false);
            await npc.DisposeAsync().ConfigureAwait(false);
            await victim.DisposeAsync().ConfigureAwait(false);
            await winner.DisposeAsync().ConfigureAwait(false);
            fixture.GameServerContext.Dispose();
        }
    }

    /// <summary>Actual mid-warp selection excludes CurrentMap-null; a completed Ready makes Crown relocation eligible again.</summary>
    /// <param name="readyBeforeCrown">Whether the ordinary Ready packet completed before capture.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task CrownSelectionFollowsRealWarpReadyMembershipAsync(bool readyBeforeCrown)
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var winner = await fixture.CreatePlayerAsync(AttackGuildId, "Winner", CastleSiegeJoinSide.Attack1, 60, 60).ConfigureAwait(false);
        var victim = await fixture.CreatePlayerAsync(DefenseGuildId, "Victim", CastleSiegeJoinSide.Defense, 61, 60, useRespawnPlugIn: false).ConfigureAwait(false);
        var gate = new ExitGate { Map = fixture.SiegeMap.Definition, X1 = 62, X2 = 62, Y1 = 61, Y2 = 61 };
        Mock.Get(victim.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Invocations.Clear();
        try
        {
            await victim.RunPersistenceExclusiveAsync(() => new WarpAction().WarpToAsync(victim, new WarpInfo { Gate = gate })).ConfigureAwait(false);
            if (readyBeforeCrown)
            {
                await victim.RunPersistenceExclusiveAsync(() => victim.ClientReadyAfterMapChangeAsync()).ConfigureAwait(false);
            }

            Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(readyBeforeCrown ? PlayerState.EnteredWorld : PlayerState.ChangingMap));
            Assert.That(fixture.Context.GetSiegePlayers().Contains(victim), Is.EqualTo(readyBeforeCrown));
            await winner.RunPersistenceExclusiveAsync(() => CastleSiegeCrownMechanics.ChangeWinnerGuildAsync(fixture.Context, winner, CastleSiegeJoinSide.Attack1)).ConfigureAwait(false);
            if (readyBeforeCrown)
            {
                var respawn = fixture.Context.GetRespawnGate(CastleSiegeJoinSide.Attack1)!;
                Assert.That(victim.SelectedCharacter!.PositionX, Is.InRange(respawn.X1, respawn.X2));
                Assert.That(victim.SelectedCharacter.PositionY, Is.InRange(respawn.Y1, respawn.Y2));
            }
            else
            {
                Assert.That(victim.SelectedCharacter!.PositionX, Is.EqualTo(62));
                Assert.That(victim.SelectedCharacter.PositionY, Is.EqualTo(61));
            }

            Mock.Get(victim.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Verify(v => v.MapChangeAsync(), Times.Exactly(readyBeforeCrown ? 2 : 1));
            await victim.RunPersistenceExclusiveAsync(() => victim.ClientReadyAfterMapChangeAsync()).ConfigureAwait(false);
            Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            Assert.That(victim.IsAlive, Is.True);
            Assert.That(fixture.SiegeMap.GetPlayers().Count(p => ReferenceEquals(p, victim)), Is.EqualTo(1));
        }
        finally
        {
            await victim.DisposeAsync().ConfigureAwait(false);
            await winner.DisposeAsync().ConfigureAwait(false);
            fixture.GameServerContext.Dispose();
        }
    }

    /// <summary>A later captured attacker may start an allowed move while an earlier snapshot entry awaits normal NPC close.</summary>
    /// <param name="readyBeforeResume">Whether that move finishes Ready before the Crown loop resumes.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task CrownSnapshotRetainsFirstWarpOwnerAsync(bool readyBeforeResume)
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var release = new ManualResetEventSlim();
        var held = 0;
        var logger = new Mock<ILogger>();
        logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(i =>
            {
                if ((LogLevel)i.Arguments[0] == LogLevel.Information && string.Equals(i.Arguments[2].ToString(), "Saving changes after closing the chaos goblin ...", StringComparison.Ordinal))
                {
                    Volatile.Write(ref held, 1);
                    entered.TrySetResult();
                    try
                    {
                        if (!release.Wait(TimeSpan.FromSeconds(5)))
                        {
                            throw new TimeoutException("Crown snapshot test did not release the earlier NPC close.");
                        }
                    }
                    finally
                    {
                        Volatile.Write(ref held, 0);
                    }
                }
            }));
        var factory = new Mock<ILoggerFactory>();
        factory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        var fixture = await CreateFixtureAsync(factory.Object).ConfigureAwait(false);
        var winner = await fixture.CreatePlayerAsync(AttackGuildId, "Winner", CastleSiegeJoinSide.Attack1, 60, 60).ConfigureAwait(false);
        var first = await fixture.CreatePlayerAsync(DefenseGuildId, "First", CastleSiegeJoinSide.Defense, 61, 60, useRespawnPlugIn: false).ConfigureAwait(false);
        var second = await fixture.CreatePlayerAsync(DefenseGuildId, "Second", CastleSiegeJoinSide.Defense, 62, 60, useRespawnPlugIn: false).ConfigureAwait(false);
        var ordered = fixture.Context.GetSiegePlayers().Where(p => !ReferenceEquals(p, winner)).ToArray();
        var blocker = ordered[0];
        var mover = ordered[1];
        var definition = new Mock<MonsterDefinition>();
        definition.SetupAllProperties();
        definition.Setup(d => d.ItemCraftings).Returns(new List<MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting> { new() });
        definition.Object.Number = 238;
        definition.Object.NpcWindow = NpcWindow.ChaosMachine;
        var npc = new NonPlayerCharacter(new MonsterSpawnArea(), definition.Object, fixture.SiegeMap) { Position = blocker.Position };
        await fixture.SiegeMap.AddAsync(npc).ConfigureAwait(false);
        var talk = new byte[MUnique.OpenMU.Network.Packets.ClientToServer.TalkToNpcRequestRef.Length];
        _ = new MUnique.OpenMU.Network.Packets.ClientToServer.TalkToNpcRequestRef(talk) { NpcId = npc.Id };
        await blocker.RunPersistenceExclusiveAsync(() => new TalkNpcHandlerPlugIn().HandlePacketAsync(blocker, talk)).ConfigureAwait(false);
        // An explicitly permitted configured move, not a direct field mutation or unlisted destination.
        using var configurationContext = fixture.PersistenceContextProvider.CreateNewConfigurationContext();
        var gate = configurationContext.CreateNew<ExitGate>();
        gate.Map = fixture.SiegeMap.Definition;
        gate.X1 = gate.X2 = 62;
        gate.Y1 = gate.Y2 = 61;
        var warpInfo = configurationContext.CreateNew<WarpInfo>();
        warpInfo.Index = 777;
        warpInfo.Gate = gate;
        warpInfo.Costs = 5;
        warpInfo.LevelRequirement = 0;
        fixture.GameServerContext.Configuration.WarpList.Add(warpInfo);
        fixture.GameServerContext.Configuration.MaximumInventoryMoney = 1_000_000;
        mover.Money = 200;
        var move = new byte[MUnique.OpenMU.Network.Packets.ClientToServer.WarpCommandRequestRef.Length];
        _ = new MUnique.OpenMU.Network.Packets.ClientToServer.WarpCommandRequestRef(move) { WarpInfoIndex = 777 };
        Mock.Get(mover.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Invocations.Clear();
        var crown = Task.Run(async () => await CastleSiegeCrownMechanics.ChangeWinnerGuildAsync(fixture.Context, winner, CastleSiegeJoinSide.Attack1).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(Volatile.Read(ref held), Is.EqualTo(1));
            Assert.That(mover.CurrentMap, Is.SameAs(fixture.SiegeMap), "The later attacker was present when the foreach snapshot was taken.");
            Assert.That(mover.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            await mover.RunPersistenceExclusiveAsync(() => new WarpHandlerPlugIn().HandlePacketAsync(mover, move)).ConfigureAwait(false);
            Assert.That(mover.Money, Is.EqualTo(195), "The actual configured move handler accepted the gate and charged its cost.");
            Assert.That(mover.CurrentMap, Is.Null);
            Assert.That(mover.PlayerState.CurrentState, Is.EqualTo(PlayerState.ChangingMap));
            if (readyBeforeResume)
            {
                await mover.RunPersistenceExclusiveAsync(() => mover.ClientReadyAfterMapChangeAsync()).ConfigureAwait(false);
            }

            Assert.That(Volatile.Read(ref held), Is.EqualTo(1));
            release.Set();
            await crown.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (readyBeforeResume)
            {
                var respawn = fixture.Context.GetRespawnGate(CastleSiegeJoinSide.Attack1)!;
                Assert.That(mover.SelectedCharacter!.PositionX, Is.InRange(respawn.X1, respawn.X2));
                Assert.That(mover.SelectedCharacter.PositionY, Is.InRange(respawn.Y1, respawn.Y2));
            }
            else
            {
                Assert.That(mover.SelectedCharacter!.PositionX, Is.EqualTo(62), "Existing ChangingMap owner is intentionally not overwritten, even from an older snapshot.");
                Assert.That(mover.SelectedCharacter.PositionY, Is.EqualTo(61));
            }

            Mock.Get(mover.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Verify(v => v.MapChangeAsync(), Times.Exactly(readyBeforeResume ? 2 : 1));
            await mover.RunPersistenceExclusiveAsync(() => mover.ClientReadyAfterMapChangeAsync()).ConfigureAwait(false);
            await blocker.RunPersistenceExclusiveAsync(() => blocker.ClientReadyAfterMapChangeAsync()).ConfigureAwait(false);
            Assert.That(mover.IsAlive, Is.True);
            Assert.That(mover.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        }
        finally
        {
            release.Set();
            await crown.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await fixture.SiegeMap.RemoveAsync(npc).ConfigureAwait(false);
            await npc.DisposeAsync().ConfigureAwait(false);
            await first.DisposeAsync().ConfigureAwait(false);
            await second.DisposeAsync().ConfigureAwait(false);
            await winner.DisposeAsync().ConfigureAwait(false);
            fixture.GameServerContext.Dispose();
        }
    }

    /// <summary>Crown relocation is placement, not permission to discard another hit's lethal reservation.</summary>
    /// <param name="lethal">Whether the former defender is waiting on a lethal hit view.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task CrownRelocationPreservesReservedDeathAsync(bool lethal)
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var winner = await fixture.CreatePlayerAsync(AttackGuildId, "Winner", CastleSiegeJoinSide.Attack1, 60, 60).ConfigureAwait(false);
        var victim = await fixture.CreatePlayerAsync(DefenseGuildId, "Victim", CastleSiegeJoinSide.Defense, 61, 60).ConfigureAwait(false);
        winner.Id = 11;
        victim.Attributes![Stats.CurrentHealth] = 10;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var deaths = 0;
        victim.Died += (_, _) => Interlocked.Increment(ref deaths);
        Mock.Get(victim.ViewPlugIns.GetPlugIn<IShowHitPlugIn>()!).Setup(v => v.ShowHitAsync(victim, It.IsAny<HitInfo>())).Returns(async () =>
        {
            entered.TrySetResult();
            await release.Task.ConfigureAwait(false);
        });
        var hit = lethal ? victim.ApplyPoisonDamageAsync(winner, 10).AsTask() : Task.CompletedTask;
        try
        {
            if (lethal)
            {
                await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            var owned = victim.LastDeath;
            await CastleSiegeCrownMechanics.ChangeWinnerGuildAsync(fixture.Context, winner, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
            release.TrySetResult();
            await hit.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(victim.Attributes[Stats.CurrentHealth], Is.EqualTo(lethal ? 0 : 10));
                Assert.That(victim.IsAlive, Is.EqualTo(!lethal));
                Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(lethal ? PlayerState.Dead : PlayerState.EnteredWorld));
                Assert.That(victim.LastDeath, Is.SameAs(owned));
                Assert.That(deaths, Is.EqualTo(lethal ? 1 : 0));
            });
            if (lethal)
            {
                Mock.Get(victim.ViewPlugIns.GetPlugIn<IObjectGotKilledPlugIn>()!).Verify(v => v.ObjectGotKilledAsync(victim, winner), Times.Once);
            }
        }
        finally
        {
            release.TrySetResult();
            await hit.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await victim.DisposeAsync().ConfigureAwait(false);
            await winner.DisposeAsync().ConfigureAwait(false);
            fixture.GameServerContext.Dispose();
        }
    }

    /// <summary>Crown placement keeps the processed death's original timer, including a paused killed view.</summary>
    /// <param name="pauseKilledView">Whether relocation precedes completion of the killed notification.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task CrownRelocationKeepsOriginalRespawnTimerAsync(bool pauseKilledView)
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var winner = await fixture.CreatePlayerAsync(AttackGuildId, "Winner", CastleSiegeJoinSide.Attack1, 60, 60).ConfigureAwait(false);
        var victim = await fixture.CreatePlayerAsync(DefenseGuildId, "Victim", CastleSiegeJoinSide.Defense, 61, 60).ConfigureAwait(false);
        victim.Attributes![Stats.CurrentHealth] = 10;
        fixture.SiegeMap.Definition.ExitGates.Add(new BasicModel.ExitGate { Map = fixture.SiegeMap.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1, IsSpawnGate = true });
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var restored = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var placements = 0;
        var deaths = 0;
        victim.Died += (_, _) => Interlocked.Increment(ref deaths);
        Mock.Get(victim.ViewPlugIns.GetPlugIn<IObjectGotKilledPlugIn>()!).Setup(v => v.ObjectGotKilledAsync(victim, winner)).Returns(async () =>
        {
            entered.TrySetResult();
            if (pauseKilledView)
            {
                await release.Task.ConfigureAwait(false);
            }
        });
        Mock.Get(victim.ViewPlugIns.GetPlugIn<IRespawnAfterDeathPlugIn>()!).Setup(v => v.RespawnAsync()).Returns(() =>
        {
            Interlocked.Increment(ref placements);
            if (victim.Attributes![Stats.CurrentHealth] == victim.Attributes[Stats.MaximumHealth])
            {
                restored.TrySetResult();
            }

            return ValueTask.CompletedTask;
        });
        Mock.Get(victim.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Invocations.Clear();
        var lethal = victim.ApplyPoisonDamageAsync(winner, 10).AsTask();
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (!pauseKilledView)
            {
                await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            var owned = victim.LastDeath;
            var relocation = CastleSiegeCrownMechanics.ChangeWinnerGuildAsync(fixture.Context, winner, CastleSiegeJoinSide.Attack1).AsTask();
            release.TrySetResult();
            await relocation.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(victim.IsAlive, Is.False);
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await restored.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await Task.Delay(50).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(victim.IsAlive, Is.True);
                Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(victim.Attributes[Stats.CurrentHealth], Is.EqualTo(victim.Attributes[Stats.MaximumHealth]));
                Assert.That(victim.LastDeath, Is.SameAs(owned));
                Assert.That(deaths, Is.EqualTo(1));
                Assert.That(placements, Is.EqualTo(1));
            });
            Mock.Get(victim.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Verify(v => v.MapChangeAsync(), Times.Once);
        }
        finally
        {
            release.TrySetResult();
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await victim.DisposeAsync().ConfigureAwait(false);
            await winner.DisposeAsync().ConfigureAwait(false);
            fixture.GameServerContext.Dispose();
        }
    }

    /// <summary>Observe actual legacy and newer Crown relocation packets without changing original death ownership.</summary>
    /// <param name="legacy">Whether the concrete075 respawn view is present.</param>
    /// <param name="lethal">Whether an actual death timer is pending.</param>
    /// <param name="initialHealth">Current health before the optional lethal hit.</param>
    /// <param name="relocate">Whether Crown relocates before the original death timer.</param>
    /// <param name="beforeRelocationMilliseconds">Elapsed real timer time before Crown capture.</param>
    [TestCase(true, true, 10, true, 0)]
    [TestCase(false, true, 10, true, 0)]
    [TestCase(true, false, 10, true, 0)]
    [TestCase(false, false, 10, true, 0)]
    [TestCase(true, false, 0, true, 0)]
    [TestCase(false, false, 0, true, 0)]
    [TestCase(true, true, 10, false, 0)]
    [TestCase(false, true, 10, false, 0)]
    [TestCase(true, true, 10, true, 1500)]
    public async Task CrownReservedDeathWireObservationAsync(bool legacy, bool lethal, int initialHealth, bool relocate, int beforeRelocationMilliseconds)
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var winner = await fixture.CreatePlayerAsync(AttackGuildId, "Winner", CastleSiegeJoinSide.Attack1, 60, 60).ConfigureAwait(false);
        using var output = new System.IO.MemoryStream();
        var writer = System.IO.Pipelines.PipeWriter.Create(output, new System.IO.Pipelines.StreamPipeWriterOptions(leaveOpen: true));
        var connection = new Mock<MUnique.OpenMU.Network.IConnection>();
        connection.SetupGet(c => c.Connected).Returns(true);
        connection.SetupGet(c => c.Output).Returns(writer);
        connection.SetupGet(c => c.OutputLock).Returns(new Nito.AsyncEx.AsyncLock());
        var restoredPacket = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var victim = await fixture.CreatePlayerAsync(DefenseGuildId, "Victim", CastleSiegeJoinSide.Defense, 61, 60,
            createPlayer: context => new CrownWirePlayer((IGameServerContext)context, connection.Object, legacy, () => restoredPacket.TrySetResult())).ConfigureAwait(false);
        fixture.SiegeMap.Definition.ExitGates.Add(new BasicModel.ExitGate { Map = fixture.SiegeMap.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1, IsSpawnGate = true });
        victim.Attributes![Stats.CurrentHealth] = initialHealth;
        var enteredAlive = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        ValueTask ObserveArrivalAsync((GameMap Map, ILocateable Object) arrival)
        {
            if (ReferenceEquals(arrival.Object, victim) && victim.IsAlive && victim.Attributes[Stats.CurrentHealth] == victim.Attributes[Stats.MaximumHealth])
            {
                enteredAlive.TrySetResult();
            }

            return ValueTask.CompletedTask;
        }

        fixture.SiegeMap.ObjectAdded += ObserveArrivalAsync;
        var start = checked((int)output.Length);
        var elapsed = Stopwatch.StartNew();
        var deaths = 0;
        victim.Died += (_, _) => Interlocked.Increment(ref deaths);
        try
        {
            if (lethal)
            {
                await victim.ApplyPoisonDamageAsync(winner, (uint)initialHealth).ConfigureAwait(false);
            }

            if (beforeRelocationMilliseconds > 0)
            {
                await Task.Delay(beforeRelocationMilliseconds).ConfigureAwait(false);
            }

            var death = victim.LastDeath;
            CancellationTokenSource? source;
            await using (var scope = await victim.PlayerState.TryBeginAdvanceToAsync(victim.PlayerState.CurrentState).ConfigureAwait(false))
            {
                scope.Allowed = false;
                source = DeathRespawnCancellation(victim);
            }

            if (relocate)
            {
                await winner.RunPersistenceExclusiveAsync(() => CastleSiegeCrownMechanics.ChangeWinnerGuildAsync(fixture.Context, winner, CastleSiegeJoinSide.Attack1)).ConfigureAwait(false);
            }

            CancellationTokenSource? after;
            await using (var scope = await victim.PlayerState.TryBeginAdvanceToAsync(victim.PlayerState.CurrentState).ConfigureAwait(false))
            {
                scope.Allowed = false;
                after = DeathRespawnCancellation(victim);
            }

            var immediate = ReadCrownPackets(output.ToArray().AsSpan(start));
            var genericPacket = legacy ? "MapChanged075" : "MapChanged";
            if (legacy && !lethal)
            {
                genericPacket = $"Respawn075:{initialHealth}";
            }

            var expectedImmediate = relocate ? new[] { genericPacket } : Array.Empty<string>();
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(new { legacy, lethal, initialHealth, relocate, elapsedMilliseconds = elapsed.ElapsedMilliseconds, immediate, alive = victim.IsAlive, state = victim.PlayerState.CurrentState.ToString(), hp = victim.Attributes[Stats.CurrentHealth], sameSource = ReferenceEquals(source, after), reserved = victim.GetPlacementDeathSnapshot().Reserved }));
            Assert.Multiple(() =>
            {
                Assert.That(victim.LastDeath, Is.SameAs(death));
                Assert.That(after, Is.SameAs(source));
                Assert.That(victim.GetPlacementDeathSnapshot().Reserved, Is.EqualTo(lethal));
                Assert.That(victim.Attributes[Stats.CurrentHealth], Is.EqualTo(lethal ? 0 : initialHealth));
                Assert.That(victim.IsAlive, Is.EqualTo(!lethal && legacy));
                Assert.That(immediate, Is.EqualTo(expectedImmediate));
            });
            if (legacy && lethal && relocate)
            {
                MUnique.OpenMU.Network.Packets.ServerToClient.MapChanged075Ref mapChange = output.ToArray().AsSpan(start, MUnique.OpenMU.Network.Packets.ServerToClient.MapChanged075Ref.Length);
                Assert.That(mapChange.IsMapChange, Is.True);
                Assert.That(mapChange.MapNumber, Is.EqualTo(victim.SelectedCharacter!.CurrentMap!.Number));
                Assert.That(mapChange.PositionX, Is.EqualTo(victim.Position.X));
                Assert.That(mapChange.PositionY, Is.EqualTo(victim.Position.Y));
            }

            if (!legacy && relocate)
            {
                var ready = new byte[MUnique.OpenMU.Network.Packets.ClientToServer.ClientReadyAfterMapChangeRef.Length];
                _ = new MUnique.OpenMU.Network.Packets.ClientToServer.ClientReadyAfterMapChangeRef(ready);
                await victim.RunPersistenceExclusiveAsync(() => new MUnique.OpenMU.GameServer.MessageHandler.Character.CharacterClientReadyPacketHandlerPlugIn().HandlePacketAsync(victim, ready)).ConfigureAwait(false);
            }

            Assert.That(victim.CurrentMap, Is.SameAs(fixture.SiegeMap));
            if (lethal)
            {
                await restoredPacket.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                if (!legacy)
                {
                    await victim.RunPersistenceExclusiveAsync(() => victim.ClientReadyAfterMapChangeAsync()).ConfigureAwait(false);
                }

                await enteredAlive.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                Assert.That(victim.Attributes[Stats.CurrentHealth], Is.EqualTo(victim.Attributes[Stats.MaximumHealth]));
                Assert.That(victim.IsAlive, Is.True);
                Assert.That(victim.GetPlacementDeathSnapshot().Reserved, Is.False);
                Assert.That(victim.LastDeath, Is.SameAs(death));
                Assert.That(deaths, Is.EqualTo(1));
            }

            var finalPackets = ReadCrownPackets(output.ToArray().AsSpan(start));
            if (legacy && lethal)
            {
                Assert.That(finalPackets.Count(p => p.StartsWith("Respawn075:", StringComparison.Ordinal)), Is.EqualTo(1));
                Assert.That(finalPackets.Last(), Is.EqualTo($"Respawn075:{victim.Attributes[Stats.MaximumHealth]}"));
            }

            Assert.That(fixture.SiegeMap.GetPlayers().Count(p => ReferenceEquals(p, victim)), Is.EqualTo(1));
            Console.WriteLine(Convert.ToHexString(output.ToArray().AsSpan(start)));
            Console.WriteLine($"{string.Join(",", finalPackets)}; elapsedMs={elapsed.ElapsedMilliseconds}");
        }
        finally
        {
            fixture.SiegeMap.ObjectAdded -= ObserveArrivalAsync;
            await victim.DisposeAsync().ConfigureAwait(false);
            await winner.DisposeAsync().ConfigureAwait(false);
            await writer.CompleteAsync().ConfigureAwait(false);
            fixture.GameServerContext.Dispose();
        }
    }

    /// <summary>The real version-selected container registers the075 map and respawn producers.</summary>
    [Test]
    public async Task LegacyMapViewSelectionIsAvailableAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer(fixture.GameServerContext, new MUnique.OpenMU.Network.PlugIns.ClientVersion(0, 75, MUnique.OpenMU.Network.PlugIns.ClientLanguage.English));
        try
        {
            Assert.That(player.ViewPlugIns.GetPlugIn<IMapChangePlugIn>(), Is.TypeOf<MUnique.OpenMU.GameServer.RemoteView.World.MapChangePlugIn075>());
            Assert.That(player.ViewPlugIns.GetPlugIn<IRespawnAfterDeathPlugIn>(), Is.TypeOf<MUnique.OpenMU.GameServer.RemoteView.World.RespawnAfterDeathPlugIn075>());
        }
        finally
        {
            await player.DisposeAsync().ConfigureAwait(false);
            output.Dispose();
            fixture.GameServerContext.Dispose();
        }
    }

    private static List<string> ReadCrownPackets(ReadOnlySpan<byte> bytes)
    {
        var packets = new List<string>();
        for (var offset = 0; offset < bytes.Length; offset += bytes[offset + 1])
        {
            Assert.That(bytes.Length - offset, Is.GreaterThanOrEqualTo(4));
            Assert.That(bytes[offset + 1], Is.GreaterThan(3));
            Assert.That(offset + bytes[offset + 1], Is.LessThanOrEqualTo(bytes.Length));
            if (bytes[offset + 2] == 0xF3)
            {
                Assert.That(bytes[offset + 1], Is.EqualTo(MUnique.OpenMU.Network.Packets.ServerToClient.RespawnAfterDeath075Ref.Length));
                Assert.That(bytes[offset + 3], Is.EqualTo(4));
                packets.Add($"Respawn075:{System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(bytes[(offset + 8)..])}");
            }
            else
            {
                Assert.That(bytes[offset + 2], Is.EqualTo(0x1C));
                Assert.That(bytes[offset], Is.EqualTo(0xC3));
                if (bytes[offset + 1] == 8)
                {
                    packets.Add("MapChanged075");
                }
                else
                {
                    Assert.That(bytes[offset + 1], Is.EqualTo(MUnique.OpenMU.Network.Packets.ServerToClient.MapChangedRef.Length));
                    packets.Add("MapChanged");
                }
            }
        }

        return packets;
    }

    private sealed class CrownWirePlayer(IGameServerContext context, MUnique.OpenMU.Network.IConnection connection, bool legacy, Action restored)
        : MUnique.OpenMU.GameServer.RemoteView.RemotePlayer(context, connection, new MUnique.OpenMU.Network.PlugIns.ClientVersion((byte)(legacy ? 0 : 6), (byte)(legacy ? 75 : 3), MUnique.OpenMU.Network.PlugIns.ClientLanguage.English))
    {
        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer() => new CrownWireViews(this, legacy, restored);
    }

    private sealed class CrownWireViews(MUnique.OpenMU.GameServer.RemoteView.RemotePlayer player, bool legacy, Action restored) : ICustomPlugInContainer<IViewPlugIn>
    {
        private readonly MockViewPlugInContainer _other = new();
        public T? GetPlugIn<T>() where T : class, IViewPlugIn
        {
            if (typeof(T) == typeof(IRespawnAfterDeathPlugIn))
            {
                return legacy ? (T)(IViewPlugIn)new ObservedCrownRespawn(player, restored) : null;
            }

            return typeof(T) == typeof(IMapChangePlugIn) ? (T)(IViewPlugIn)new ObservedCrownMapChange(player, legacy, restored) : this._other.GetPlugIn<T>();
        }
    }

    private sealed class ObservedCrownRespawn(MUnique.OpenMU.GameServer.RemoteView.RemotePlayer player, Action restored) : IRespawnAfterDeathPlugIn
    {
        public async ValueTask RespawnAsync()
        {
            await new MUnique.OpenMU.GameServer.RemoteView.World.RespawnAfterDeathPlugIn075(player).RespawnAsync().ConfigureAwait(false);
            if (player.Attributes![Stats.CurrentHealth] == player.Attributes[Stats.MaximumHealth])
            {
                restored();
            }
        }
    }

    private sealed class ObservedCrownMapChange(MUnique.OpenMU.GameServer.RemoteView.RemotePlayer player, bool legacy, Action restored) : IMapChangePlugIn
    {
        public async ValueTask MapChangeAsync()
        {
            if (legacy)
            {
                await new MUnique.OpenMU.GameServer.RemoteView.World.MapChangePlugIn075(player).MapChangeAsync().ConfigureAwait(false);
            }
            else
            {
                await new MUnique.OpenMU.GameServer.RemoteView.World.MapChangePlugIn(player).MapChangeAsync().ConfigureAwait(false);
            }

            if (player.Attributes![Stats.CurrentHealth] == player.Attributes[Stats.MaximumHealth])
            {
                restored();
            }
        }

        public ValueTask MapChangeFailedAsync() => ValueTask.CompletedTask;
    }

    [System.Runtime.CompilerServices.UnsafeAccessor(System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_pendingDeathRespawn")]
    private static extern ref (CancellationTokenSource Source, DeathInformation Death, bool Claimed)? PendingDeathRespawn(Player player);

    [System.Runtime.CompilerServices.UnsafeAccessor(System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_respawnAfterDeathCts")]
    private static extern ref CancellationTokenSource? DeathRespawnCancellation(Player player);

    private static async ValueTask<TestFixture> CreateFixtureAsync(ILoggerFactory? loggerFactory = null)
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
            loggerFactory ?? NullLoggerFactory.Instance,
            plugInManager,
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameServerContext.PlugInManager;
        mapInitializer.PathFinderPool = gameServerContext.PathFinderPool;

        var initializationTimeUtc = new DateTime(2026, 8, 3, 12, 0, 0, DateTimeKind.Utc);
        var context = new CastleSiegeContext(gameServerContext, configuration);
        await context.InitializeAsync(initializationTimeUtc).ConfigureAwait(false);
        context.CurrentState = CastleSiegeState.Start;
        context.AddSelectedGuild(new CastleSiegeGuildParticipant
        {
            GuildId = DefenseGuildId,
            PersistentGuildId = defensePersistentGuildId,
            GuildName = "Defenders",
            Side = CastleSiegeJoinSide.Defense,
            IsAllianceMaster = true,
        });
        context.AddSelectedGuild(new CastleSiegeGuildParticipant
        {
            GuildId = AttackGuildId,
            PersistentGuildId = attackPersistentGuildId,
            GuildName = "Attackers",
            Side = CastleSiegeJoinSide.Attack1,
            IsAllianceMaster = true,
        });
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
            byte y,
            bool useRespawnPlugIn = true,
            Func<IGameContext, Player>? createPlayer = null)
        {
            Player player;
            if (createPlayer is not null)
            {
                player = createPlayer(this.GameServerContext);
                await PlayerTestHelper.EnterWorldAsync(player, PlayerTestHelper.CreateCharacter(this.GameServerContext)).ConfigureAwait(false);
            }
            else if (useRespawnPlugIn)
            {
                player = await PlayerTestHelper.CreatePlayerAsync(this.GameServerContext).ConfigureAwait(false);
            }
            else
            {
                player = new MapChangePlayer(this.GameServerContext);
                await PlayerTestHelper.EnterWorldAsync(player, PlayerTestHelper.CreateCharacter(this.GameServerContext)).ConfigureAwait(false);
            }
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
    private sealed class MapChangePlayer(IGameContext context) : Player(context)
    {
        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer() => new MapChangeViews();
    }

    private sealed class MapChangeViews : ICustomPlugInContainer<IViewPlugIn>
    {
        private readonly MockViewPlugInContainer _mocks = new();

        public T? GetPlugIn<T>()
            where T : class, IViewPlugIn
            => typeof(T) == typeof(IRespawnAfterDeathPlugIn) ? null : this._mocks.GetPlugIn<T>();
    }

}
