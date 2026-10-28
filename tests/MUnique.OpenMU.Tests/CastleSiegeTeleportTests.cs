// <copyright file="CastleSiegeTeleportTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.ComponentModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests that players can't teleport over a closed Castle Siege gate.
/// </summary>
[TestFixture]
public class CastleSiegeTeleportTests
{
    private static readonly Guid CastleSiegePlugInId = new("B7F62FA9-59E6-49E9-B499-0358A14957CF");

    /// <summary>
    /// The area of the closed test gate, six tiles wide and two tiles high.
    /// </summary>
    private static readonly (byte StartX, byte StartY, byte EndX, byte EndY) GateArea = (90, 100, 95, 101);

    /// <summary>
    /// Verifies which lines cross the area of a closed gate.
    /// </summary>
    /// <param name="fromX">The x coordinate of the start.</param>
    /// <param name="fromY">The y coordinate of the start.</param>
    /// <param name="toX">The x coordinate of the end.</param>
    /// <param name="toY">The y coordinate of the end.</param>
    /// <param name="expected">The expected result.</param>
    [TestCase(92, 97, 92, 104, true, TestName = "Straight over the gate")]
    [TestCase(88, 97, 97, 104, true, TestName = "Diagonal over the gate")]
    [TestCase(95, 98, 90, 103, true, TestName = "Diagonal over the gate, other direction")]
    [TestCase(92, 104, 92, 97, true, TestName = "From the inside out")]
    [TestCase(80, 97, 80, 104, false, TestName = "Beside the gate")]
    [TestCase(92, 94, 92, 99, false, TestName = "In front of the gate")]
    [TestCase(92, 97, 92, 97, false, TestName = "Same tile")]
    public async ValueTask LineCrossesClosedGateAsync(int fromX, int fromY, int toX, int toY, bool expected)
    {
        var fixture = await SiegeFixture.CreateAsync().ConfigureAwait(false);
        fixture.Context.NpcController.BlockGateTerrain(fixture.Map, GateArea);

        var result = fixture.Context.NpcController.IsLineBlockedByClosedGate(
            new Point((byte)fromX, (byte)fromY),
            new Point((byte)toX, (byte)toY));

        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Verifies that an opened gate doesn't block the teleport anymore.
    /// </summary>
    [Test]
    public async ValueTask OpenedGateDoesNotBlockAsync()
    {
        var fixture = await SiegeFixture.CreateAsync().ConfigureAwait(false);
        fixture.Context.NpcController.BlockGateTerrain(fixture.Map, GateArea);
        fixture.Context.NpcController.ReleaseGateTerrain(fixture.Map, GateArea);

        Assert.That(fixture.Context.NpcController.IsLineBlockedByClosedGate(new Point(92, 97), new Point(92, 104)), Is.False);
    }

    /// <summary>
    /// Verifies that the plugin prevents a teleport over a closed gate on the siege map only.
    /// </summary>
    [Test]
    public async ValueTask TeleportOverClosedGateIsPreventedOnTheSiegeMapAsync()
    {
        var fixture = await SiegeFixture.CreateAsync().ConfigureAwait(false);
        fixture.Context.NpcController.BlockGateTerrain(fixture.Map, GateArea);
        var player = await fixture.CreatePlayerAsync(1, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        player.Position = new Point(92, 97);
        var plugInPoint = fixture.GameContext.PlugInManager.GetPlugInPoint<IPlayerTeleportRestrictionPlugIn>()!;

        var overTheGate = new CancelEventArgs();
        plugInPoint.CheckTeleport(player, new Point(92, 104), overTheGate);
        var besideTheGate = new CancelEventArgs();
        plugInPoint.CheckTeleport(player, new Point(92, 99), besideTheGate);
        player.SetCurrentMapSilently(fixture.OtherMap);
        var otherMap = new CancelEventArgs();
        plugInPoint.CheckTeleport(player, new Point(92, 104), otherMap);

        Assert.Multiple(() =>
        {
            Assert.That(overTheGate.Cancel, Is.True);
            Assert.That(besideTheGate.Cancel, Is.False);
            Assert.That(otherMap.Cancel, Is.False, "Not on the siege map.");
        });
    }

    private sealed class SiegeFixture
    {
        private SiegeFixture(IGameContext gameContext, CastleSiegeContext context, GameMap map, GameMap otherMap)
        {
            this.GameContext = gameContext;
            this.Context = context;
            this.Map = map;
            this.OtherMap = otherMap;
        }

        public IGameContext GameContext { get; }

        public CastleSiegeContext Context { get; }

        public GameMap Map { get; }

        public GameMap OtherMap { get; }

        public static async ValueTask<SiegeFixture> CreateAsync()
        {
            var gameContext = GameContextTestHelper.CreateGameContext(
            [
                new PlugInConfiguration { TypeId = CastleSiegePlugInId, IsActive = true },
            ]);
            var siegeMap = new BasicModel.GameMapDefinition { Number = 30, TerrainData = new byte[ushort.MaxValue + 3] };
            gameContext.Configuration.Maps.Add(siegeMap);
            var configuration = new BasicModel.CastleSiegeConfiguration
            {
                Enabled = true,
                CastleSiegeMapDefinition = siegeMap,
            };
            configuration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry { State = CastleSiegeState.Ready, DayOfWeek = DayOfWeek.Monday });
            configuration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry { State = CastleSiegeState.Start, DayOfWeek = DayOfWeek.Tuesday });
            gameContext.Configuration.CastleSiegeConfiguration = configuration;

            var plugIn = gameContext.PlugInManager.GetActivePlugInsOf<IPeriodicTaskPlugIn>().OfType<CastleSiegePlugIn>().Single();
            await plugIn.ExecuteTaskAsync(gameContext).ConfigureAwait(false);
            var context = plugIn.GetContext(gameContext) ?? throw new InvalidOperationException("The context wasn't created.");
            context.CurrentState = CastleSiegeState.Start;

            var map = new GameMap(siegeMap, TimeSpan.FromMinutes(1), 8);
            var otherMap = new GameMap(gameContext.Configuration.Maps.First(), TimeSpan.FromMinutes(1), 8);
            return new SiegeFixture(gameContext, context, map, otherMap);
        }

        public async ValueTask<Player> CreatePlayerAsync(uint guildId, CastleSiegeJoinSide side)
        {
            var player = await PlayerTestHelper.CreatePlayerAsync(this.GameContext).ConfigureAwait(false);
            player.SelectedCharacter!.Id = Guid.NewGuid();
            player.GuildStatus = new GuildMemberStatus(guildId, GuildPosition.NormalMember);
            player.Position = new Point((byte)guildId, 20);
            player.SetCurrentMapSilently(this.Map);
            this.Context.PlayerJoinSides[player.SelectedCharacter.Id] = side;
            return player;
        }
    }
}
