// <copyright file="IBuyNpcItemFailedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    /// <summary>
    /// Interface of a view whose implementation informs about a failed buy attempt at a npc merchant.
    /// </summary>
    public interface IBuyNpcItemFailedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Notifies the client that the item could not be bought from the npc.
        /// </summary>
        void BuyNpcItemFailed();
    }
}