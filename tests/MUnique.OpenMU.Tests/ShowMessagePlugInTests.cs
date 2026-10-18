// <copyright file="ShowMessagePlugInTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// Tests for the <see cref="ShowMessagePlugIn"/>.
/// </summary>
[TestFixture]
public class ShowMessagePlugInTests
{
    private static readonly ClientVersion Season6 = new(6, 3, ClientLanguage.English);

    /// <summary>
    /// A slide notice is sent as <see cref="SlideNotice"/> packet: queued in the notice band,
    /// shown once, in white and at the default speed of the client.
    /// </summary>
    [Test]
    public async ValueTask SlideNoticeIsSentAsSlideNoticePacketAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer(Season6);

        await new ShowMessagePlugIn(player).ShowMessageAsync("Hola ñ", MessageType.SlideNotice).ConfigureAwait(false);

        var data = output.ToArray();
        var packet = new SlideNotice(data);
        Assert.Multiple(() =>
        {
            Assert.That(data, Has.Length.EqualTo(SlideNoticeRef.GetRequiredSize("Hola ñ")));
            Assert.That(packet.Header.Type, Is.EqualTo(SlideNotice.HeaderType));
            Assert.That(packet.Header.Code, Is.EqualTo(SlideNotice.Code));
            Assert.That(packet.Header.Length, Is.EqualTo(data.Length));
            Assert.That(packet.Type, Is.EqualTo(14));
            Assert.That(packet.LoopCount, Is.EqualTo(1));
            Assert.That(packet.LoopDelay, Is.EqualTo(0));
            Assert.That(packet.Color, Is.EqualTo(0xFFFFFFFF));
            Assert.That(packet.Speed, Is.EqualTo(0));
            Assert.That(packet.Message, Is.EqualTo("Hola ñ"));
            Assert.That(data[^1], Is.EqualTo(0), "The message has to be terminated.");
        });
    }

    /// <summary>
    /// Clients before season 1 get a slide notice as golden notice.
    /// </summary>
    [Test]
    public async ValueTask SlideNoticeFallsBackToGoldenNoticeForOldClientsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer(new ClientVersion(0, 75, ClientLanguage.Invariant));

        await new ShowMessagePlugIn(player).ShowMessageAsync("Hi", MessageType.SlideNotice).ConfigureAwait(false);

        Assert.That(output.ToArray(), Is.EqualTo(new byte[] { 0xC1, 7, 0x0D, 0, (byte)'H', (byte)'i', 0 }));
    }
}
