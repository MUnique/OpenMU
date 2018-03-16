// <copyright file="CharacterMoveTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.GameServer.MessageHandler;
    using MUnique.OpenMU.Pathfinding;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="CharacterMoveHandler"/>.
    /// </summary>
    [TestFixture]
    public class CharacterMoveTest
    {
        /// <summary>
        /// Tests if handling a walk packet results in the correct target coordinates.
        /// By example: walking from 147, 120 to 151, 122: C1 08 D4 93 78 44 33 44
        /// The packet contains the starting coordinates and the target is determined by the given path
        /// </summary>
        [Test]
        public void TestWalkTargetIsCorrect()
        {
            var packet = new byte[] { 0xC1, 0x08, (byte)PacketType.Walk, 147, 120, 0x44, 0x33, 0x44 };
            var player = TestHelper.GetPlayer();
            var moveHandler = new CharacterMoveHandler();
            moveHandler.HandlePacket(player, packet);
            Assert.That(player.WalkTarget, Is.EqualTo(new Point(151, 122)));
            Assert.That(player.NextDirections.Count, Is.EqualTo(4));
        }
    }
}
