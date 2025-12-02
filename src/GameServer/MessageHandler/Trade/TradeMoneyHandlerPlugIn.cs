// <copyright file="TradeMoneyHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Trade;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the trade money packets.
/// </summary>
[PlugIn("TradeMoneyHandlerPlugIn", "Handles the trade money packets.")]
[Guid("3c18f0ca-4ad8-4e07-a111-0acbe81256ca")]
internal class TradeMoneyHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly TradeMoneyAction _tradeAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => SetTradeMoney.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        SetTradeMoney message = packet;
        await this._tradeAction.TradeMoneyAsync(player, message.Amount).ConfigureAwait(false);
    }
}