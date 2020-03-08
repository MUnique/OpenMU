// <copyright file="ItemOfItemSet.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items
{
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// Defines additional bonus options for this item of a set.
    /// </summary>
    /// <remarks>
    /// Here we can define additional bonus options, like the ancient options (e.g. +5 / +10 Str etc.).
    /// </remarks>
    public class ItemOfItemSet
    {
        /// <summary>
        /// Gets or sets the item's definition for which the bonus should apply.
        /// </summary>
        public virtual ItemDefinition ItemDefinition { get; set; }

        /// <summary>
        /// Gets or sets the bonus option.
        /// </summary>
        [MemberOfAggregate]
        public virtual IncreasableItemOption BonusOption { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.BonusOption?.PowerUpDefinition?.ToString() ?? string.Empty;
        }
    }
}