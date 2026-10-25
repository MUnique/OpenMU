// <copyright file="CharacterMoveTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.MessageHandler;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests for the <see cref="CharacterWalkHandlerPlugIn"/>.
/// </summary>
[TestFixture]
public class CharacterMoveTest
{
    private static readonly Point StartPoint = new(147, 120);
    private static readonly Point EndPoint = new(151, 122);

    /// <summary>
    /// Tests if handling a walk packet results in the correct target coordinates.
    /// </summary>
    [Test]
    public async ValueTask TestWalkTargetIsCorrectAsync()
    {
        var player = await this.DoTheWalkAsync().ConfigureAwait(false);
        Assert.That(player.WalkTarget, Is.EqualTo(EndPoint));
    }

    /// <summary>
    /// Tests if handling a walk packet results in the correct walk directions.
    /// </summary>
    [Test]
    public async ValueTask TestWalkStepsAreCorrectAsync()
    {
        var player = await this.DoTheWalkAsync().ConfigureAwait(false);

        // the next check is questionable - there is a timer which is removing a direction every 500ms. If the test runs "too slow", the count is 3 ;-)
        Memory<WalkingStep> steps = new WalkingStep[16];
        var count = await player.GetStepsAsync(steps).ConfigureAwait(false);
        Assert.That(count, Is.EqualTo(4));

        steps = steps.Slice(0, count);
        steps.Span.Reverse();
        Assert.That(steps.Span[0].From, Is.EqualTo(StartPoint));
        Assert.That(steps.Span[steps.Length - 1].To, Is.EqualTo(EndPoint));
        for (var index = 0; index < steps.Span.Length; index++)
        {
            var direction = steps.Span[index];
            Assert.That(direction.From, Is.Not.EqualTo(direction.To));
        }
    }

    /// <summary>
    /// Tests if a walk packet without steps - the way the client reports that it stopped a walk by
    /// itself - ends the walk at the position the client reports.
    /// </summary>
    [Test]
    public async ValueTask TestWalkWithoutStepsEndsTheWalkAsync()
    {
        var player = await this.DoTheWalkAsync().ConfigureAwait(false);
        var stopPoint = (await GetWalkPointsAsync(player).ConfigureAwait(false))[1];
        player.Position = StartPoint;

        await HandleStopPacketAsync(player, stopPoint).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(player.IsWalking, Is.False);
            Assert.That(player.Position, Is.EqualTo(stopPoint));
        });
    }

    /// <summary>
    /// Tests if the end of a walk is reported to the other players in view, but not to the player which
    /// stopped. Its client already placed the character and started the attack animation; telling it
    /// again would pull the character onto the tile centre and drop that animation.
    /// </summary>
    [Test]
    public async ValueTask TestEndOfWalkIsReportedToObserversOnlyAsync()
    {
        var player = await this.DoTheWalkAsync().ConfigureAwait(false);
        var observer = await PlayerTestHelper.CreatePlayerAsync(player.GameContext).ConfigureAwait(false);
        await player.AddObserverAsync(observer).ConfigureAwait(false);
        var stopPoint = (await GetWalkPointsAsync(player).ConfigureAwait(false))[1];
        player.Position = StartPoint;

        await HandleStopPacketAsync(player, stopPoint).ConfigureAwait(false);

        Mock.Get(observer.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>()!)
            .Verify(p => p.ObjectMovedAsync(player, MoveType.Instant), Times.Once);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>()!)
            .Verify(p => p.ObjectMovedAsync(player, MoveType.Instant), Times.Never);
    }

    /// <summary>
    /// Tests if a reported stop position which the current walk doesn't pass through is ignored, so the
    /// request can't be used to leave the path.
    /// </summary>
    [Test]
    public async ValueTask TestStopPositionOutsideOfTheWalkIsIgnoredAsync()
    {
        var player = await this.DoTheWalkAsync().ConfigureAwait(false);
        var walkPoints = await GetWalkPointsAsync(player).ConfigureAwait(false);
        var offPathPoint = new Point(StartPoint.X, (byte)(StartPoint.Y - 2));
        Assert.That(walkPoints, Does.Not.Contain(offPathPoint), "The test needs a position the walk doesn't pass through.");
        player.Position = StartPoint;

        await HandleStopPacketAsync(player, offPathPoint).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(player.IsWalking, Is.True);
            Assert.That(player.Position, Is.Not.EqualTo(offPathPoint));
        });
    }

    /// <summary>
    /// Tests if a reported stop position further away than a walk may start away from us is ignored, so
    /// the request can't be used to travel the rest of the path at once.
    /// </summary>
    [Test]
    public async ValueTask TestStopPositionTooFarAwayIsIgnoredAsync()
    {
        // The walker takes its first step right away and runs in the background. If it took another step
        // after we moved the player away from the path, the player would be back in reach of the end point.
        // So the steps are slowed down, and we wait until the first one is done before moving the player away.
        var player = await this.DoTheWalkAsync(movementSpeedFactor: 0.0001f).ConfigureAwait(false);
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (player.Position == StartPoint && DateTime.UtcNow < deadline)
        {
            await Task.Delay(10).ConfigureAwait(false);
        }

        Assert.That(player.Position, Is.Not.EqualTo(StartPoint), "The walker didn't take its first step.");
        player.Position = new Point((byte)(StartPoint.X + 10), StartPoint.Y);

        await HandleStopPacketAsync(player, EndPoint).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(player.IsWalking, Is.True);
            Assert.That(player.Position, Is.Not.EqualTo(EndPoint));
        });
    }

    /// <summary>
    /// Tests if a walk packet without steps still applies the rotation it carries, which is what it was
    /// used for before it also ended the walk.
    /// </summary>
    [Test]
    public async ValueTask TestWalkWithoutStepsAppliesTheRotationAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Position = StartPoint;

        await HandleStopPacketAsync(player, StartPoint, rotation: 3).ConfigureAwait(false);

        Assert.That(player.Rotation, Is.EqualTo(Direction.SouthEast));
    }

    /// <summary>
    /// Gets every position the player's current walk passes through, from its start to its target.
    /// </summary>
    /// <param name="player">The walking player.</param>
    /// <returns>The positions of the walk.</returns>
    private static async ValueTask<IList<Point>> GetWalkPointsAsync(Player player)
    {
        Memory<WalkingStep> steps = new WalkingStep[16];
        var count = await player.GetStepsAsync(steps).ConfigureAwait(false);
        steps = steps[..count];
        steps.Span.Reverse();

        var points = new List<Point> { steps.Span[0].From };
        foreach (var step in steps.Span)
        {
            points.Add(step.To);
        }

        return points;
    }

    /// <summary>
    /// Handles a walk packet without any steps, the way the client reports that it stopped a walk.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="stopPoint">The position the client reports it stopped at.</param>
    /// <param name="rotation">The rotation the client reports.</param>
    private static ValueTask HandleStopPacketAsync(Player player, Point stopPoint, byte rotation = 4)
    {
        var packet = new byte[] { 0xC1, 0x06, (byte)PacketType.Walk, stopPoint.X, stopPoint.Y, (byte)(rotation << 4) };

        return new CharacterWalkHandlerPlugIn().HandlePacketAsync(player, packet);
    }

    /// <summary>
    /// Creates the player and performs the example walk.
    /// By example: walking from 147, 120 to 151, 122: C1 08 D4 93 78 44 33 44
    /// The packet contains the starting coordinates and the target is determined by the given path.
    /// </summary>
    /// <param name="movementSpeedFactor">The movement speed factor of the player, if it should differ from the default.</param>
    /// <returns>The player which walked.</returns>
    private async ValueTask<Player> DoTheWalkAsync(float? movementSpeedFactor = null)
    {
        var packet = new byte[] { 0xC1, 0x08, (byte)PacketType.Walk, 0x93, 0x78, 0x44, 0x33, 0x44 };
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        if (movementSpeedFactor is { } factor)
        {
            player.Attributes!.AddElement(new SimpleElement(factor, AggregateType.AddRaw), Stats.MovementSpeedFactor);
        }

        player.SelectedCharacter!.PositionX = StartPoint.X;
        player.SelectedCharacter.PositionY = StartPoint.Y;
        var moveHandler = new CharacterWalkHandlerPlugIn();
        await moveHandler.HandlePacketAsync(player, packet).ConfigureAwait(false);

        return player;
    }
}