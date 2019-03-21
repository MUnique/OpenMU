// <copyright file="IPlayerShopOpenedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.PlayerShop
{
    /// <summary>
    /// Interface of a view whose implementation informs about an opened player shop.
    /// </summary>
    public interface IPlayerShopOpenedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// A player opened his shop.
        /// </summary>
        /// <param name="player">The player who opened the shop.</param>
        void PlayerShopOpened(Player player);
    }
}