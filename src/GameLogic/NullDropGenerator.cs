// <copyright file="NullDropGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// A drop generator which generates nothing.
/// </summary>
public class NullDropGenerator : IDropGenerator
{
    private static IDropGenerator? _instance;

    /// <summary>
    /// Prevents a default instance of the <see cref="NullDropGenerator"/> class from being created.
    /// </summary>
    private NullDropGenerator()
    {
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    public static IDropGenerator Instance => _instance ??= new NullDropGenerator();

    /// <inheritdoc />
    public IEnumerable<Item> GenerateItemDrops(MonsterDefinition monster, int gainedExperience, Player player, out uint? droppedMoney)
    {
        droppedMoney = null;
        return Enumerable.Empty<Item>();
    }

    /// <inheritdoc />
    public Item? GenerateItemDrop(DropItemGroup group)
    {
        return null;
    }

    /// <inheritdoc />
    public Item? GenerateItemDrop(IEnumerable<DropItemGroup> group, out ItemDropEffect? dropEffect, out uint? droppedMoney)
    {
        droppedMoney = null;
        dropEffect = ItemDropEffect.Undefined;
        return null;
    }

    /// <inheritdoc />
    public Item? GenerateRandomItem(ICollection<ItemDefinition>? possibleItems)
    {
        return null;
    }
}