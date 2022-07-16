// <copyright file="ChangeTradeButtonStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Trade;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IChangeTradeButtonStatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ChangeTradeButtonStatePlugIn", "The default implementation of the IChangeTradeButtonStatePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("d33144a1-634a-4c7a-9add-2086c3b9b0ea")]
public class ChangeTradeButtonStatePlugIn : IChangeTradeButtonStatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChangeTradeButtonStatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ChangeTradeButtonStatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    /// <remarks>
    /// This message is sent when the trading partner presses or un-presses the trade accept button.
    /// Examples:
    /// C1 04 3C 00 Partner unpressed
    /// C1 04 3C 01 Partner pressed
    /// C1 04 3C 02 Button is red for x seconds.
    /// </remarks>
    public async ValueTask ChangeTradeButtonStateAsync(TradeButtonState state)
    {
        await this._player.Connection.SendTradeButtonStateChangedAsync(Convert(state)).ConfigureAwait(false);
    }

    private static TradeButtonStateChanged.TradeButtonState Convert(TradeButtonState state)
    {
        return state switch
        {
            TradeButtonState.Unchecked => TradeButtonStateChanged.TradeButtonState.Unchecked,
            TradeButtonState.Checked => TradeButtonStateChanged.TradeButtonState.Checked,
            TradeButtonState.Red => TradeButtonStateChanged.TradeButtonState.Red,
            _ => throw new ArgumentException($"Unhandled enum value {state}."),
        };
    }
}