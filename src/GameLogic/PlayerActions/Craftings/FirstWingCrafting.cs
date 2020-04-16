// <copyright file="FirstWingCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings
{
    using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;

    /// <summary>
    /// A crafting to create a first wing for elf, dark wizard or dark knight.
    /// </summary>
    public class FirstWingCrafting : SimpleItemCraftingHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirstWingCrafting"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public FirstWingCrafting(SimpleCraftingSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets the chaos weapon reference which must be specified in the <see cref="ItemCraftingRequiredItem.Reference"/>.
        /// </summary>
        public static byte ChaosWeaponReference { get; } = 0xAA;

        /// <inheritdoc />
        protected override bool RequiredItemMatches(Item item, ItemCraftingRequiredItem requiredItem)
        {
            if (requiredItem.Reference == ChaosWeaponReference)
            {
                return base.RequiredItemMatches(item, requiredItem)
                       && (this.IsChaosNatureBow(item)
                           || this.IsChaosDragonAxe(item)
                           || this.IsChaosLightningStaff(item));
            }

            return base.RequiredItemMatches(item, requiredItem);
        }

        private bool IsChaosNatureBow(Item item) => item.Definition.Group == 4 && item.Definition.Number == 6;

        private bool IsChaosDragonAxe(Item item) => item.Definition.Group == 2 && item.Definition.Number == 6;

        private bool IsChaosLightningStaff(Item item) => item.Definition.Group == 4 && item.Definition.Number == 7;
    }
}