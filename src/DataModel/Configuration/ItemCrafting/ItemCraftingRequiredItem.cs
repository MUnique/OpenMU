// <copyright file="ItemCraftingRequiredItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.ItemCrafting
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration.Items;

    /// <summary>
    /// Describes an required item for a crafting.
    /// </summary>
    public class ItemCraftingRequiredItem
    {
        /// <summary>
        /// Gets the collection of possible items which are valid for this requirement.
        /// </summary>
        public virtual ICollection<ItemDefinition> PossibleItems { get; protected set; }

        /// <summary>
        /// Gets or sets the minimum item level.
        /// </summary>
        public byte MinimumItemLevel { get; set; }

        /// <summary>
        /// Gets or sets the maximum item level.
        /// </summary>
        public byte MaximumItemLevel { get; set; }

        /// <summary>
        /// Gets or sets the required item options.
        /// </summary>
        public virtual ICollection<ItemOptionType> RequiredItemOptions { get; protected set; }

        /// <summary>
        /// Gets or sets the minimum amount.
        /// </summary>
        public byte MinimumAmount { get; set; }

        /// <summary>
        /// Gets or sets the maximum amount.
        /// </summary>
        public byte MaximumAmount { get; set; }

        /// <summary>
        /// Gets or sets the success result.
        /// </summary>
        public MixResult SuccessResult { get; set; }

        /// <summary>
        /// Gets or sets the fail result.
        /// </summary>
        public MixResult FailResult { get; set; }

        /// <summary>
        /// Gets or sets the NPC price divisor. For each full division, the percentage gets increased by 1 percent, and the mix price rises.
        /// </summary>
        public int NpcPriceDivisor { get; set; }

        /// <summary>
        /// Gets or sets the add percentage per item.
        /// </summary>
        public byte AddPercentage { get; set; }

        /// <summary>
        /// Gets or sets the reference identifier to the corresponding <see cref="ItemCraftingResultItem.Reference"/>.
        /// If <c>0</c>, no reference exists.
        /// </summary>
        public byte Reference { get; set; }
    }
}
