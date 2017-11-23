// -----------------------------------------------------------------------
// <copyright file="ItemModifyConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Consume handler to modify items which are specified by the target slot.
    /// </summary>
    public abstract class ItemModifyConsumeHandler : IItemConsumeHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemModifyConsumeHandler"/> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        protected ItemModifyConsumeHandler(IRepositoryManager repositoryManager)
        {
            this.RepositoryManager = repositoryManager;
        }

        /// <summary>
        /// Gets the repository manager.
        /// </summary>
        protected IRepositoryManager RepositoryManager { get; }

        /// <inheritdoc/>
        public bool ConsumeItem(Player player, byte itemSlot, byte targetSlot)
        {
            if (player.PlayerState.CurrentState != PlayerState.EnteredWorld)
            {
                return false;
            }

            Item item = player.Inventory.GetItem(targetSlot);
            if (item == null)
            {
                return false;
            }

            if (!this.ModifyItem(item))
            {
                return false;
            }

            player.PlayerView.InventoryView.ItemUpgraded(item);
            return true;
        }

        /// <summary>
        /// Modifies the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Flag indicating whether the modification of the item occured.</returns>
        protected abstract bool ModifyItem(Item item);
    }
}
