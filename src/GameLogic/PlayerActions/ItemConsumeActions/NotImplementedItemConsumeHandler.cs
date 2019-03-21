// -----------------------------------------------------------------------
// <copyright file="NotImplementedItemConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// A consume handler which is not implemented yet. It will show a message to the player.
    /// </summary>
    internal class NotImplementedItemConsumeHandler : IItemConsumeHandler
    {
        /// <inheritdoc/>
        public bool ConsumeItem(Player player, Item item, Item targetItem)
        {
            player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Using this item is not implemented yet.", MessageType.BlueNormal);
            return false;
        }
    }
}
