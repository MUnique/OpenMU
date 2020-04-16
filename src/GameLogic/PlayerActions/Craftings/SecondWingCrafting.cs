// <copyright file="SecondWingCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings
{
    using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;

    /// <summary>
    /// A crafting to create a Cape of Lord/Fighter and second level wings.
    /// </summary>
    public class SecondWingCrafting : SimpleItemCraftingHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecondWingCrafting"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public SecondWingCrafting(SimpleCraftingSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets the first wing reference which must be specified in the <see cref="ItemCraftingRequiredItem.Reference"/>.
        /// </summary>
        public static byte FirstWingReference { get; } = 0xBB;

        /// <inheritdoc />
        protected override bool RequiredItemMatches(Item item, ItemCraftingRequiredItem requiredItem)
        {
            if (requiredItem.Reference == FirstWingReference)
            {
                return base.RequiredItemMatches(item, requiredItem)
                       && (this.IsWingsOfFairy(item)
                           || this.IsWingsOfHeaven(item)
                           || this.IsWingsOfSatan(item)
                           || this.IsWingsOfMisery(item));
            }

            return base.RequiredItemMatches(item, requiredItem);
        }

        private bool IsWingsOfFairy(Item item) => item.Definition.Group == 12 && item.Definition.Number == 0;

        private bool IsWingsOfHeaven(Item item) => item.Definition.Group == 12 && item.Definition.Number == 1;

        private bool IsWingsOfSatan(Item item) => item.Definition.Group == 12 && item.Definition.Number == 2;

        private bool IsWingsOfMisery(Item item) => item.Definition.Group == 12 && item.Definition.Number == 41;
    }
}