// <copyright file="TradeButtonHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Trade;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;
using TradeButtonState = MUnique.OpenMU.GameLogic.Views.Trade.TradeButtonState;

/// <summary>
/// Handles the trade button packets.
/// </summary>
[PlugIn("TradeButtonHandlerPlugIn", "Handles the trade button packets.")]
[Guid("4e70bdec-c890-4e7d-93a9-1801f821f322")]
internal class TradeButtonHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly TradeButtonAction _buttonAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => TradeButtonStateChange.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        TradeButtonStateChange message = packet;
        if (packet.Length < 4)
        {
            return;
        }

        await this._buttonAction.TradeButtonChangedAsync(player, (TradeButtonState)message.NewState).ConfigureAwait(false);
    }
}