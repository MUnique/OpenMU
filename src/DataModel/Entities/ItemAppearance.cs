// <copyright file="ItemAppearance.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities
{
    using MUnique.OpenMU.DataModel.Configuration.Items;

    /// <summary>
    /// Appearance of an item.
    /// </summary>
    public class ItemAppearance
    {
        /// <summary>
        /// Gets or sets the item slot.
        /// </summary>
        public byte ItemSlot { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public ushort Index { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        public byte Group { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        public byte Level { get; set; }

        /// <summary>
        /// Gets or sets the visible options.
        /// </summary>
        public virtual ItemOptionType[] VisibleOptions { get; set; }
    }
}
