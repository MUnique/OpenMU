// -----------------------------------------------------------------------
// <copyright file="ManaPotionConsumehandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Consume handler for potions which refills the players attribute <see cref="Stats.CurrentMana"/>.
    /// </summary>
    public abstract class ManaPotionConsumehandler : RecoverConsumeHandler.ManaHealthConsumeHandler, IItemConsumeHandler
    {
        /// <inheritdoc/>
        protected override AttributeDefinition MaximumAttribute
        {
            get
            {
                return Stats.MaximumMana;
            }
        }

        /// <inheritdoc/>
        protected override AttributeDefinition CurrentAttribute
        {
            get
            {
                return Stats.CurrentMana;
            }
        }

        /// <inheritdoc />
        public override bool ConsumeItem(Player player, byte itemSlot, byte targetSlot)
        {
            if (base.ConsumeItem(player, itemSlot, targetSlot))
            {
                player.PlayerView.UpdateCurrentMana();
                return true;
            }

            return false;
        }
    }
}
