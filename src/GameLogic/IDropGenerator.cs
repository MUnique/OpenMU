// <copyright file="IDropGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// The interface for a drop generator.
    /// </summary>
    public interface IDropGenerator
    {
        /// <summary>
        /// Gets the item drops which are generated when a monster got killed by a player.
        /// </summary>
        /// <param name="monster">The monster which got killed.</param>
        /// <param name="gainedExperience">The experience which the player gained form the kill (relevant for the money drop).</param>
        /// <param name="player">The player who killed the monster.</param>
        /// <param name="droppedMoney">The dropped money, if available.</param>
        /// <returns>
        /// The item drops which are generated when a monster got killed by a player.
        /// </returns>
        IEnumerable<Item> GenerateItemDrops(MonsterDefinition monster, int gainedExperience, Player player, out uint? droppedMoney);
    }
}
