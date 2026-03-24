// <copyright file="KanturuInfoRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for 0xD1/0x00 — KanturuInfoRequest.
/// The client sends this packet periodically while the gateway dialog is open
/// (every ~5 seconds via the Refresh button or the auto-refresh timer).
/// The server responds with a fresh 0xD1/0x00 StateInfo packet so the dialog
/// shows current player count and remaining time.
/// </summary>
[PlugIn]
[Display(Name = "Kanturu Info Request Handler", Description = "Responds to 0xD1/0x00 (KanturuInfoRequest) with fresh event state info.")]
[Guid("D5F2B8A1-4C73-4E09-8A31-7B6F9E2C4D08")]
[BelongsToGroup(KanturuGroupHandlerPlugIn.GroupKey)]
internal class KanturuInfoRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => KanturuInfoRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < KanturuInfoRequest.Length)
        {
            return;
        }

        await KanturuGatewayPlugIn.SendKanturuStateInfoAsync(player).ConfigureAwait(false);
    }
}
