// <copyright file="ItemCraftingRequiredItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.ItemCrafting
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration.Items;

    /// <summary>
    /// Describes an required item for a crafting.
    /// TODO: Some properties are not used yet.
    /// </summary>
    public class ItemCraftingRequiredItem
    {
        /// <summary>
        /// Gets or sets the item definition.
        /// </summary>
        public virtual ItemDefinition ItemDefinition { get; set; }

        /// <summary>
        /// Gets or sets the minimum level.
        /// </summary>
        public byte MinLvl { get; set; }

        /// <summary>
        /// Gets or sets the required item options.
        /// </summary>
        public virtual ICollection<ItemOptionType> RequiredItemOptions { get; protected set; }

        /// <summary>
        /// Gets or sets the minimum amount.
        /// </summary>
        public byte MinAmount { get; set; }

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
        /// Gets or sets the add percentage per division.
        /// </summary>
        /// <value>
        /// The add percentage.
        /// </value>
        public byte AddPercentage { get; set; }

        /// <summary>
        /// Gets or sets the reference identifier.
        /// </summary>
        public byte RefID { get; set; }
    }
}
