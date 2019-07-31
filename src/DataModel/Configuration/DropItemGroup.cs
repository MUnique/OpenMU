// <copyright file="DropItemGroup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration.Items;

    /// <summary>
    /// Enumeration of special item types.
    /// </summary>
    public enum SpecialItemType
    {
        /// <summary>
        /// No special item type.
        /// </summary>
        None,

        /// <summary>
        /// The ancient special item type.
        /// </summary>
        Ancient,

        /// <summary>
        /// The excellent special item type.
        /// </summary>
        Excellent,

        /// <summary>
        /// The random item special item type.
        /// </summary>
        RandomItem,

        /// <summary>
        /// The socket item special item type.
        /// </summary>
        SocketItem,

        /// <summary>
        /// The money special item type.
        /// </summary>
        Money,
    }

    /// <summary>
    /// Idea: append several "drop item groups" with its certain probability.
    /// In the drop generator sort all DropItemGroups by its chance.
    /// Classes which can have DropItemGroups: Maps, Monsters(for example the kundun drops), Players(for quest items).
    /// </summary>
    public class DropItemGroup
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the chance of the item drop group to apply. From 0.0 to 1.0.
        /// </summary>
        public double Chance { get; set; }

        /// <summary>
        /// Gets or sets the special type of the item.
        /// </summary>
        public SpecialItemType ItemType { get; set; }

        /// <summary>
        /// Gets or sets the possible items which can be dropped.
        /// </summary>
        public virtual ICollection<ItemDefinition> PossibleItems { get; protected set; }
    }
}
