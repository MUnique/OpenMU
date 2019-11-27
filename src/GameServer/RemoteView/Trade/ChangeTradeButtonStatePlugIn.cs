// <copyright file="ChangeTradeButtonStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Trade
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Trade;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IChangeTradeButtonStatePlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ChangeTradeButtonStatePlugIn", "The default implementation of the IChangeTradeButtonStatePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("d33144a1-634a-4c7a-9add-2086c3b9b0ea")]
    public class ChangeTradeButtonStatePlugIn : IChangeTradeButtonStatePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTradeButtonStatePlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ChangeTradeButtonStatePlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        /// <remarks>
        /// This message is sent when the trading partner presses or un-presses the trade accept button.
        /// Examples:
        /// C1 04 3C 00 Partner unpressed
        /// C1 04 3C 01 Partner pressed
        /// C1 04 3C 02 Button is red for x seconds.
        /// </remarks>
        public void ChangeTradeButtonState(TradeButtonState state)
        {
            using var writer = this.player.Connection.StartSafeWrite(TradeButtonStateChanged.HeaderType, TradeButtonStateChanged.Length);
            _ = new TradeButtonStateChanged(writer.Span)
            {
                State = Convert(state),
            };
            writer.Commit();
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
}