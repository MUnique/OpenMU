// <copyright file="ItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Message relevant extensions for items.
    /// </summary>
    public static class ItemExtensions
    {
        /// <summary>
        /// Gets the glow level of the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The glow level of the item.</returns>
        public static byte GetGlowLevel(this Item item) => GetGlowLevel(item.Level);

        /// <summary>
        /// Gets the glow level of the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The glow level of the item.</returns>
        public static byte GetGlowLevel(this ItemAppearance item) => GetGlowLevel(item.Level);

        private static byte GetGlowLevel(int itemLevel) => (byte)((itemLevel - 1) / 2);
    }
}
