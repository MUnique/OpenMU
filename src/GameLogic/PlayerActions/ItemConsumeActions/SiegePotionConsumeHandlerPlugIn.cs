// -----------------------------------------------------------------------
// <copyright file="SiegePotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The alcohol consume handler.
/// </summary>
[Guid("9D50CE95-5354-43A7-8DD5-9D6953700DFA")]
[PlugIn(nameof(SiegePotionConsumeHandlerPlugIn), "Plugin which handles the siege potion consumption.")]
public class SiegePotionConsumeHandlerPlugIn : ApplyMagicEffectConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.SiegePotion;

    /// <inheritdoc/>
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (item.Level == 0
            && player.GameContext.Configuration.MagicEffects.FirstOrDefault(e => e.Number == 10) is { } blessEffectDefinition)
        {
            return await base.ConsumeItemAsyncCore(player, item, targetItem, fruitUsage, blessEffectDefinition).ConfigureAwait(false);
        }

        if (item.Level == 1
            && player.GameContext.Configuration.MagicEffects.FirstOrDefault(e => e.Number == 11) is { } effectDefinition)
        {
            if (await base.ConsumeItemAsyncCore(player, item, targetItem, fruitUsage, effectDefinition).ConfigureAwait(false))
            {
                await player.InvokeViewPlugInAsync<IConsumeSpecialItemPlugIn>(p => p.ConsumeSpecialItemAsync(item, (ushort)(effectDefinition.Duration?.ConstantValue.Value ?? 0))).ConfigureAwait(false);
                return true;
            }
        }
        else
        {
            await player.ShowMessageAsync("Effect for item not found.").ConfigureAwait(false);
        }

        return false;
    }
}