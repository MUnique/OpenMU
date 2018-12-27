// -----------------------------------------------------------------------
// <copyright file="BaseConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    /// <summary>
    /// Base class of an item consumption handler.
    /// </summary>
    public class BaseConsumeHandler : IItemConsumeHandler
    {
        /// <inheritdoc/>
        public virtual bool ConsumeItem(Player player, byte itemSlot, byte targetSlot)
        {
            if (player.PlayerState.CurrentState != PlayerState.EnteredWorld)
            {
                return false;
            }

            var item = player.Inventory.GetItem(itemSlot);
            if (item != null)
            {
                if (item.Durability > 0)
                {
                    item.Durability -= 1;
                }

                if (item.Durability == 0)
                {
                    player.Inventory.RemoveItem(item);
                    player.PersistenceContext.Delete(item);
                }

                return true;
            }

            return false;
        }
    }
}
