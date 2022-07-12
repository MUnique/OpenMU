// -----------------------------------------------------------------------
// <copyright file="NotImplementedItemConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// A consume handler which is not implemented yet. It will show a message to the player.
/// </summary>
internal class NotImplementedItemConsumeHandler : IItemConsumeHandler
{
    /// <inheritdoc/>
    public async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Using this item is not implemented yet.", MessageType.BlueNormal)).ConfigureAwait(false);
        return false;
    }
}