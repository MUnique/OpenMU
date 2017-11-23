// <copyright file="CraftingRequiredItemLink.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Defines which actual items of the <see cref="Player.TemporaryStorage"/> are fulfilling a specific item requirement.
    /// </summary>
    public class CraftingRequiredItemLink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CraftingRequiredItemLink"/> class.
        /// </summary>
        /// <param name="storedItem">The stored item.</param>
        /// <param name="requiredItem">The required item.</param>
        public CraftingRequiredItemLink(IEnumerable<Item> storedItem, ItemCraftingRequiredItem requiredItem)
        {
            this.StoredItem = storedItem;
            this.ItemRequirement = requiredItem;
        }

        /// <summary>
        /// Gets or sets the stored items of the <see cref="Player.TemporaryStorage"/>.
        /// </summary>
        public IEnumerable<Item> StoredItem { get; set; }

        /// <summary>
        /// Gets or sets the item requirement.
        /// </summary>
        public ItemCraftingRequiredItem ItemRequirement { get; set; }
    }
}
