// <copyright file="ChangeOnlineStateHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Messenger;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for online state change packets.
/// </summary>
[PlugIn("ChangeOnlineStateHandlerPlugIn", "Handler for online state change packets.")]
[Guid("ff1d0b4e-4748-4ee0-b68e-f42b700c0f63")]
internal class ChangeOnlineStateHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly ChangeOnlineStateAction _changeAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => SetFriendOnlineState.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        SetFriendOnlineState message = packet;
        await this._changeAction.SetOnlineStateAsync(player, message.OnlineState).ConfigureAwait(false);
    }
}