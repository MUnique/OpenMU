// -----------------------------------------------------------------------
// <copyright file="AlcoholConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// The alcohol consume handler.
/// </summary>
public class AlcoholConsumeHandler : BaseConsumeHandler
{
    /// <inheritdoc/>
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (await base.ConsumeItemAsync(player, item, targetItem, fruitUsage).ConfigureAwait(false))
        {
            await player.InvokeViewPlugInAsync<IDrinkAlcoholPlugIn>(p => p.DrinkAlcoholAsync()).ConfigureAwait(false);
            return true;
        }

        return false;
    }
}