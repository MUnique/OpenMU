// <copyright file="SimpleCraftingSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.ItemCrafting
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// Crafting settings for the simple item crafting handler.
    /// </summary>
    public class SimpleCraftingSettings
    {
        /// <summary>
        /// Gets or sets the price to do the crafting.
        /// </summary>
        public int Money { get; set; }

        /// <summary>
        /// Gets or sets the success percent.
        /// </summary>
        public byte SuccessPercent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether multiple crafting at the same time are allowed for this crafting.
        /// </summary>
        public bool MultipleAllowed { get; set; }

        /// <summary>
        /// Gets or sets the required items.
        /// </summary>
        [MemberOfAggregate]
        public virtual IList<ItemCraftingRequiredItem> RequiredItems { get; protected set; }

        /// <summary>
        /// Gets or sets the result items, which are generated when the crafting succeeded.
        /// </summary>
        [MemberOfAggregate]
        public virtual IList<ItemCraftingResultItem> ResultItems { get; protected set; }

        /// <summary>
        /// Gets or sets the result item selection.
        /// </summary>
        public ResultItemSelection ResultItemSelect { get; set; }

        /// <summary>
        /// Gets or sets the luck option chance.
        /// </summary>
        public byte LuckOptionChance { get; set; }

        /// <summary>
        /// Gets or sets the skill option chance.
        /// </summary>
        public byte SkillOptionChance { get; set; }

        /// <summary>
        /// Gets or sets the excellent option chance.
        /// </summary>
        public byte ExcOptionChance { get; set; }

        /// <summary>
        /// Gets or sets the maximum excellent options.
        /// </summary>
        public byte MaxExcOptions { get; set; }
    }
}
