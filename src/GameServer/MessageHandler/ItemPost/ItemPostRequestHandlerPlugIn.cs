// <copyright file="ItemPostRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.ItemPost;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemPost;
using MUnique.OpenMU.GameServer.MessageHandler.Character;
using MUnique.OpenMU.GameServer.RemoteView.Inventory;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles item post requests (F3 40).
/// </summary>
[PlugIn(nameof(ItemPostRequestHandlerPlugIn), "Handles item post requests (F3 40).")]
[Guid("1A8F8A31-72E2-423B-97F2-1F7A874953E2")]
[BelongsToGroup(CharacterGroupHandlerPlugIn.GroupKey)]
internal class ItemPostRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly ItemPostAction _itemPostAction = new ();

    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public byte Key => ItemPostRequest.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < ItemPostRequest.Length)
        {
            return;
        }

        ItemPostRequest message = packet;
        await this._itemPostAction.PostItemAsync(player, message.StorageType.Convert(), message.ItemSlot).ConfigureAwait(false);
    }
}
