// <copyright file="IItemSoldToMerchantPlugIn.cs" company="MUnique">
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
    [PlugInPoint("Item sold to merchant npc", "Is called when an item got sold to a merchant npc.")]
    [Guid("42107B17-0C86-4163-A80F-88CEE929CE3F")]
    public interface IItemSoldToMerchantPlugIn
    {
        /// <summary>
        /// Is called when an item got sold to a merchant npc.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="item">The sold item.</param>
        /// <param name="merchant">The merchant npc.</param>
        void ItemSold(Player player, Item item, NonPlayerCharacter merchant);
    }
}