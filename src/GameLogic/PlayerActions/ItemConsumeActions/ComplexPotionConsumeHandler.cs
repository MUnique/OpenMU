// <copyright file="ComplexPotionConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using System;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Abstract consume handler for complex potions which combines a <see cref="HealthPotionConsumeHandler"/> and a <see cref="ShieldPotionConsumeHandler"/>.
    /// </summary>
    public class ComplexPotionConsumeHandler : BaseConsumeHandler
    {
        private readonly HealthPotionConsumeHandler healthPotionConsumeHandler;
        private readonly ShieldPotionConsumeHandler shieldPotionConsumeHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexPotionConsumeHandler"/> class.
        /// </summary>
        /// <param name="healthPotionConsumeHandler">The health potion consume handler.</param>
        /// <param name="shieldPotionConsumeHandler">The shield potion consume handler.</param>
        protected ComplexPotionConsumeHandler(HealthPotionConsumeHandler healthPotionConsumeHandler, ShieldPotionConsumeHandler shieldPotionConsumeHandler)
        {
            this.healthPotionConsumeHandler = healthPotionConsumeHandler ?? throw new ArgumentNullException(nameof(healthPotionConsumeHandler));
            this.shieldPotionConsumeHandler = shieldPotionConsumeHandler ?? throw new ArgumentNullException(nameof(shieldPotionConsumeHandler));
        }

        /// <inheritdoc />
        public override bool ConsumeItem(Player player, Item item, Item targetItem, FruitUsage fruitUsage)
        {
            if (base.ConsumeItem(player, item, targetItem, fruitUsage))
            {
                this.healthPotionConsumeHandler.Recover(player);
                this.shieldPotionConsumeHandler.Recover(player);
                return true;
            }

            return false;
        }
    }
}