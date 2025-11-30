// <copyright file="ItemPostInfoRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.ItemPost;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemPost;
using MUnique.OpenMU.GameServer.MessageHandler.Character;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles requests for item post information (F3 41).
/// </summary>
[PlugIn(nameof(ItemPostInfoRequestHandlerPlugIn), "Handles requests for item post information (F3 41).")]
[Guid("4F1A45D1-5B33-4EA0-828E-8DBD2FB7530F")]
[BelongsToGroup(CharacterGroupHandlerPlugIn.GroupKey)]
internal class ItemPostInfoRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly ItemPostInfoAction _infoAction = new ();

    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public byte Key => ItemPostInfoRequest.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < ItemPostInfoRequest.Length)
        {
            return;
        }

        ItemPostInfoRequest message = packet;
        await this._infoAction.SendItemInfoAsync(player, message.PostId).ConfigureAwait(false);
    }
}
