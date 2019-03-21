// <copyright file="IShowMerchantStoreItemListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface of a view whose implementation informs about the available items in an opened npc merchant dialog.
    /// </summary>
    public interface IShowMerchantStoreItemListPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the merchant store item list.
        /// </summary>
        /// <param name="storeItems">The store items.</param>
        void ShowMerchantStoreItemList(ICollection<Item> storeItems);
    }
}