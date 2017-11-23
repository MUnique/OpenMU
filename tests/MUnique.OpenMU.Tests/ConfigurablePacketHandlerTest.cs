// <copyright file="ConfigurablePacketHandlerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.Interfaces;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;
    using Rhino.Mocks;

    /// <summary>
    /// Tests for the <see cref="ConfigurableMainPacketHandler"/>.
    /// </summary>
    [TestFixture]
    public class ConfigurablePacketHandlerTest
    {
        private const byte PacketType = 0xF1;

        /// <summary>
        /// Tests the initialization of a configured packet handler.
        /// </summary>
        [Test]
        public void InitializationOfPacketHandler()
        {
            var packetHandler = this.CreateMainPacketHandler(false);
            var handler = packetHandler.GetPacketHandler(PacketType) as TestPacketHandler;
            Assert.That(handler, Is.Not.Null);
            Assert.That(handler.ConstructorCalled, Is.True);
        }

        /// <summary>
        /// Tests the creation of a sub packet handler.
        /// </summary>
        [Test]
        public void SubPacketHandlerCreation()
        {
            var mainPacketHandler = this.CreateMainPacketHandlerWithSubPacketHandler();
            var handler = mainPacketHandler.GetPacketHandler(PacketType) as ConfigurablePacketHandler;
            var subHandler = handler?.GetPacketHandler(PacketType) as TestPacketHandler;
            Assert.That(subHandler, Is.Not.Null);
        }

        /// <summary>
        /// Tests if a packet is handled by a sub packet handler.
        /// </summary>
        [Test]
        public void SubPacketHandlerHandling()
        {
            var mainPacketHandler = this.CreateMainPacketHandlerWithSubPacketHandler();
            var handler = mainPacketHandler.GetPacketHandler(PacketType) as ConfigurablePacketHandler;
            var subHandler = handler?.GetPacketHandler(PacketType) as TestPacketHandler;
            Assert.That(subHandler, Is.Not.Null);
            mainPacketHandler.HandlePacket(null, new byte[] { 0xC1, 0x04, PacketType, PacketType });
            Assert.That(subHandler.HandlePacketCalled, Is.True);
        }

        /// <summary>
        /// Tests if a packet is handled if it the packet is encrypted when it's required.
        /// </summary>
        /// <param name="withEncryptedPacket">If set to <c>true</c> the tested packet is encrypted.</param>
        /// <param name="isEncryptionRequired">If set to <c>true</c> the packet handler requires encryption.</param>
        /// <param name="isHandlePacketCalledExpectation">If set to <c>true</c> it's expected that the packet was handled.</param>
        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(true, false, true)]
        [TestCase(true, true, true)]
        public void TestHandlePacketCalled(bool withEncryptedPacket, bool isEncryptionRequired, bool isHandlePacketCalledExpectation)
        {
            var packetHandler = this.CreateMainPacketHandler(isEncryptionRequired);
            packetHandler.HandlePacket(null, new byte[] { withEncryptedPacket ? (byte)0xC3 : (byte)0xC1, 0x03, PacketType });
            var handler = packetHandler.GetPacketHandler(PacketType) as TestPacketHandler;
            Assert.That(handler, Is.Not.Null);
            Assert.That(handler.HandlePacketCalled, isHandlePacketCalledExpectation ? (Constraint)Is.True : Is.False);
        }

        private ConfigurableMainPacketHandler CreateMainPacketHandlerWithSubPacketHandler()
        {
            var mainConfiguration = MockRepository.GenerateStub<MainPacketHandlerConfiguration>();
            mainConfiguration.Stub(c => c.PacketHandlers).Return(new List<PacketHandlerConfiguration>());

            var packetHandler = MockRepository.GenerateStub<PacketHandlerConfiguration>();
            packetHandler.Stub(p => p.SubPacketHandlers).Return(new List<PacketHandlerConfiguration>());
            packetHandler.PacketIdentifier = PacketType;
            packetHandler.SubPacketHandlers.Add(new PacketHandlerConfiguration
            {
                PacketIdentifier = PacketType,
                PacketHandlerClassName = typeof(TestPacketHandler).AssemblyQualifiedName
            });
            mainConfiguration.PacketHandlers.Add(packetHandler);
            var gameServerContext = MockRepository.GenerateStub<IGameServerContext>();
            gameServerContext.Stub(g => g.FriendServer).Return(MockRepository.GenerateStub<IFriendServer>());
            return new ConfigurableMainPacketHandler(mainConfiguration, gameServerContext);
        }

        private ConfigurableMainPacketHandler CreateMainPacketHandler(bool encryptionNeeded)
        {
            var mainConfiguration = MockRepository.GenerateStub<MainPacketHandlerConfiguration>();
            mainConfiguration.Stub(c => c.PacketHandlers).Return(new List<PacketHandlerConfiguration>());
            mainConfiguration.PacketHandlers.Add(
                new PacketHandlerConfiguration
                {
                    NeedsToBeEncrypted = encryptionNeeded,
                    PacketIdentifier = PacketType,
                    PacketHandlerClassName = typeof(TestPacketHandler).AssemblyQualifiedName
                });
            var gameServerContext = MockRepository.GenerateStub<IGameServerContext>();
            gameServerContext.Stub(g => g.FriendServer).Return(MockRepository.GenerateStub<IFriendServer>());
            return new ConfigurableMainPacketHandler(mainConfiguration, gameServerContext);
        }
    }
}
