// <copyright file="MossMerchantWindowRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameServer.RemoteView.NPC;
using NpcWindowResponse = MUnique.OpenMU.Network.Packets.ServerToClient.NpcWindowResponse;

/// <summary>
/// Tests the NPC window of the moss merchant.
/// </summary>
[TestFixture]
public class MossMerchantWindowRemoteViewTests
{
    /// <summary>
    /// Verifies that the new value is appended to the persisted enum.
    /// </summary>
    [Test]
    public void DataEnumValueIsAppended()
    {
        Assert.That((int)NpcWindow.MossMerchant, Is.EqualTo(32));
    }

    /// <summary>
    /// Verifies that the window is sent as NPC window 0x22.
    /// </summary>
    [Test]
    public async ValueTask SerializeMossMerchantWindowAsync()
    {
        var (player, output) = PlayerTestHelper.CreateRemotePlayer();

        await new OpenNpcWindowPlugIn(player).OpenNpcWindowAsync(NpcWindow.MossMerchant).ConfigureAwait(false);

        var packet = (NpcWindowResponse)output.ToArray().AsMemory();
        Assert.That((byte)packet.Window, Is.EqualTo(0x22));
        Assert.That((byte)NpcWindowResponse.NpcWindow.MossMerchant, Is.EqualTo(0x22));
    }
}
