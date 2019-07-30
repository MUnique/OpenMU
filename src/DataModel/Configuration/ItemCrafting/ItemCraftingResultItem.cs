// <copyright file="ItemCraftingResultItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.ItemCrafting
{
    using MUnique.OpenMU.DataModel.Configuration.Items;

    /// <summary>
    /// Defines the resulting item of a crafting.
    /// TODO: Some properties are not used yet.
    /// </summary>
    public class ItemCraftingResultItem
    {
        /// <summary>
        /// Gets or sets the item definition.
        /// </summary>
        public virtual ItemDefinition ItemDefinition { get; set; }

        /// <summary>
        /// Gets or sets the random minimum level.
        /// </summary>
        public byte RandLvlMin { get; set; }

        /// <summary>
        /// Gets or sets the random maximum level.
        /// </summary>
        public byte RandLvlMax { get; set; }

        // public byte Level { get; set; }

        /// <summary>
        /// Gets or sets the reference identifier.
        /// </summary>
        /// <remarks>
        /// For Item Upping.
        /// </remarks>
        public byte RefID { get; set; }

        /// <summary>
        /// Gets or sets the add level.
        /// </summary>
        /// <remarks>
        /// For Item Upping.
        /// </remarks>
        public byte AddLevel { get; set; }
    }
}
