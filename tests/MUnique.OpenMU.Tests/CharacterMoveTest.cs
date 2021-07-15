// <copyright file="CharacterMoveTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.GameServer.MessageHandler;
    using MUnique.OpenMU.Pathfinding;
    using NUnit.Framework;

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
        public void TestWalkTargetIsCorrect()
        {
            var player = this.DoTheWalk();
            Assert.That(player.WalkTarget, Is.EqualTo(EndPoint));
        }

        /// <summary>
        /// Tests if handling a walk packet results in the correct walk directions.
        /// </summary>
        [Test]
        public void TestWalkStepsAreCorrect()
        {
            var player = this.DoTheWalk();

            // the next check is questionable - there is a timer which is removing a direction every 500ms. If the test runs "too slow", the count is 3 ;-)
            Span<WalkingStep> steps = new WalkingStep[16];
            var count = player.GetSteps(steps);
            Assert.That(count, Is.EqualTo(4));

            steps = steps.Slice(0, count);
            steps.Reverse();
            Assert.That(steps[0].From, Is.EqualTo(StartPoint));
            Assert.That(steps[steps.Length - 1].To, Is.EqualTo(EndPoint));
            foreach (var direction in steps)
            {
                Assert.That(direction.From, Is.Not.EqualTo(direction.To));
            }
        }

        /// <summary>
        /// Creates the player and performs the example walk.
        /// By example: walking from 147, 120 to 151, 122: C1 08 D4 93 78 44 33 44
        /// The packet contains the starting coordinates and the target is determined by the given path.
        /// </summary>
        /// <returns>The player which walked.</returns>
        private Player DoTheWalk()
        {
            var packet = new byte[] { 0xC1, 0x08, (byte)PacketType.Walk, 147, 120, 0x44, 0x33, 0x44 };
            var player = TestHelper.CreatePlayer();
            var moveHandler = new CharacterWalkHandlerPlugIn();
            moveHandler.HandlePacket(player, packet);

            return player;
        }
    }
}
