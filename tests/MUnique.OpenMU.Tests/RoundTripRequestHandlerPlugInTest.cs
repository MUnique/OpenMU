// <copyright file="RoundTripRequestHandlerPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.IO;
using System.IO.Pipelines;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.MessageHandler;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;

/// <summary>
/// Tests for the <see cref="RoundTripRequestHandlerPlugIn"/>, with which the client measures its ping.
/// </summary>
[TestFixture]
public class RoundTripRequestHandlerPlugInTest
{
    /// <summary>
    /// Tests that a request is answered with the token it carried.
    /// </summary>
    [Test]
    public async Task RequestIsAnsweredWithItsTokenAsync()
    {
        var (player, output) = CreatePlayer();
        var request = new byte[RoundTripRequestRef.Length];
        _ = new RoundTripRequestRef(request) { Token = 0xDEADBEEF };

        await new RoundTripRequestHandlerPlugIn().HandlePacketAsync(player, request).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data, Has.Length.EqualTo(RoundTripResponseRef.Length));
        var response = new RoundTripResponseRef(data);
        Assert.That(response.Header.Code, Is.EqualTo(0xF5));
        Assert.That(response.Header.SubCode, Is.EqualTo(0x0D));
        Assert.That(response.Token, Is.EqualTo(0xDEADBEEF));
    }

    /// <summary>
    /// Tests that a request which is too short to carry a token isn't answered.
    /// </summary>
    [Test]
    public async Task ShortRequestIsIgnoredAsync()
    {
        var (player, output) = CreatePlayer();
        var request = new byte[RoundTripRequestRef.Length];
        _ = new RoundTripRequestRef(request);

        await new RoundTripRequestHandlerPlugIn().HandlePacketAsync(player, request.AsMemory(0, RoundTripRequestRef.Length - 1)).ConfigureAwait(false);

        Assert.That(output.Length, Is.Zero);
    }

    private static (RemotePlayer Player, MemoryStream Output) CreatePlayer()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<IViewPlugIn, RoundTripViewPlugIn>();
        var gameContext = new Mock<IGameServerContext>();
        gameContext.Setup(c => c.PersistenceContextProvider).Returns(new Mock<IPersistenceContextProvider>().Object);
        gameContext.Setup(c => c.Configuration).Returns(new GameConfiguration());
        gameContext.Setup(c => c.PlugInManager).Returns(manager);
        gameContext.Setup(c => c.FeaturePlugIns).Returns(new FeaturePlugInContainer(manager));
        gameContext.Setup(c => c.LoggerFactory).Returns(new NullLoggerFactory());

        var output = new MemoryStream();
        var writer = PipeWriter.Create(output, new StreamPipeWriterOptions(leaveOpen: true));
        var connection = new Mock<IConnection>();
        connection.SetupGet(c => c.Connected).Returns(true);
        connection.SetupGet(c => c.Output).Returns(writer);
        connection.SetupGet(c => c.OutputLock).Returns(new AsyncLock());
        return (new RemotePlayer(gameContext.Object, connection.Object, new ClientVersion(106, 3, ClientLanguage.Invariant)), output);
    }
}
