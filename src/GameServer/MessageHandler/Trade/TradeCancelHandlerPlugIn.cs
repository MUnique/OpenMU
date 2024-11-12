// <copyright file="TradeCancelHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Trade;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the trade cancel packets.
/// </summary>
[PlugIn("TradeCancelHandlerPlugIn", "Handles the trade cancel packets.")]
[Guid("13c7ba03-0ec2-4f41-bc0a-30fb9a035240")]
internal class TradeCancelHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly TradeCancelAction _cancelHandler = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => TradeCancel.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._cancelHandler.CancelTradeAsync(player).ConfigureAwait(false);
    }
}