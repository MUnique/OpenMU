// <copyright file="ThirdWingCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings
{
    using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;

    /// <summary>
    /// Crafting for the first step of third wings.
    /// </summary>
    public class ThirdWingCrafting : SimpleItemCraftingHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThirdWingCrafting"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public ThirdWingCrafting(SimpleCraftingSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets the second wing reference which must be specified in the <see cref="ItemCraftingRequiredItem.Reference"/>.
        /// </summary>
        public static byte SecondWingReference { get; } = 0xCC;

        /// <inheritdoc />
        protected override bool RequiredItemMatches(Item item, ItemCraftingRequiredItem requiredItem)
        {
            if (requiredItem.Reference == SecondWingReference)
            {
                return base.RequiredItemMatches(item, requiredItem)
                       && (this.IsWingsOfSpirits(item)
                           || this.IsWingsOfSoul(item)
                           || this.IsWingsOfDragon(item)
                           || this.IsWingsOfDarkness(item)
                           || this.IsWingsOfDespair(item));
            }

            return base.RequiredItemMatches(item, requiredItem);
        }

        private bool IsWingsOfDespair(Item item) => item.Definition.Group == 12 && item.Definition.Number == 42;

        private bool IsWingsOfSpirits(Item item) => item.Definition.Group == 12 && item.Definition.Number == 3;

        private bool IsWingsOfSoul(Item item) => item.Definition.Group == 12 && item.Definition.Number == 4;

        private bool IsWingsOfDragon(Item item) => item.Definition.Group == 12 && item.Definition.Number == 5;

        private bool IsWingsOfDarkness(Item item) => item.Definition.Group == 12 && item.Definition.Number == 6;
    }
}