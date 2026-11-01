// <copyright file="WarpGateActionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests the position check of the <see cref="WarpGateAction"/>.
/// </summary>
[TestFixture]
public class WarpGateActionTest
{
    private const byte GateX1 = 100;
    private const byte GateX2 = 102;
    private const byte GateY = 101;

    /// <summary>
    /// The info range is set much higher than the gate entry tolerance, to show that it doesn't matter for the check.
    /// </summary>
    private const byte InfoRange = 30;

    /// <summary>
    /// Tests that a player standing inside of the gate area can enter the gate.
    /// </summary>
    [Test]
    public async Task PlayerInsideOfTheGateCanEnterAsync()
    {
        var (player, gate) = await CreatePlayerAndGateAsync().ConfigureAwait(false);
        player.Position = new Point(GateX1 + 1, GateY);

        await new WarpGateAction().EnterGateAsync(player, gate).ConfigureAwait(false);

        AssertWarped(player);
    }

    /// <summary>
    /// Tests that a player standing exactly at the tolerance next to the gate area can enter the gate.
    /// </summary>
    [Test]
    public async Task PlayerAtTheToleranceCanEnterAsync()
    {
        var (player, gate) = await CreatePlayerAndGateAsync().ConfigureAwait(false);
        player.Position = new Point(GateX2 + WarpGateAction.GateEntryTolerance, GateY);

        await new WarpGateAction().EnterGateAsync(player, gate).ConfigureAwait(false);

        AssertWarped(player);
    }

    /// <summary>
    /// Tests that a player standing one tile further away than the tolerance can't enter the gate,
    /// although it's still within the info range.
    /// </summary>
    [Test]
    public async Task PlayerJustOutsideOfTheToleranceCantEnterAsync()
    {
        var (player, gate) = await CreatePlayerAndGateAsync().ConfigureAwait(false);
        player.Position = new Point(GateX2 + WarpGateAction.GateEntryTolerance + 1, GateY);

        await new WarpGateAction().EnterGateAsync(player, gate).ConfigureAwait(false);

        AssertNotWarped(player);
    }

    /// <summary>
    /// Tests that a walking player whose walk ends within the tolerance can enter the gate,
    /// although its current position is still outside.
    /// </summary>
    [Test]
    public async Task WalkingPlayerWithTargetInsideCanEnterAsync()
    {
        var (player, gate) = await CreatePlayerAndGateAsync().ConfigureAwait(false);
        var start = new Point(GateX2 + WarpGateAction.GateEntryTolerance + 3, GateY);
        await StartSlowWalkAsync(player, start, Direction.NorthWest).ConfigureAwait(false);
        Assume.That(player.Position.X, Is.GreaterThan(GateX2 + WarpGateAction.GateEntryTolerance), "The player should still be outside.");

        await new WarpGateAction().EnterGateAsync(player, gate).ConfigureAwait(false);

        AssertWarped(player);
    }

    /// <summary>
    /// Tests that a walking player whose walk ends outside of the tolerance can't enter the gate,
    /// although its current position is still inside.
    /// </summary>
    [Test]
    public async Task WalkingPlayerWithTargetOutsideCantEnterAsync()
    {
        var (player, gate) = await CreatePlayerAndGateAsync().ConfigureAwait(false);
        var start = new Point(GateX2 + WarpGateAction.GateEntryTolerance - 2, GateY);
        await StartSlowWalkAsync(player, start, Direction.SouthEast).ConfigureAwait(false);
        Assume.That(player.Position.X, Is.LessThanOrEqualTo(GateX2 + WarpGateAction.GateEntryTolerance), "The player should still be inside.");

        await new WarpGateAction().EnterGateAsync(player, gate).ConfigureAwait(false);

        AssertNotWarped(player);
    }

    private static async ValueTask<(Player Player, EnterGate Gate)> CreatePlayerAndGateAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.Configuration.InfoRange = InfoRange;
        var map = player.CurrentMap!.Definition;
        map.IsActive = true;
        var gate = new EnterGate
        {
            X1 = GateX1,
            X2 = GateX2,
            Y1 = GateY - 1,
            Y2 = GateY + 1,
            TargetGate = new ExitGate { Map = map, X1 = 10, X2 = 10, Y1 = 10, Y2 = 10 },
        };
        Mock.Get(map).Setup(m => m.EnterGates).Returns(new List<EnterGate> { gate });
        return (player, gate);
    }

    /// <summary>
    /// Starts a slow walk of three steps into the given direction. The first step is taken right away,
    /// the remaining ones take very long, so the player keeps walking during the test.
    /// </summary>
    private static async ValueTask StartSlowWalkAsync(Player player, Point start, Direction direction)
    {
        player.Attributes!.AddElement(new SimpleElement(0.0001f, AggregateType.AddRaw), Stats.MovementSpeedFactor);
        player.Position = start;
        var steps = new WalkingStep[3];
        var from = start;
        for (var i = 0; i < steps.Length; i++)
        {
            var to = from.CalculateTargetPoint(direction);
            steps[i] = new WalkingStep { From = from, To = to, Direction = direction };
            from = to;
        }

        await player.WalkToAsync(from, steps).ConfigureAwait(false);

        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (player.Position == start && DateTime.UtcNow < deadline)
        {
            await Task.Delay(10).ConfigureAwait(false);
        }

        Assume.That(player.IsWalking, Is.True, "The player should still be walking.");
        Assume.That(player.WalkTarget, Is.EqualTo(from));
    }

    private static void AssertWarped(Player player)
    {
        var mapChangePlugIn = Mock.Get(player.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!);
        mapChangePlugIn.Verify(p => p.MapChangeAsync(), Times.Once);
        mapChangePlugIn.Verify(p => p.MapChangeFailedAsync(), Times.Never);
    }

    private static void AssertNotWarped(Player player)
    {
        var mapChangePlugIn = Mock.Get(player.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!);
        mapChangePlugIn.Verify(p => p.MapChangeAsync(), Times.Never);
        mapChangePlugIn.Verify(p => p.MapChangeFailedAsync(), Times.Once);
    }
}
