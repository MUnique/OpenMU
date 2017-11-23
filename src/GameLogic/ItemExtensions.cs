// <copyright file="ItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;

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
            if (item.Definition.ItemSlot == null)
            {
                // Items which are not wearable don't have a "real" durability. If the item is stackable, durability means number of pieces in this case
                return 1; // TODO: check if this makes sense for all cases
            }

            var result = item.Definition.Durability + AdditionalDurabilityPerLevel[item.Level];
            if (item.ItemOptions.Any(link => link.ItemOption.OptionType == ItemOptionTypes.AncientOption))
            {
                result += 20;
            }
            else if (item.ItemOptions.Any(link => link.ItemOption.OptionType == ItemOptionTypes.Excellent))
            {
                // TODO: Exclude Wings and archangel weapons, but I guess it's not a big issue if we don't, because of their already high durability
                result += 15;
            }

            return (byte)Math.Min(byte.MaxValue, result);
        }
    }
}
