// <copyright file="PacketHandlerPlugInContainerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.MessageHandler;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Unit tests for the <see cref="MainPacketHandlerPlugInContainer"/>.
/// </summary>
[TestFixture]
public class PacketHandlerPlugInContainerTest
{
    private const byte HandlerKey = 0xF1;

    private static readonly ClientVersion Season6E3English = new (6, 3, ClientLanguage.English);

    private static readonly ClientVersion Season9E2English = new (9, 2, ClientLanguage.English);

    /// <summary>
    /// Tests if the the plug in of correct version is selected when the plugin for the exact version is available.
    /// </summary>
    [Test]
    public void SelectPlugInOfCorrectVersionWhenExactVersionIsAvailable()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason1>();
        manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6>();
        manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason9>();
        var clientVersionProvider = new Mock<IClientVersionProvider>();
        clientVersionProvider.Setup(p => p.ClientVersion).Returns(Season6E3English);
        var containerForSeason6 = new MainPacketHandlerPlugInContainer(clientVersionProvider.Object, manager, new NullLoggerFactory());
        containerForSeason6.Initialize();
        var handler = containerForSeason6[HandlerKey];
        Assert.That(handler!.GetType(), Is.EqualTo(typeof(PacketHandlerSeason6)));
    }

    /// <summary>
    /// Tests if the the plug in of correct version is selected when only plugins for lower versions are available.
    /// </summary>
    [Test]
    public void SelectPlugInOfCorrectVersionWhenLowerVersionsAreAvailable()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason1>();
        manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6>();
        var clientVersionProvider = new Mock<IClientVersionProvider>();
        clientVersionProvider.Setup(p => p.ClientVersion).Returns(Season9E2English);
        var containerForSeason9 = new MainPacketHandlerPlugInContainer(clientVersionProvider.Object, manager, new NullLoggerFactory());
        containerForSeason9.Initialize();
        var handler = containerForSeason9[HandlerKey];
        Assert.That(handler!.GetType(), Is.EqualTo(typeof(PacketHandlerSeason6)));
    }

    /// <summary>
    /// Tests if plugins of the correct language are selected.
    /// </summary>
    [Test]
    public void SelectPlugInOfCorrectLanguage()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6Chinese>();
        manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6>();
        var clientVersionProvider = new Mock<IClientVersionProvider>();
        clientVersionProvider.Setup(p => p.ClientVersion).Returns(Season6E3English);
        var containerForSeason6 = new MainPacketHandlerPlugInContainer(clientVersionProvider.Object, manager, new NullLoggerFactory());
        containerForSeason6.Initialize();
        var handler = containerForSeason6[HandlerKey];
        Assert.That(handler!.GetType(), Is.EqualTo(typeof(PacketHandlerSeason6)));
    }

    /// <summary>
    /// Tests if plugins of invariant language and version are selected.
    /// </summary>
    [Test]
    public void SelectInvariantPlugIn()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerInvariant>();
        var clientVersionProvider = new Mock<IClientVersionProvider>();
        clientVersionProvider.Setup(p => p.ClientVersion).Returns(Season6E3English);
        var containerForSeason6 = new MainPacketHandlerPlugInContainer(clientVersionProvider.Object, manager, new NullLoggerFactory());
        containerForSeason6.Initialize();
        var handler = containerForSeason6[HandlerKey];
        Assert.That(handler!.GetType(), Is.EqualTo(typeof(PacketHandlerInvariant)));
    }

    /// <summary>
    /// Tests if another plugin is getting 'effective' when the currently effective plugin gets deactivated.
    /// </summary>
    [Test]
    public void SelectPlugInAfterDeactivation()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason1>();
        manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6>();
        var clientVersionProvider = new Mock<IClientVersionProvider>();
        clientVersionProvider.Setup(p => p.ClientVersion).Returns(Season6E3English);
        var containerForSeason6 = new MainPacketHandlerPlugInContainer(clientVersionProvider.Object, manager, new NullLoggerFactory());
        containerForSeason6.Initialize();
        manager.DeactivatePlugIn(typeof(PacketHandlerSeason6));

        var handler = containerForSeason6[HandlerKey];
        Assert.That(handler, Is.Not.Null);
        Assert.That(handler!.GetType(), Is.EqualTo(typeof(PacketHandlerSeason1)));
    }

    /// <summary>
    /// Tests if the language specific plugin has priority over the invariant one.
    /// </summary>
    [Test]
    public void SelectLanguageSpecificOverInvariant()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6>();
        manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6Chinese>();
        manager.RegisterPlugIn<IPacketHandlerPlugIn, PacketHandlerSeason6English>();
        var clientVersionProvider = new Mock<IClientVersionProvider>();
        clientVersionProvider.Setup(p => p.ClientVersion).Returns(Season6E3English);
        var containerForSeason6 = new MainPacketHandlerPlugInContainer(clientVersionProvider.Object, manager, new NullLoggerFactory());
        containerForSeason6.Initialize();
        var handler = containerForSeason6[HandlerKey];
        Assert.That(handler!.GetType(), Is.EqualTo(typeof(PacketHandlerSeason6English)));
    }

    /// <summary>
    /// Test packet handler implementation for season 1.
    /// </summary>
    [MinimumClient(1, 0, ClientLanguage.Invariant)]
    public class PacketHandlerSeason1 : IPacketHandlerPlugIn
    {
        /// <inheritdoc />
        public byte Key => HandlerKey;

        /// <inheritdoc />
        public bool IsEncryptionExpected => false;

        /// <inheritdoc />
        public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
        {
            // does nothing here
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>
    /// Test packet handler implementation for season 6.
    /// </summary>
    [MinimumClient(6, 3, ClientLanguage.Invariant)]
    public class PacketHandlerSeason6 : IPacketHandlerPlugIn
    {
        /// <inheritdoc />
        public byte Key => HandlerKey;

        /// <inheritdoc />
        public bool IsEncryptionExpected => false;

        /// <inheritdoc />
        public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
        {
            // does nothing here
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>
    /// Test packet handler implementation for season 6 english.
    /// </summary>
    [MinimumClient(6, 3, ClientLanguage.English)]
    public class PacketHandlerSeason6English : IPacketHandlerPlugIn
    {
        /// <inheritdoc />
        public byte Key => HandlerKey;

        /// <inheritdoc />
        public bool IsEncryptionExpected => false;

        /// <inheritdoc />
        public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
        {
            // does nothing here
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>
    /// Test packet handler implementation for season 6 chinese.
    /// </summary>
    [MinimumClient(6, 3, ClientLanguage.Chinese)]
    public class PacketHandlerSeason6Chinese : IPacketHandlerPlugIn
    {
        /// <inheritdoc />
        public byte Key => HandlerKey;

        /// <inheritdoc />
        public bool IsEncryptionExpected => false;

        /// <inheritdoc />
        public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
        {
            // does nothing here
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>
    /// Test packet handler implementation for season 9.
    /// </summary>
    [MinimumClient(9, 2, ClientLanguage.Invariant)]
    public class PacketHandlerSeason9 : IPacketHandlerPlugIn
    {
        /// <inheritdoc />
        public byte Key => HandlerKey;

        /// <inheritdoc />
        public bool IsEncryptionExpected => false;

        /// <inheritdoc />
        public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
        {
            // does nothing here
            return ValueTask.CompletedTask;
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
        public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
        {
            // does nothing here
            return ValueTask.CompletedTask;
        }
    }
}