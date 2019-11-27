// <copyright file="ItemPriceResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.PlayerShop
{
    using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;

    /// <summary>
    /// Result of the <see cref="BuyRequestAction"/>.
    /// </summary>
    public enum ItemPriceResult
    {
        /// <summary>
        /// Failed, e.g. because the shop feature is deactivated.
        /// </summary>
        Failed,

        /// <summary>
        /// The price has been set successfully.
        /// </summary>
        Success,

        /// <summary>
        /// Failed because the item slot was out of range.
        /// </summary>
        ItemSlotOutOfRange,

        /// <summary>
        /// Failed because the item could not be found.
        /// </summary>
        ItemNotFound,

        /// <summary>
        /// Failed because the price was negative.
        /// </summary>
        PriceNegative,

        /// <summary>
        /// Failed because the item is blocked.
        /// </summary>
        ItemIsBlocked,

        /// <summary>
        /// Failed because the character level is too low (below level 6).
        /// </summary>
        CharacterLevelTooLow,
    }
}