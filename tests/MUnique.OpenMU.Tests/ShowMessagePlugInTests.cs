// <copyright file="ShowMessagePlugInTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// Tests for the <see cref="ShowMessagePlugIn"/>.
/// </summary>
[TestFixture]
public class ShowMessagePlugInTests
{
    private static readonly ClientVersion Season6 = new(6, 3, ClientLanguage.English);

    /// <summary>
    /// A slide notice is sent as 0x0D packet of type 14 with the extended fields the client reads for it.
    /// </summary>
    [Test]
    public async ValueTask SlideNoticeWritesTheExtendedNoticePacketAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer(Season6);

        await new ShowMessagePlugIn(player).ShowMessageAsync("Hola ñ", MessageType.SlideNotice).ConfigureAwait(false);

        byte[] expected =
        [
            0xC1, 21, 0x0D,
            14, // type: bold notice band
            1, // loop count
            0, // padding
            0, 0, // loop delay
            0xFF, 0xFF, 0xFF, 0xFF, // color
            0, // default speed
            (byte)'H', (byte)'o', (byte)'l', (byte)'a', (byte)' ', 0xC3, 0xB1,
            0, // terminator
        ];
        Assert.That(output.ToArray(), Is.EqualTo(expected));
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
