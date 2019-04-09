﻿// <copyright file="PacketHandlerPlugInContainerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System;
    using Moq;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.GameServer.MessageHandler;
    using MUnique.OpenMU.GameServer.MessageHandler.Login;
    using MUnique.OpenMU.PlugIns;
    using NUnit.Framework;

    /// <summary>
    /// Unit tests for the <see cref="MainPacketHandlerPlugInContainer"/>.
    /// </summary>
    [TestFixture]
    public class PacketHandlerPlugInContainerTest
    {
        private const byte HandlerKey = 0xF1;

        private static readonly ClientVersion Season6E3English = new ClientVersion(6, 3, ClientLanguage.English);

        private static readonly ClientVersion Season9E2English = new ClientVersion(9, 2, ClientLanguage.English);

        /// <summary>
        /// Tests if the the plug in of correct version is selected when the plugin for the exact version is available.
        /// </summary>
        [Test]
        public void SelectPlugInOfCorrectVersionWhenExactVersionIsAvailable()
        {
            var manager = new PlugInManager();
            manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason1>();
            manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6>();
            manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason9>();
            var clientVersionProvider = new Mock<IClientVersionProvider>();
            clientVersionProvider.Setup(p => p.ClientVersion).Returns(Season6E3English);
            var containerForSeason6 = new MainPacketHandlerPlugInContainer(clientVersionProvider.Object, manager);
            containerForSeason6.Initialize();
            var handler = containerForSeason6[HandlerKey];
            Assert.That(handler.GetType(), Is.EqualTo(typeof(PacketHandlerSeason6)));
        }

        /// <summary>
        /// Tests if the the plug in of correct version is selected when only plugins for lower versions are available.
        /// </summary>
        [Test]
        public void SelectPlugInOfCorrectVersionWhenLowerVersionsAreAvailable()
        {
            var manager = new PlugInManager();
            manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason1>();
            manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6>();
            var clientVersionProvider = new Mock<IClientVersionProvider>();
            clientVersionProvider.Setup(p => p.ClientVersion).Returns(Season9E2English);
            var containerForSeason9 = new MainPacketHandlerPlugInContainer(clientVersionProvider.Object, manager);
            containerForSeason9.Initialize();
            var handler = containerForSeason9[HandlerKey];
            Assert.That(handler.GetType(), Is.EqualTo(typeof(PacketHandlerSeason6)));
        }

        /// <summary>
        /// Tests if plugins of the correct language are selected.
        /// </summary>
        [Test]
        public void SelectPlugInOfCorrectLanguage()
        {
            var manager = new PlugInManager();
            manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6Chinese>();
            manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6>();
            var clientVersionProvider = new Mock<IClientVersionProvider>();
            clientVersionProvider.Setup(p => p.ClientVersion).Returns(Season6E3English);
            var containerForSeason6 = new MainPacketHandlerPlugInContainer(clientVersionProvider.Object, manager);
            containerForSeason6.Initialize();
            var handler = containerForSeason6[HandlerKey];
            Assert.That(handler.GetType(), Is.EqualTo(typeof(PacketHandlerSeason6)));
        }

        /// <summary>
        /// Tests if plugins of invariant language and version are selected.
        /// </summary>
        [Test]
        public void SelectInvariantPlugIn()
        {
            var manager = new PlugInManager();
            manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerInvariant>();
            var clientVersionProvider = new Mock<IClientVersionProvider>();
            clientVersionProvider.Setup(p => p.ClientVersion).Returns(Season6E3English);
            var containerForSeason6 = new MainPacketHandlerPlugInContainer(clientVersionProvider.Object, manager);
            containerForSeason6.Initialize();
            var handler = containerForSeason6[HandlerKey];
            Assert.That(handler.GetType(), Is.EqualTo(typeof(PacketHandlerInvariant)));
        }

        /// <summary>
        /// Tests if another plugin is getting 'effective' when the currently effective plugin gets deactivated.
        /// </summary>
        [Test]
        public void SelectPlugInAfterDeactivation()
        {
            var manager = new PlugInManager();
            manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason1>();
            manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6>();
            var clientVersionProvider = new Mock<IClientVersionProvider>();
            clientVersionProvider.Setup(p => p.ClientVersion).Returns(Season6E3English);
            var containerForSeason6 = new MainPacketHandlerPlugInContainer(clientVersionProvider.Object, manager);
            containerForSeason6.Initialize();
            manager.DeactivatePlugIn(typeof(PacketHandlerSeason6));
            var handler = containerForSeason6[HandlerKey];
            Assert.That(handler.GetType(), Is.EqualTo(typeof(PacketHandlerSeason1)));
        }

        /// <summary>
        /// Tests if the language specific plugin has priority over the invariant one.
        /// </summary>
        [Test]
        public void SelectLanguageSpecificOverInvariant()
        {
            var manager = new PlugInManager();
            manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6>();
            manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6Chinese>();
            manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6English>();
            var clientVersionProvider = new Mock<IClientVersionProvider>();
            clientVersionProvider.Setup(p => p.ClientVersion).Returns(Season6E3English);
            var containerForSeason6 = new MainPacketHandlerPlugInContainer(clientVersionProvider.Object, manager);
            containerForSeason6.Initialize();
            var handler = containerForSeason6[HandlerKey];
            Assert.That(handler.GetType(), Is.EqualTo(typeof(PacketHandlerSeason6English)));
        }

        /// <summary>
        /// Test packet handler implementation for season 1.
        /// </summary>
        [Client(1, 0, ClientLanguage.Invariant)]
        public class PacketHandlerSeason1 : IPacketHandlerPlugIn
        {
            /// <inheritdoc />
            public byte Key => HandlerKey;

            /// <inheritdoc />
            public bool IsEncryptionExpected => false;

            /// <inheritdoc />
            public void HandlePacket(Player player, Span<byte> packet)
            {
                // does nothing here
            }
        }

        /// <summary>
        /// Test packet handler implementation for season 6.
        /// </summary>
        [Client(6, 3, ClientLanguage.Invariant)]
        public class PacketHandlerSeason6 : IPacketHandlerPlugIn
        {
            /// <inheritdoc />
            public byte Key => HandlerKey;

            /// <inheritdoc />
            public bool IsEncryptionExpected => false;

            /// <inheritdoc />
            public void HandlePacket(Player player, Span<byte> packet)
            {
                // does nothing here
            }
        }

        /// <summary>
        /// Test packet handler implementation for season 6 english.
        /// </summary>
        [Client(6, 3, ClientLanguage.English)]
        public class PacketHandlerSeason6English : IPacketHandlerPlugIn
        {
            /// <inheritdoc />
            public byte Key => HandlerKey;

            /// <inheritdoc />
            public bool IsEncryptionExpected => false;

            /// <inheritdoc />
            public void HandlePacket(Player player, Span<byte> packet)
            {
                // does nothing here
            }
        }

        /// <summary>
        /// Test packet handler implementation for season 6 chinese.
        /// </summary>
        [Client(6, 3, ClientLanguage.Chinese)]
        public class PacketHandlerSeason6Chinese : IPacketHandlerPlugIn
        {
            /// <inheritdoc />
            public byte Key => HandlerKey;

            /// <inheritdoc />
            public bool IsEncryptionExpected => false;

            /// <inheritdoc />
            public void HandlePacket(Player player, Span<byte> packet)
            {
                // does nothing here
            }
        }

        /// <summary>
        /// Test packet handler implementation for season 9.
        /// </summary>
        [Client(9, 2, ClientLanguage.Invariant)]
        public class PacketHandlerSeason9 : IPacketHandlerPlugIn
        {
            /// <inheritdoc />
            public byte Key => HandlerKey;

            /// <inheritdoc />
            public bool IsEncryptionExpected => false;

            /// <inheritdoc />
            public void HandlePacket(Player player, Span<byte> packet)
            {
                // does nothing here
            }
        }

        /// <summary>
        /// Invariant test packet handler implementation for all versions.
        /// </summary>
        public class PacketHandlerInvariant : IPacketHandlerPlugIn
        {
            /// <inheritdoc />
            public byte Key => HandlerKey;

            /// <inheritdoc />
            public bool IsEncryptionExpected => false;

            /// <inheritdoc />
            public void HandlePacket(Player player, Span<byte> packet)
            {
                // does nothing here
            }
        }
    }
}