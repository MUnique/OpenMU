// <copyright file="IPlayerShopClosedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.PlayerShop
{
    /// <summary>
    /// Interface of a view whose implementation informs about a closed player shop.
    /// </summary>
    public interface IPlayerShopClosedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// A player closed his shop.
        /// </summary>
        /// <param name="playerWithClosedShop">Player of closing shop.</param>
        void PlayerShopClosed(Player playerWithClosedShop);
    }
}