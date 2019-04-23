// <copyright file="ItemGroups.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using MUnique.OpenMU.DataModel.Configuration.Items;

    /// <summary>
    /// Enumeration of <see cref="ItemDefinition.Group"/> values.
    /// </summary>
    internal enum ItemGroups
    {
        /// <summary>
        /// The swords group.
        /// </summary>
        Swords = 0,

        /// <summary>
        /// The axes group.
        /// </summary>
        Axes = 1,

        /// <summary>
        /// The scepters group.
        /// </summary>
        Scepters = 2,

        /// <summary>
        /// The spears group.
        /// </summary>
        Spears = 3,

        /// <summary>
        /// The bows group.
        /// </summary>
        Bows = 4,

        /// <summary>
        /// The staff group.
        /// </summary>
        Staff = 5,

        /// <summary>
        /// The shields group.
        /// </summary>
        Shields = 6,

        /// <summary>
        /// The helm group.
        /// </summary>
        Helm = 7,

        /// <summary>
        /// The armor group.
        /// </summary>
        Armor = 8,

        /// <summary>
        /// The pants group.
        /// </summary>
        Pants = 9,

        /// <summary>
        /// The gloves group.
        /// </summary>
        Gloves = 10,

        /// <summary>
        /// The boots group.
        /// </summary>
        Boots = 11,

        /// <summary>
        /// The orbs group.
        /// </summary>
        Orbs = 12,

        /// <summary>
        /// The scrolls group.
        /// </summary>
        Scrolls = 15,

        /// <summary>
        /// The weapon group, which is used to identify weapon guardian options, but not a valid value at <see cref="ItemDefinition.Group"/>.
        /// </summary>
        Weapon = 0xF0,
    }
}
