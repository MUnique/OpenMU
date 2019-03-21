﻿// -----------------------------------------------------------------------
// <copyright file="HealthPotionConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;

    /// <summary>
    /// The consume handler for a potion that recovers health.
    /// </summary>
    public abstract class HealthPotionConsumeHandler : RecoverConsumeHandler.ManaHealthConsumeHandler, IItemConsumeHandler
    {
        /// <inheritdoc/>
        protected override AttributeDefinition MaximumAttribute
        {
            get
            {
                return Stats.MaximumHealth;
            }
        }

        /// <inheritdoc/>
        protected override AttributeDefinition CurrentAttribute
        {
            get
            {
                return Stats.CurrentHealth;
            }
        }

        /// <inheritdoc/>
        public override bool ConsumeItem(Player player, Item item, Item targetItem)
        {
            if (base.ConsumeItem(player, item, targetItem))
            {
                // maybe instead of calling UpdateCurrentHealth etc. provide a more general method where we pass this.CurrentAttribute. The view can then decide what to do with it.
                player.ViewPlugIns.GetPlugIn<IUpdateCurrentHealthPlugIn>()?.UpdateCurrentHealth();
                return true;
            }

            return false;
        }
    }
}
