// <copyright file="TradeFinishedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Trade;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ITradeFinishedPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("TradeFinishedPlugIn", "The default implementation of the ITradeFinishedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("ec11fe95-31c6-4a2e-942d-4d10a84830c1")]
public class TradeFinishedPlugIn : ITradeFinishedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="TradeFinishedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public TradeFinishedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public void TradeFinished(TradeResult tradeResult)
    {
        this._player.Connection?.SendTradeFinished(Convert(tradeResult));

        if (tradeResult != TradeResult.TimedOut)
        {
            this._player.ViewPlugIns.GetPlugIn<IUpdateInventoryListPlugIn>()?.UpdateInventoryList();
            this._player.ViewPlugIns.GetPlugIn<IUpdateMoneyPlugIn>()?.UpdateMoney();
        }
    }

    private static TradeFinished.TradeResult Convert(TradeResult tradeResult)
    {
        return tradeResult switch
        {
            TradeResult.Cancelled => Network.Packets.ServerToClient.TradeFinished.TradeResult.Cancelled,
            TradeResult.Success => Network.Packets.ServerToClient.TradeFinished.TradeResult.Success,
            TradeResult.FailedByFullInventory => Network.Packets.ServerToClient.TradeFinished.TradeResult.FailedByFullInventory,
            TradeResult.TimedOut => Network.Packets.ServerToClient.TradeFinished.TradeResult.TimedOut,
            TradeResult.FailedByItemsNotAllowedToTrade => Network.Packets.ServerToClient.TradeFinished.TradeResult.FailedByItemsNotAllowedToTrade,
            _ => throw new ArgumentException($"TradeResult {tradeResult} not mapped to a byte value."),
        };
    }
}