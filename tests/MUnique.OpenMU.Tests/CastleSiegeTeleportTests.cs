// <copyright file="CastleSiegeTeleportTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.ComponentModel;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views.World;
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
    private const short TeleportSkillId = 6;
    private const short TeleportAllySkillId = 15;
    private const int ManaCost = 5;

    private static readonly Guid CastleSiegePlugInId = new("B7F62FA9-59E6-49E9-B499-0358A14957CF");

    /// <summary>
    /// The area of the closed test gate, six tiles wide and two tiles high.
    /// </summary>
    private static readonly (byte StartX, byte StartY, byte EndX, byte EndY) GateArea = (90, 100, 95, 101);

    /// <summary>
    /// Verifies which lines cross the area of a closed gate, in both directions of the line.
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
    [TestCase(85, 99, 91, 102, true, TestName = "Shallow line over the gate corner")]
    [TestCase(95, 99, 96, 100, true, TestName = "Through the corner of a gate tile")]
    [TestCase(89, 102, 90, 99, true, TestName = "Steep line over the gate edge")]
    [TestCase(96, 98, 97, 99, false, TestName = "Through a corner beside the gate")]
    [TestCase(84, 98, 96, 99, false, TestName = "Long line in front of the gate")]
    public async ValueTask LineCrossesClosedGateAsync(int fromX, int fromY, int toX, int toY, bool expected)
    {
        var fixture = await SiegeFixture.CreateAsync().ConfigureAwait(false);
        fixture.Context.NpcController.BlockGateTerrain(fixture.Map, GateArea);

        var from = new Point((byte)fromX, (byte)fromY);
        var to = new Point((byte)toX, (byte)toY);

        var result = fixture.Context.NpcController.IsLineBlockedByClosedGate(from, to);
        var reverseResult = fixture.Context.NpcController.IsLineBlockedByClosedGate(to, from);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(reverseResult, Is.EqualTo(expected), "Reverse direction");
        });
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

    /// <summary>
    /// Verifies that the teleport skill doesn't consume mana and reports the failed teleport when the
    /// line crosses a closed gate, and still works beside the gate.
    /// </summary>
    /// <param name="targetY">The y coordinate of the target.</param>
    /// <param name="isPrevented">Whether the teleport is expected to be prevented.</param>
    [TestCase(104, true, TestName = "Teleport over the gate is prevented")]
    [TestCase(99, false, TestName = "Teleport in front of the gate is allowed")]
    public async ValueTask TeleportSkillOverClosedGateAsync(int targetY, bool isPrevented)
    {
        var fixture = await SiegeFixture.CreateAsync().ConfigureAwait(false);
        fixture.Context.NpcController.BlockGateTerrain(fixture.Map, GateArea);
        var player = await fixture.CreatePlayerAsync(1, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        player.Position = new Point(92, 97);
        await AddTeleportSkillAsync(player, TeleportSkillId).ConfigureAwait(false);
        var manaBefore = player.Attributes![Stats.CurrentMana];

        await new WizardTeleportAction().TryTeleportWithSkillAsync(player, new Point(92, (byte)targetY)).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(player.Attributes[Stats.CurrentMana], isPrevented ? Is.EqualTo(manaBefore) : Is.EqualTo(manaBefore - ManaCost));
            Mock.Get(player.ViewPlugIns.GetPlugIn<ITeleportPlugIn>()!)
                .Verify(p => p.ShowTeleportedAsync(), isPrevented ? Times.Once() : Times.Never());
        });
    }

    /// <summary>
    /// Verifies that the 'Teleport Ally' skill doesn't consume mana when the line of the ally crosses a
    /// closed gate, and still works beside the gate.
    /// </summary>
    /// <param name="targetY">The y coordinate of the target.</param>
    /// <param name="isPrevented">Whether the teleport is expected to be prevented.</param>
    [TestCase(104, true, TestName = "Teleport Ally over the gate is prevented")]
    [TestCase(99, false, TestName = "Teleport Ally in front of the gate is allowed")]
    public async ValueTask TeleportAllySkillOverClosedGateAsync(int targetY, bool isPrevented)
    {
        var fixture = await SiegeFixture.CreateAsync().ConfigureAwait(false);
        fixture.Context.NpcController.BlockGateTerrain(fixture.Map, GateArea);
        var player = await fixture.CreatePlayerAsync(1, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        var ally = await fixture.CreatePlayerAsync(2, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        player.Position = new Point(92, 96);
        ally.Position = new Point(92, 97);
        var party = new Party(new PartyManager(5, new NullLogger<Party>()), 5, new NullLogger<Party>());
        await party.AddAsync(player).ConfigureAwait(false);
        await party.AddAsync(ally).ConfigureAwait(false);
        await player.AddObserverAsync(ally).ConfigureAwait(false);
        await AddTeleportSkillAsync(player, TeleportAllySkillId).ConfigureAwait(false);
        var manaBefore = player.Attributes![Stats.CurrentMana];

        await new WizardTeleportAction().TryTeleportTargetWithSkillAsync(player, ally.Id, new Point(92, (byte)targetY)).ConfigureAwait(false);

        Assert.That(player.Attributes[Stats.CurrentMana], isPrevented ? Is.EqualTo(manaBefore) : Is.EqualTo(manaBefore - ManaCost));
    }

    private static async ValueTask AddTeleportSkillAsync(Player player, short skillNumber)
    {
        var skill = new BasicModel.Skill { Number = skillNumber, Range = 10 };
        skill.ConsumeRequirements.Add(new BasicModel.AttributeRequirement { Attribute = Stats.CurrentMana, MinimumValue = ManaCost });
        await player.SkillList!.AddLearnedSkillAsync(skill).ConfigureAwait(false);
        player.Attributes![Stats.CurrentMana] = ManaCost * 2;
    }

    private sealed class SiegeFixture
    {
        public SiegeFixture(IGameContext gameContext, CastleSiegeContext context, GameMap map, GameMap otherMap)
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
