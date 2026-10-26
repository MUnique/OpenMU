// <copyright file="MerchantStoreExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Extension methods for the <see cref="ItemStorage"/> of a merchant store.
/// </summary>
public static class MerchantStoreExtensions
{
    /// <summary>
    /// Gets the items of a merchant store which can be offered to the player.
    /// Items whose <see cref="ItemDefinition.IsActive"/> is <c>false</c> are left out.
    /// </summary>
    /// <param name="merchantStore">The merchant store.</param>
    /// <returns>The items which can be offered to the player.</returns>
    public static ICollection<Item> GetOfferedItems(this ItemStorage merchantStore)
    {
        if (merchantStore.Items.All(item => item.Definition?.IsActive is not false))
        {
            return merchantStore.Items;
        }

        return merchantStore.Items.Where(item => item.Definition?.IsActive is not false).ToList();
    }
}
