// <copyright file="ItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Extensions for <see cref="Item"/>.
/// </summary>
public static class ItemExtensions
{
    /// <summary>
    /// Determines whether this instance is a wing.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is wing; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsWing(this Item item)
    {
        return item.Definition?.IsWing() ?? false;
    }

    /// <summary>
    /// Determines whether this instance is a wing.
    /// </summary>
    /// <param name="itemDefinition">The item definition.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is wing; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsWing(this ItemDefinition itemDefinition)
    {
        return itemDefinition?.Group is 12 or 13
               && itemDefinition is { Width: >= 2, Height: >= 2, MaximumItemLevel: >= 11, Durability: >= 200 }
               && (itemDefinition.ItemSlot?.ItemSlots.Any() ?? false);
    }

    /// <summary>
    /// Determines whether this item is a pet (Dark Raven, Dark Horse) which can gain levels itself.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><see langword="true"/>, if the item is a pet.</returns>
    public static bool IsTrainablePet(this Item item)
    {
        return item.Definition?.IsTrainablePet() ?? false;
    }

    /// <summary>
    /// Determines whether this item is a pet (Dark Raven, Dark Horse) which can gain levels itself.
    /// </summary>
    /// <param name="itemDefinition">The item definition.</param>
    /// <returns><see langword="true"/>, if the item is a pet.</returns>
    public static bool IsTrainablePet(this ItemDefinition itemDefinition)
    {
        return itemDefinition.PetExperienceFormula is not null;
    }

    /// <summary>
    /// Determines whether this item can have a skill.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item can have a skill; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanHaveSkill(this Item item) => item.IsWearable() && item.Definition?.Skill != null && item.Definition.QualifiedCharacters.Any();

    /// <summary>
    /// Determines whether this item is wearable.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is wearable; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsWearable(this Item item) => item.Definition?.ItemSlot != null;

    /// <summary>
    /// Determines whether this item is stackable.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is stackable; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsStackable(this Item item) => !item.IsWearable() && item.Definition?.Durability > 1;

    /// <summary>
    /// Determines whether this item can be completely stacked on the specified other item.
    /// After stacking, this item is destroyed.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="otherItem">The other item.</param>
    /// <returns>
    ///   <c>true</c> if this item can be completely stacked on the specified other item; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanCompletelyStackOn(this Item item, Item otherItem) => item.IsStackable() && item.IsSameItemAs(otherItem) && (item.Durability + otherItem.Durability) <= item.Definition?.Durability;

    /// <summary>
    /// Determines whether this item can be partially stacked on the specified other item.
    /// After stacking, this item is left with the rest of its durability.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="otherItem">The other item.</param>
    /// <returns>
    ///   <c>true</c> if this item can be partially stacked on the specified other item; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanPartiallyStackOn(this Item item, Item otherItem) => item.IsStackable() && item.IsSameItemAs(otherItem) && otherItem.Durability < otherItem.Definition?.Durability;

    /// <summary>
    /// Determines whether this item is of the same type as the specified other item.
    /// <see cref="Item.Definition"/> and <see cref="Item.Level"/> need to be equal to get considered as the same item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="otherItem">The other item.</param>
    /// <returns>
    ///   <c>true</c> if this item is of the same type as the specified other item; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsSameItemAs(this Item item, Item otherItem) => item.Definition == otherItem.Definition && item.Level == otherItem.Level;

    /// <summary>
    /// Determines whether the item level can be upgraded by using jewels of bless or soul.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the item level can be upgraded by using jewels of bless or soul; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanLevelBeUpgraded(this Item item)
    {
        return item.IsWearable()
               && item.Definition!.ItemSlot!.ItemSlots.Any(slot => slot <= InventoryConstants.WingsSlot)
               && item.Level < item.Definition.MaximumItemLevel;
    }

    /// <summary>
    /// Gets the <see cref="Item.Durability"/> as byte value, rounded off.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The durability.</returns>
    public static byte Durability(this Item item) => (byte)Math.Floor(item.Durability);

    /// <summary>
    /// Decreases the durability of an item, and returns <see langword="true"/>, if the integral number changed.
    /// </summary>
    /// <param name="item">The item whose durability should be decreased.</param>
    /// <param name="decrement">The decrement value.</param>
    /// <returns><see langword="true"/>, if the integral number changed.</returns>
    public static bool DecreaseDurability(this Item item, double decrement)
    {
        var previous = item.Durability;
        item.Durability = Math.Max(previous - decrement, 0.0);
        return (byte)Math.Floor(item.Durability) != (byte)Math.Floor(previous);
    }
}