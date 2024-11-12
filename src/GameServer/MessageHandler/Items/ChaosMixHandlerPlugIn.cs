// <copyright file="ChaosMixHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for chaos mix packets.
/// </summary>
[PlugIn(nameof(ChaosMixHandlerPlugIn), "Handler for chaos mix packets.")]
[Guid("0693e102-0adc-41e4-b0d4-ce22687b6dbb")]
internal class ChaosMixHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly ItemCraftAction _mixAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => ChaosMachineMixRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        ChaosMachineMixRequest message = packet;
        var socketSlot = packet.Length > 4 ? message.SocketSlot : (byte)0;
        var mixType = packet.Length > 3 ? message.MixType : ChaosMachineMixRequest.ChaosMachineMixType.ChaosWeapon;
        await this._mixAction.MixItemsAsync(player, (byte)mixType, socketSlot).ConfigureAwait(false);
    }
}