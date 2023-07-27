// <copyright file="IDropGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

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
    /// <returns>
    /// The item drops and money which are generated when a monster got killed by a player.
    /// </returns>
    ValueTask<(IEnumerable<Item> Items, uint? Money)> GenerateItemDropsAsync(MonsterDefinition monster, int gainedExperience, Player player);

    /// <summary>
    /// Generates an item based on a <see cref="DropItemGroup"/>.
    /// </summary>
    /// <param name="group">The <see cref="DropItemGroup"/> which defines which item should be generated.</param>
    /// <returns>The generated item or <see langword="null"/>.</returns>
    Item? GenerateItemDrop(DropItemGroup group);

    /// <summary>
    /// Generates an item based on a <see cref="DropItemGroup"/>s.
    /// </summary>
    /// <param name="groups">The <see cref="DropItemGroup"/>s which define which item should be generated.</param>
    /// <returns>The generated item, money and drop effect of the selected group.</returns>
    (Item? Item, uint? Money, ItemDropEffect DropEffect) GenerateItemDrop(IEnumerable<DropItemGroup> groups);
}