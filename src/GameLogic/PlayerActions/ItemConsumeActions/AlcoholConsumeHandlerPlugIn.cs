// -----------------------------------------------------------------------
// <copyright file="AlcoholConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The alcohol consume handler.
/// </summary>
[Guid("7FC2FE02-9215-4AD3-958F-D2279CD84266")]
[PlugIn(nameof(AlcoholConsumeHandlerPlugIn), "Plugin which handles the alcohol consumption.")]
public class AlcoholConsumeHandlerPlugIn : ApplyMagicEffectConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.Alcohol;

    /// <inheritdoc/>
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (await base.ConsumeItemAsync(player, item, targetItem, fruitUsage).ConfigureAwait(false))
        {
            var effectDefinition = item.Definition?.ConsumeEffect;
            await player.InvokeViewPlugInAsync<IConsumeSpecialItemPlugIn>(p => p.ConsumeSpecialItemAsync(item, (ushort)(effectDefinition?.Duration?.ConstantValue.Value ?? 0))).ConfigureAwait(false);
            return true;
        }

        return false;
    }
}