// <copyright file="IItemPowerUpFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Generic;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// A Factory for power ups which are provided by equipped items.
    /// Each power up has to be created indiviually for a specific player, because some depend on the attributes of the player.
    /// </summary>
    public interface IItemPowerUpFactory
    {
        /// <summary>
        /// Gets the power ups of an individual item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="attributeSystem">The attribute system of the player who equipped the item.</param>
        /// <returns>The created powerups.</returns>
        IEnumerable<PowerUpWrapper> GetPowerUps(Item item, AttributeSystem attributeSystem);

        /// <summary>
        /// Gets the set power ups, which are created for existing <see cref="ItemSetGroup"/>s in the equipped items.
        /// </summary>
        /// <param name="equippedItems">The equipped items.</param>
        /// <param name="attributeSystem">The attribute system of the player who equipped the items.</param>
        /// <returns>The created set power ups.</returns>
        IEnumerable<PowerUpWrapper> GetSetPowerUps(IEnumerable<Item> equippedItems, AttributeSystem attributeSystem);
    }
}
