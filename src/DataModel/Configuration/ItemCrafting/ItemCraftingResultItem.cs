// <copyright file="ItemCraftingResultItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.ItemCrafting
{
    using MUnique.OpenMU.DataModel.Configuration.Items;

    /// <summary>
    /// Defines the resulting item of a crafting.
    /// </summary>
    public class ItemCraftingResultItem
    {
        /// <summary>
        /// Gets or sets the item definition.
        /// </summary>
        public virtual ItemDefinition ItemDefinition { get; set; }

        /// <summary>
        /// Gets or sets the random minimum level for a created item.
        /// </summary>
        public byte RandomMinimumLevel { get; set; }

        /// <summary>
        /// Gets or sets the random maximum level for a created item.
        /// </summary>
        public byte RandomMaximumLevel { get; set; }

        /// <summary>
        /// Gets or sets the durability for a created item explicitly.
        /// </summary>
        public byte? Durability { get; set; }

        /// <summary>
        /// Gets or sets the reference to the corresponding <see cref="ItemCraftingRequiredItem.Reference"/>.
        /// If <c>0</c>, no reference exists.
        /// </summary>
        /// <remarks>
        /// For Item Upping.
        /// </remarks>
        public byte Reference { get; set; }

        /// <summary>
        /// Gets or sets the add level.
        /// </summary>
        /// <remarks>
        /// For Item Upping.
        /// </remarks>
        public byte AddLevel { get; set; }
    }
}
