// -----------------------------------------------------------------------
// <copyright file="AlcoholConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    /// <summary>
    /// The alcohol consume handler.
    /// </summary>
    public class AlcoholConsumeHandler : BaseConsumeHandler
    {
        /// <inheritdoc/>
        public override bool ConsumeItem(Player player, byte itemSlot, byte targetSlot)
        {
            if (base.ConsumeItem(player, itemSlot, targetSlot))
            {
                player.PlayerView.DrinkAlcohol();
                return true;
            }

            return false;
        }
    }
}
