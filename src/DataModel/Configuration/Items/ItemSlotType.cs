// <copyright file="ItemSlotType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items
{
    using System.Collections.Generic;

    /// <summary>
    /// The item slot type. Each of this may have one or more actual item slots.
    /// </summary>
    public class ItemSlotType
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the item slots of this slot type.
        /// </summary>
        public virtual ICollection<int> ItemSlots { get; protected set; }
    }
}
