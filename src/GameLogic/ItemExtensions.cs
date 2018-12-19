// <copyright file="ItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Extension methods for <see cref="Item"/>.
    /// </summary>
    public static class ItemExtensions
    {
        private static readonly byte[] AdditionalDurabilityPerLevel = { 0, 1, 2, 3, 4, 6, 8, 10, 12, 14, 17, 21, 26, 32, 39, 47 };

        /// <summary>
        /// Gets the maximum durability of the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The maximum durability of the item.</returns>
        /// <remarks>
        /// I think this is more like the durability which can be dropped.
        /// Some items can be stacked up to 255 pieces, which increases the durability value.
        /// </remarks>
        public static byte GetMaximumDurabilityOfOnePiece(this Item item)
        {
            if (!item.IsWearable())
            {
                // Items which are not wearable don't have a "real" durability. If the item is stackable, durability means number of pieces in this case
                return 1;
            }

            var result = item.Definition.Durability + AdditionalDurabilityPerLevel[item.Level];
            if (item.ItemOptions.Any(link => link.ItemOption.OptionType == ItemOptionTypes.AncientOption))
            {
                result += 20;
            }
            else if (item.ItemOptions.Any(link => link.ItemOption.OptionType == ItemOptionTypes.Excellent))
            {
                // TODO: archangel weapons, but I guess it's not a big issue if we don't, because of their already high durability
                result += 15;
            }
            else
            {
                // there are no other options which increase the durability.
                // It might be nice to add the magic values above to the ItemOptionType, as data.
            }

            return (byte)Math.Min(byte.MaxValue, result);
        }

        /// <summary>
        /// Determines whether this item is wearable.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the specified item is wearable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWearable(this Item item) => item.Definition.ItemSlot != null;

        /// <summary>
        /// Determines whether this item is stackable.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the specified item is stackable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsStackable(this Item item) => !item.IsWearable() && item.Definition.Durability > 1;

        /// <summary>
        /// Determines whether this item can be completely stacked on the specified other item.
        /// After stacking, this item is destroyed.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="otherItem">The other item.</param>
        /// <returns>
        ///   <c>true</c> if this item can be completely stacked on the specified other item; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanCompletelyStackOn(this Item item, Item otherItem) => item.IsStackable() && item.IsSameItemAs(otherItem) && (item.Durability + otherItem.Durability) <= item.Definition.Durability;

        /// <summary>
        /// Determines whether this item can be partially stacked on the specified other item.
        /// After stacking, this item is left with the rest of its durability.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="otherItem">The other item.</param>
        /// <returns>
        ///   <c>true</c> if this item can be partially stacked on the specified other item; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanPartiallyStackOn(this Item item, Item otherItem) => item.IsStackable() && item.IsSameItemAs(otherItem) && otherItem.Durability < otherItem.Definition.Durability;

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
        /// Gets the item data which is relvant for the visual appearance of an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The item data which is relvant for the visual appearance of an item.</returns>
        public static ItemAppearance GetAppearance(this Item item)
        {
            var appearance = new TemporaryItemAppearance
            {
                Definition = item.Definition,
                ItemSlot = item.ItemSlot,
                Level = item.Level,
            };
            item.ItemOptions
                .Where(option => option.ItemOption.OptionType?.IsVisible ?? false)
                .Select(option => option.ItemOption.OptionType)
                .ForEach(appearance.VisibleOptions.Add);
            return appearance;
        }

        /// <summary>
        /// Creates a persistent instance of the given <see cref="ItemAppearance"/> and returns it.
        /// </summary>
        /// <param name="itemAppearance">The item appearance.</param>
        /// <param name="persistenceContext">The persistence context where the object should be added.</param>
        /// <returns>A persistent instance of the given <see cref="ItemAppearance"/>.</returns>
        public static ItemAppearance MakePersistent(this ItemAppearance itemAppearance, IContext persistenceContext)
        {
            var persistent = persistenceContext.CreateNew<ItemAppearance>();
            persistent.ItemSlot = itemAppearance.ItemSlot;
            persistent.Definition = itemAppearance.Definition;
            persistent.Level = itemAppearance.Level;
            itemAppearance.VisibleOptions.ForEach(o => persistent.VisibleOptions.Add(o));
            return persistent;
        }

        private sealed class TemporaryItemAppearance : ItemAppearance
        {
            public override ICollection<ItemOptionType> VisibleOptions => base.VisibleOptions ?? (base.VisibleOptions = new List<ItemOptionType>());
        }
    }
}
