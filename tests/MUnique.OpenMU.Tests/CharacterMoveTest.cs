// <copyright file="CharacterMoveTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.MessageHandler;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests for the <see cref="CharacterWalkHandlerPlugIn"/>.
/// </summary>
[TestFixture]
public class CharacterMoveTest
{
    private static readonly Point StartPoint = new (147, 120);
    private static readonly Point EndPoint = new (151, 122);

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
    /// Creates the player and performs the example walk.
    /// By example: walking from 147, 120 to 151, 122: C1 08 D4 93 78 44 33 44
    /// The packet contains the starting coordinates and the target is determined by the given path.
    /// </summary>
    /// <returns>The player which walked.</returns>
    private async ValueTask<Player> DoTheWalkAsync()
    {
        var packet = new byte[] { 0xC1, 0x08, (byte)PacketType.Walk, 0x93, 0x78, 0x44, 0x33, 0x44 };
        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.SelectedCharacter!.PositionX = StartPoint.X;
        player.SelectedCharacter.PositionY = StartPoint.Y;
        var moveHandler = new CharacterWalkHandlerPlugIn();
        await moveHandler.HandlePacketAsync(player, packet).ConfigureAwait(false);

        return player;
    }
}