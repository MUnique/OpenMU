// <copyright file="IChangeTradeButtonStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Trade
{
    /// <summary>
    /// Interface of a view whose implementation informs about a change in the trade button state.
    /// </summary>
    public interface IChangeTradeButtonStatePlugIn : IViewPlugIn
    {
        /// <summary>
        /// Changes the state of the other players trade button.
        /// </summary>
        /// <param name="state">The state.</param>
        void ChangeTradeButtonState(TradeButtonState state);
    }
}