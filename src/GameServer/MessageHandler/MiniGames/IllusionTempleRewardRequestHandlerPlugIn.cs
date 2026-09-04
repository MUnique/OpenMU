// <copyright file="IllusionTempleRewardRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameServer.MessageHandler.MuHelper;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for illusion temple reward request packets - sent by the client when the player clicks the
/// "Close" button on the result dialog after the event ended.
/// </summary>
/// <remarks>
/// The packet belongs to the 0xBF group, which is dispatched by the <see cref="MuHelperGroupHandler"/>.
/// Therefore this is a sub packet handler which is selected by the sub code, and not a handler of its own.
/// </remarks>
[PlugIn]
[Guid("8B4E6C2A-9A3D-4E7F-8C1B-2D5A6F9E0B3C")]
[BelongsToGroup(MuHelperGroupHandler.GroupKey)]
internal class IllusionTempleRewardRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => IllusionTempleRewardRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < IllusionTempleRewardRequest.Length
            || player.CurrentMiniGame is not IllusionTempleContext illusionTemple)
        {
            return;
        }

        await illusionTemple.ClaimRewardAsync(player).ConfigureAwait(false);
    }
}
