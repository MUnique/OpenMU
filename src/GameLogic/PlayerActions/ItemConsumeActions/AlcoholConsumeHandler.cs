// -----------------------------------------------------------------------
// <copyright file="AlcoholConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// The alcohol consume handler.
    /// </summary>
    public class AlcoholConsumeHandler : BaseConsumeHandler
    {
        /// <inheritdoc/>
        public override bool ConsumeItem(Player player, Item item, Item targetItem)
        {
            if (base.ConsumeItem(player, item, targetItem))
            {
                player.PlayerView.DrinkAlcohol();
                return true;
            }

            return false;
        }
    }
}
