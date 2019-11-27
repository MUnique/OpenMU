// <copyright file="TradeButtonState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Trade
{
    /// <summary>
    /// The state of the trade button.
    /// </summary>
    public enum TradeButtonState : byte
    {
        /// <summary>
        /// Trade button is not pressed. It means that the trade is not yet accepted by the trader.
        /// </summary>
        Unchecked,

        /// <summary>
        /// Trade Button is pressed. It means that the trade is accepted by the trader.
        /// </summary>
        Checked,

        /// <summary>
        /// This state is only sent to the client. After some seconds the client is changing back to normal Unchecked.
        /// </summary>
        Red,
    }
}