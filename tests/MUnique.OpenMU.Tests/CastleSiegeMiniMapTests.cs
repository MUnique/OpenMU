// <copyright file="CastleSiegeMiniMapTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Pathfinding;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests Castle Siege mini-map access, snapshot consistency, and command validation and throttling.
/// </summary>
[TestFixture]
public class CastleSiegeMiniMapTests
{
    /// <summary>
    /// Verifies the command handler's protocol subcode.
    /// </summary>
    [Test]
    public void CommandHandlerUsesExpectedSubcode()
    {
        Assert.That(new CastleSiegeCommandHandlerPlugIn().Key, Is.EqualTo(0x1D));
    }

    /// <summary>
    /// Verifies that an incomplete packet never enters the command action.
    /// </summary>
    /// <param name="empty">Whether the packet is empty instead of one byte short.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async ValueTask CommandHandlerIgnoresTruncatedPacketsAsync(bool empty)
    {
        var fixture = new SiegeFixture();
        var player = await fixture.CreatePlayerAsync().ConfigureAwait(false);

        var packet = new byte[empty ? 0 : CastleGuildCommand.Length - 1];
        await new CastleSiegeCommandHandlerPlugIn().HandlePacketAsync(player, packet).ConfigureAwait(false);

        CommandView(player).VerifyNoOtherCalls();
    }

    /// <summary>
    /// Verifies that malformed command values cannot reach clients or context resolution.
    /// </summary>
    /// <param name="team">The command group index.</param>
    /// <param name="command">The raw command value.</param>
    [TestCase(7, 0)]
    [TestCase(255, 0)]
    [TestCase(0, 9)]
    [TestCase(0, 255)]
    public async ValueTask CommandHandlerRejectsInvalidValuesAsync(byte team, byte command)
    {
        var fixture = new SiegeFixture();
        var gameContext = new Mock<IGameServerContext>();
        gameContext.SetupGet(context => context.Configuration).Returns(fixture.Context.GameContext.Configuration);
        gameContext.SetupGet(context => context.LoggerFactory).Returns(fixture.Context.GameContext.LoggerFactory);
        gameContext.SetupGet(context => context.PersistenceContextProvider).Returns(fixture.Context.GameContext.PersistenceContextProvider);
        gameContext.SetupGet(context => context.PlugInManager).Returns(fixture.Context.GameContext.PlugInManager);
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer(gameContext.Object);
        var packet = new CastleGuildCommand(new byte[CastleGuildCommand.Length])
        {
            Team = team,
            Command = (CastleSiegeGuildCommandType)command,
        };

        // With no registered CastleSiegePlugIn, a regression which relays invalid input would otherwise
        // still produce no view call. Verify that the handler rejects it before looking up that context.
        gameContext.Invocations.Clear();
        await new CastleSiegeCommandHandlerPlugIn().HandlePacketAsync(player, packet).ConfigureAwait(false);

        gameContext.VerifyGet(context => context.PlugInManager, Times.Never);
        Assert.That(output.Length, Is.Zero);
    }

    /// <summary>
    /// Verifies the first and last valid groups and all three commands, including same-side allied guilds.
    /// </summary>
    /// <param name="team">The command group index.</param>
    /// <param name="command">The command to issue.</param>
    [TestCase(0, CastleSiegeCommandType.Attack)]
    [TestCase(6, CastleSiegeCommandType.Defend)]
    [TestCase(5, CastleSiegeCommandType.Wait)]
    public async ValueTask CommandsReachOnlySameSidePlayersAsync(byte team, CastleSiegeCommandType command)
    {
        var fixture = new SiegeFixture();
        var issuer = await fixture.CreatePlayerAsync().ConfigureAwait(false);
        var ally = await fixture.CreatePlayerAsync(2).ConfigureAwait(false);
        var enemy = await fixture.CreatePlayerAsync(3, CastleSiegeJoinSide.Defense).ConfigureAwait(false);
        var spectator = await fixture.CreatePlayerAsync(4, CastleSiegeJoinSide.None).ConfigureAwait(false);
        var offMap = await fixture.CreatePlayerAsync().ConfigureAwait(false);
        offMap.SetCurrentMapSilently(null);

        await CastleSiegeGuildCommandAction.IssueCommandAsync(issuer, fixture.Context, team, 255, 0, command).ConfigureAwait(false);

        CommandView(issuer).Verify(view => view.ShowGuildCommandAsync(team, 255, 0, command), Times.Once);
        CommandView(ally).Verify(view => view.ShowGuildCommandAsync(team, 255, 0, command), Times.Once);
        CommandView(enemy).VerifyNoOtherCalls();
        CommandView(spectator).VerifyNoOtherCalls();
        CommandView(offMap).VerifyNoOtherCalls();
    }

    /// <summary>
    /// Verifies that the issuer and recipients use the same tracked side during a Crown-capture resynchronization.
    /// </summary>
    [Test]
    public async ValueTask CommandsUseTrackedSideAfterCaptureAsync()
    {
        var fixture = new SiegeFixture();
        var issuer = await fixture.CreatePlayerAsync().ConfigureAwait(false);
        var ally = await fixture.CreatePlayerAsync(2, isMaster: false).ConfigureAwait(false);
        var enemy = await fixture.CreatePlayerAsync(3, CastleSiegeJoinSide.Defense, false).ConfigureAwait(false);
        fixture.Context.PlayerJoinSides[issuer.SelectedCharacter!.Id] = CastleSiegeJoinSide.Attack1;
        fixture.Context.FinalGuildList[1].Side = CastleSiegeJoinSide.Defense;

        await IssueAsync(fixture, issuer).ConfigureAwait(false);

        CommandView(issuer).Verify(view => view.ShowGuildCommandAsync(0, 10, 20, CastleSiegeCommandType.Attack), Times.Once);
        CommandView(ally).Verify(view => view.ShowGuildCommandAsync(0, 10, 20, CastleSiegeCommandType.Attack), Times.Once);
        CommandView(enemy).VerifyNoOtherCalls();
    }

    /// <summary>
    /// Verifies that membership, alliance leadership, and battle state are required.
    /// </summary>
    /// <param name="scenario">The authorization condition to invalidate.</param>
    [TestCase("disabled")]
    [TestCase("state")]
    [TestCase("member")]
    [TestCase("allied-master")]
    [TestCase("unselected")]
    [TestCase("no-side")]
    [TestCase("off-map")]
    public async ValueTask CommandsRejectUnauthorizedIssuersAsync(string scenario)
    {
        var fixture = new SiegeFixture();
        var issuer = await fixture.CreatePlayerAsync().ConfigureAwait(false);
        switch (scenario)
        {
            case "disabled":
                fixture.Context.Configuration.Enabled = false;
                break;
            case "state":
                fixture.Context.CurrentState = CastleSiegeState.End;
                break;
            case "member":
                issuer.GuildStatus = new GuildMemberStatus(1, GuildPosition.NormalMember);
                break;
            case "allied-master":
                fixture.Context.FinalGuildList[1].IsAllianceMaster = false;
                break;
            case "unselected":
                fixture.Context.FinalGuildList.Clear();
                break;
            case "no-side":
                fixture.Context.FinalGuildList[1].Side = CastleSiegeJoinSide.None;
                break;
            case "off-map":
                issuer.SetCurrentMapSilently(null);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scenario), scenario, "Unknown authorization scenario.");
        }

        await IssueAsync(fixture, issuer).ConfigureAwait(false);

        CommandView(issuer).VerifyNoOtherCalls();
    }

    /// <summary>
    /// Verifies that no command escapes when the battle ends while waiting for the siege lock.
    /// </summary>
    [Test]
    public async ValueTask CommandsRecheckStateAfterWaitingAsync()
    {
        var fixture = new SiegeFixture();
        var issuer = await fixture.CreatePlayerAsync().ConfigureAwait(false);
        await fixture.Context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        var pending = IssueAsync(fixture, issuer).AsTask();
        try
        {
            Assert.That(pending.IsCompleted, Is.False);
            fixture.Context.CurrentState = CastleSiegeState.End;
        }
        finally
        {
            fixture.Context.ExecutionLock.Release();
        }

        await pending.ConfigureAwait(false);
        CommandView(issuer).VerifyNoOtherCalls();
    }

    /// <summary>
    /// Verifies the cooldown with the injected clock, without real delays.
    /// </summary>
    [Test]
    public async ValueTask CommandsRespectInjectedCooldownAsync()
    {
        var fixture = new SiegeFixture();
        var issuer = await fixture.CreatePlayerAsync().ConfigureAwait(false);
        await IssueAsync(fixture, issuer).ConfigureAwait(false);
        fixture.Clock.Advance(TimeSpan.FromMilliseconds(999));
        await IssueAsync(fixture, issuer).ConfigureAwait(false);
        CommandView(issuer).Verify(view => view.ShowGuildCommandAsync(0, 10, 20, CastleSiegeCommandType.Attack), Times.Once);

        fixture.Clock.Advance(TimeSpan.FromMilliseconds(1));
        await IssueAsync(fixture, issuer).ConfigureAwait(false);
        CommandView(issuer).Verify(view => view.ShowGuildCommandAsync(0, 10, 20, CastleSiegeCommandType.Attack), Times.Exactly(2));
    }

    /// <summary>
    /// Verifies that repeated attempts from unauthorized players are dropped before waiting for the global lock.
    /// </summary>
    [Test]
    public async ValueTask UnauthorizedCommandsAreThrottledBeforeLockAsync()
    {
        var fixture = new SiegeFixture();
        var issuer = await fixture.CreatePlayerAsync(isMaster: false).ConfigureAwait(false);
        await IssueAsync(fixture, issuer).ConfigureAwait(false);
        await fixture.Context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        Task pending;
        try
        {
            pending = IssueAsync(fixture, issuer).AsTask();
            Assert.That(pending.IsCompletedSuccessfully, Is.True, "Repeated unauthorized attempts must not wait for ExecutionLock.");
        }
        finally
        {
            fixture.Context.ExecutionLock.Release();
        }

        await pending.ConfigureAwait(false);
        CommandView(issuer).VerifyNoOtherCalls();
    }

    /// <summary>
    /// Verifies that a burst reserves only one pending command while the siege tick holds the lock.
    /// </summary>
    [Test]
    public async ValueTask CommandBurstQueuesOnlyOneAttemptAsync()
    {
        var fixture = new SiegeFixture();
        var issuer = await fixture.CreatePlayerAsync().ConfigureAwait(false);
        await fixture.Context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        var pending = Enumerable.Range(0, 20).Select(_ => IssueAsync(fixture, issuer).AsTask()).ToArray();
        try
        {
            Assert.That(pending.Count(task => !task.IsCompleted), Is.EqualTo(1));
        }
        finally
        {
            fixture.Context.ExecutionLock.Release();
        }

        await Task.WhenAll(pending).ConfigureAwait(false);
        CommandView(issuer).Verify(view => view.ShowGuildCommandAsync(0, 10, 20, CastleSiegeCommandType.Attack), Times.Once);
    }

    /// <summary>
    /// Verifies a single side snapshot, player-before-NPC ordering, and recovery when recipients depart during delivery.
    /// </summary>
    [Test]
    public async ValueTask MiniMapUsesStableSnapshotWhenRecipientsLeaveAsync()
    {
        var fixture = new SiegeFixture();
        var first = await fixture.CreatePlayerAsync().ConfigureAwait(false);
        var second = await fixture.CreatePlayerAsync(2, CastleSiegeJoinSide.Defense).ConfigureAwait(false);
        var deliveries = new Dictionary<Player, List<string>>();
        foreach (var player in new[] { first, second })
        {
            deliveries[player] = [];
            var view = MiniMapView(player);
            view.Setup(plugIn => plugIn.ShowPlayerPositionsAsync(It.IsAny<IReadOnlyList<CastleSiegeMiniMapPlayerInfo>>()))
                .Callback(() =>
                {
                    deliveries[player].Add("players");
                    first.SetCurrentMapSilently(null);
                    second.SetCurrentMapSilently(null);
                })
                .Returns(ValueTask.CompletedTask);
            view.Setup(plugIn => plugIn.ShowNpcPositionsAsync(It.IsAny<IReadOnlyList<CastleSiegeMiniMapNpcInfo>>()))
                .Callback(() => deliveries[player].Add("npcs"))
                .Returns(ValueTask.CompletedTask);
        }

        await CastleSiegeMiniMap.BroadcastAsync(fixture.Context).ConfigureAwait(false);

        foreach (var player in new[] { first, second })
        {
            Assert.That(deliveries[player], Is.EqualTo(new[] { "players", "npcs" }));
            MiniMapView(player).Verify(view => view.ShowPlayerPositionsAsync(
                It.Is<IReadOnlyList<CastleSiegeMiniMapPlayerInfo>>(positions => positions.Count == 1)), Times.Once);
            MiniMapView(player).Verify(view => view.ShowNpcPositionsAsync(It.IsAny<IReadOnlyList<CastleSiegeMiniMapNpcInfo>>()), Times.Once);
        }
    }

    /// <summary>
    /// Verifies that only alliance masters receive updates and that their position layer excludes other sides.
    /// </summary>
    [Test]
    public async ValueTask MiniMapFiltersRecipientsPlayersAndStructuresAsync()
    {
        var fixture = new SiegeFixture();
        var commander = await fixture.CreatePlayerAsync().ConfigureAwait(false);
        var ally = await fixture.CreatePlayerAsync(2, isMaster: false).ConfigureAwait(false);
        var enemy = await fixture.CreatePlayerAsync(3, CastleSiegeJoinSide.Defense, false).ConfigureAwait(false);
        var spectator = await fixture.CreatePlayerAsync(4, CastleSiegeJoinSide.None).ConfigureAwait(false);
        fixture.Context.PlayerJoinSides[commander.SelectedCharacter!.Id] = CastleSiegeJoinSide.Attack1;
        fixture.Context.FinalGuildList[1].Side = CastleSiegeJoinSide.Defense;
        fixture.Context.ActiveNpcs.Add(CreateStructure(CastleSiegeGate.MonsterNumber, true));
        fixture.Context.ActiveNpcs.Add(CreateStructure(CastleSiegeStatue.MonsterNumber, true));
        fixture.Context.ActiveNpcs.Add(CreateStructure(CastleSiegeStatue.MonsterNumber, false));

        await CastleSiegeMiniMap.BroadcastAsync(fixture.Context).ConfigureAwait(false);

        MiniMapView(commander).Verify(view => view.ShowPlayerPositionsAsync(
            It.Is<IReadOnlyList<CastleSiegeMiniMapPlayerInfo>>(positions => positions.Count == 2)), Times.Once);
        MiniMapView(commander).Verify(view => view.ShowNpcPositionsAsync(
            It.Is<IReadOnlyList<CastleSiegeMiniMapNpcInfo>>(positions => positions.Count == 2 && positions[0].IsGate && !positions[1].IsGate)), Times.Once);
        MiniMapView(ally).VerifyNoOtherCalls();
        MiniMapView(enemy).VerifyNoOtherCalls();
        MiniMapView(spectator).VerifyNoOtherCalls();
    }

    /// <summary>
    /// Verifies that position updates stop outside the battle state.
    /// </summary>
    [Test]
    public async ValueTask MiniMapStopsOutsideBattleAsync()
    {
        var fixture = new SiegeFixture();
        var commander = await fixture.CreatePlayerAsync().ConfigureAwait(false);
        fixture.Context.CurrentState = CastleSiegeState.End;

        await CastleSiegeMiniMap.BroadcastAsync(fixture.Context).ConfigureAwait(false);

        MiniMapView(commander).VerifyNoOtherCalls();
    }

    private static ValueTask IssueAsync(SiegeFixture fixture, Player player)
        => CastleSiegeGuildCommandAction.IssueCommandAsync(player, fixture.Context, 0, 10, 20, CastleSiegeCommandType.Attack);

    private static Mock<ICastleSiegeCommandPlugIn> CommandView(Player player)
        => Mock.Get(player.ViewPlugIns.GetPlugIn<ICastleSiegeCommandPlugIn>()!);

    private static Mock<ICastleSiegeMiniMapPlugIn> MiniMapView(Player player)
        => Mock.Get(player.ViewPlugIns.GetPlugIn<ICastleSiegeMiniMapPlugIn>()!);

    private static CastleSiegeNpcRuntime CreateStructure(short number, bool isAlive)
        => new()
        {
            Definition = new BasicModel.CastleSiegeNpcDefinition
            {
                MonsterDefinition = new BasicModel.MonsterDefinition { Number = number },
                IsPersistedToDatabase = true,
                SpawnX = 50,
                SpawnY = 60,
            },
            IsAlive = isAlive,
        };

    private sealed class SiegeFixture
    {
        public SiegeFixture()
        {
            var gameContext = GameContextTestHelper.CreateGameContext();
            var configuration = new BasicModel.CastleSiegeConfiguration
            {
                Enabled = true,
                CastleSiegeMapDefinition = gameContext.Configuration.Maps.First(),
            };
            configuration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry { State = CastleSiegeState.Ready, DayOfWeek = DayOfWeek.Monday });
            configuration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry { State = CastleSiegeState.Start, DayOfWeek = DayOfWeek.Tuesday });
            this.Context = new CastleSiegeContext(gameContext, configuration, this.Clock) { CurrentState = CastleSiegeState.Start };
            this.Map = new GameMap(configuration.CastleSiegeMapDefinition, TimeSpan.FromMinutes(1), 8);
        }

        public CastleSiegeContext Context { get; }

        public GameMap Map { get; }

        public ManualTimeProvider Clock { get; } = new();

        public async ValueTask<Player> CreatePlayerAsync(uint guildId = 1, CastleSiegeJoinSide side = CastleSiegeJoinSide.Attack1, bool isMaster = true)
        {
            var player = await PlayerTestHelper.CreatePlayerAsync(this.Context.GameContext).ConfigureAwait(false);
            player.SelectedCharacter!.Id = Guid.NewGuid();
            player.GuildStatus = new GuildMemberStatus(guildId, isMaster ? GuildPosition.GuildMaster : GuildPosition.NormalMember);
            player.Position = new Point((byte)guildId, 20);
            player.SetCurrentMapSilently(this.Map);
            this.Context.FinalGuildList[guildId] = new CastleSiegeGuildParticipant { GuildId = guildId, Side = side, IsAllianceMaster = true };
            this.Context.TrackPlayer(player, this.Map);
            return player;
        }
    }

    private sealed class ManualTimeProvider : TimeProvider
    {
        private DateTimeOffset _utcNow = new(2026, 10, 20, 12, 0, 0, TimeSpan.Zero);

        public override DateTimeOffset GetUtcNow() => this._utcNow;

        public void Advance(TimeSpan elapsed) => this._utcNow += elapsed;
    }
}
