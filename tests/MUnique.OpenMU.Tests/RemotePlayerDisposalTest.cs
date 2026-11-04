// <copyright file="RemotePlayerDisposalTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.MessageHandler;
using MUnique.OpenMU.GameServer.MessageHandler.Character;
using MUnique.OpenMU.GameServer.MessageHandler.Gens;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests that a disconnected <see cref="MUnique.OpenMU.GameServer.RemoteView.RemotePlayer"/> is released completely.
/// </summary>
/// <remarks>
/// Every accepted connection creates a remote player, even one which never logs in. Its packet handler and
/// view plugin containers subscribe to the server-wide <see cref="PlugInManager"/>, so a missed unsubscription
/// keeps every connection the server ever accepted in memory.
/// </remarks>
[TestFixture]
public class RemotePlayerDisposalTest
{
    /// <summary>
    /// Tests if a disposed remote player, including its per-player group packet handlers, isn't kept alive by the plugin manager.
    /// </summary>
    [Test]
    public async ValueTask DisposedRemotePlayerIsNotKeptAliveByPlugInManagerAsync()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<IPacketHandlerPlugIn, CharacterGroupHandlerPlugIn>();

        var playerReference = await CreateAndDisposePlayerAsync(manager).ConfigureAwait(false);

        CollectibleAssert.IsCollected(playerReference, "disposed remote player");
        GC.KeepAlive(manager);
    }

    /// <summary>
    /// Tests if disposing a remote player leaves the services it shares with the whole server usable.
    /// The packet handler containers hold them in a <see cref="System.ComponentModel.Design.ServiceContainer"/>,
    /// which disposes its services when it's disposed itself.
    /// </summary>
    [Test]
    public async ValueTask DisposingRemotePlayerKeepsSharedLoggerFactoryUsableAsync()
    {
        using var loggerFactory = new LoggerFactory();
        var manager = new PlugInManager(null, loggerFactory, null, null);
        manager.RegisterPlugIn<IPacketHandlerPlugIn, CharacterGroupHandlerPlugIn>();
        var (player, _) = CastleSiegeRemoteViewTestHelper.CreatePlayer(CreateGameContext(manager, loggerFactory));

        await player.DisposeAsync().ConfigureAwait(false);

        Assert.DoesNotThrow(() => loggerFactory.CreateLogger("next connection"));
    }

    /// <summary>
    /// Tests if a group packet handler which a client version change makes unsuitable is released
    /// without disposing the player, which it holds as its <see cref="IClientVersionProvider"/>.
    /// </summary>
    [Test]
    public async ValueTask ClientVersionChangeDoesNotDisposePlayerAsync()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<IPacketHandlerPlugIn, GensGroupHandlerPlugIn>();
        var (player, _) = CastleSiegeRemoteViewTestHelper.CreatePlayer(CreateGameContext(manager, new NullLoggerFactory()));
        player.ClientVersion = new ClientVersion(6, 3, ClientLanguage.Invariant);
        Assert.That(player.MainPacketHandler.AvailableStrategies, Is.Not.Empty, "The gens group handler should have been created for season 6.");

        player.ClientVersion = new ClientVersion(1, 0, ClientLanguage.Invariant);

        Assert.That(player.MainPacketHandler.AvailableStrategies, Is.Empty, "The gens group handler isn't suitable for season 1.");
        Assert.That(player.IsDisposed, Is.False);
        await player.DisposeAsync().ConfigureAwait(false);
    }

    private static IGameServerContext CreateGameContext(PlugInManager manager, ILoggerFactory loggerFactory)
    {
        var gameContext = new Mock<IGameServerContext>();
        gameContext.Setup(c => c.PersistenceContextProvider)
            .Returns(new Mock<IPersistenceContextProvider> { DefaultValue = DefaultValue.Mock }.Object);
        gameContext.Setup(c => c.Configuration).Returns(new GameConfiguration());
        gameContext.Setup(c => c.PlugInManager).Returns(manager);
        gameContext.Setup(c => c.LoggerFactory).Returns(loggerFactory);
        return gameContext.Object;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static async ValueTask<WeakReference> CreateAndDisposePlayerAsync(PlugInManager manager)
    {
        var (player, _) = CastleSiegeRemoteViewTestHelper.CreatePlayer(CreateGameContext(manager, new NullLoggerFactory()));
        Assert.That(player.MainPacketHandler.AvailableStrategies, Is.Not.Empty, "The group handler should have been created for the player.");
        await player.DisposeAsync().ConfigureAwait(false);
        return new WeakReference(player);
    }
}
