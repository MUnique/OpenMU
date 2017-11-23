// <copyright file="ListPacketQueueTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// Unit tests for the <see cref="ListPacketQueue"/>.
    /// </summary>
    [TestFixture]
    public class ListPacketQueueTest
    {
        /// <summary>
        /// Tests the dequeuing.
        /// </summary>
        [Test]
        public void TestMultipleDequeuing()
        {
            var buffer = new ListPacketQueue();
            var packet = new byte[10];
            packet[0] = 0xC1;
            packet[1] = 10;
            packet[2] = 255;
            buffer.AddRange(packet);
            buffer.AddRange(packet);
            var packet2 = new byte[20];
            packet2[0] = 0xC2;
            packet2[1] = 0;
            packet2[2] = 20;
            packet2[3] = 255;
            buffer.AddRange(packet2);

            var dequeued = buffer.DequeueNextPacket();
            Assert.That(dequeued, Is.Not.Null);
            dequeued = buffer.DequeueNextPacket();
            Assert.That(dequeued, Is.Not.Null);
            dequeued = buffer.DequeueNextPacket();
            Assert.That(dequeued, Is.Not.Null);
            dequeued = buffer.DequeueNextPacket();
            Assert.That(dequeued, Is.Null);
        }

        /// <summary>
        /// Tests if an exception is thrown for an invalid packet.
        /// </summary>
        [Test]
        public void TestInvalidPacket()
        {
            var buffer = new ListPacketQueue();
            var packet = new byte[] { 0, 0, 0, 0 };
            buffer.AddRange(packet);
            Assert.Throws<System.Exception>(() =>
            {
                var dequeued = buffer.DequeueNextPacket();
            });
        }

        /// <summary>
        /// Tests if dequeuing of an incomplete packet isn't happening.
        /// </summary>
        [Test]
        public void IncompletePacket()
        {
            var buffer = new ListPacketQueue();
            var packet = new byte[] { 0xC1, 4, 255 };
            buffer.AddRange(packet);
            var dequeued = buffer.DequeueNextPacket();
            Assert.That(dequeued, Is.Null);
        }

        /// <summary>
        /// Tests if a complete packet is getting dequeued.
        /// </summary>
        [Test]
        public void CompletePacket()
        {
            var buffer = new ListPacketQueue();
            var packet = new byte[] { 0xC1, 4, 255, 0 };
            buffer.AddRange(packet);
            var dequeued = buffer.DequeueNextPacket();
            Assert.That(dequeued, Is.Not.Null);
        }
    }
}