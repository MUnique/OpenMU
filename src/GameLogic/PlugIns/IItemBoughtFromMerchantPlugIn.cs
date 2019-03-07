// <copyright file="IItemBoughtFromMerchantPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface which is called when an item got sold to a merchant npc.
    /// </summary>
    [PlugInPoint("Item bought from merchant npc", "Is called when an item got bought from a merchant npc.")]
    [Guid("57AB1E67-5330-441C-8B9F-F9F3AF65E5BA")]
    public interface IItemBoughtFromMerchantPlugIn
    {
        /// <summary>
        /// Is called when an item got bought from a merchant npc.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="item">The item which got added to the players inventory.</param>
        /// <param name="sourceItem">The source item which was copied and is configured to be in the merchant npc.</param>
        /// <param name="merchant">The merchant npc.</param>
        void ItemBought(Player player, Item item, Item sourceItem, NonPlayerCharacter merchant);
    }
}