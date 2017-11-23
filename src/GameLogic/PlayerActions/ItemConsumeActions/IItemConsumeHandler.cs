// -----------------------------------------------------------------------
// <copyright file="IItemConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    /// <summary>
    /// Consume handler to modify items which are specified by the target slot.
    /// </summary>
    public interface IItemConsumeHandler
    {
        /// <summary>
        /// Consumes the item at the specified slot, and reduces its durability by one.
        /// If the durability has reached 0, the item is getting destroyed.
        /// If a target slot is specified, the consumation targets the item on this slot (e.g. uprade of an item by a jewel).
        /// </summary>
        /// <param name="player">The player which is consuming.</param>
        /// <param name="itemSlot">The inventory slot of the item which should get consumed.</param>
        /// <param name="targetSlot">The inventory slot of the item which is affected.</param>
        /// <returns>The success of the consumation.</returns>
        bool ConsumeItem(Player player, byte itemSlot, byte targetSlot);
    }
}
