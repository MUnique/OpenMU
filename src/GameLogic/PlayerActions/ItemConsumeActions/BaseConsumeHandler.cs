// -----------------------------------------------------------------------
// <copyright file="BaseConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns;

    /// <summary>
    /// Base class of an item consumption handler.
    /// </summary>
    public class BaseConsumeHandler : IItemConsumeHandler
    {
        /// <inheritdoc/>
        public virtual bool ConsumeItem(Player player, Item item, Item targetItem)
        {
            if (player.PlayerState.CurrentState != PlayerState.EnteredWorld)
            {
                return false;
            }

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
