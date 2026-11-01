// -----------------------------------------------------------------------
// <copyright file="SiegePotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The consume handler for the siege potions, the Potion of Bless and the Potion of Soul.
/// </summary>
[Guid("9D50CE95-5354-43A7-8DD5-9D6953700DFA")]
[PlugIn]
[Display(Name = nameof(PlugInResources.SiegePotionConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.SiegePotionConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class SiegePotionConsumeHandlerPlugIn : ApplyMagicEffectConsumeHandlerPlugIn, ISupportCustomConfiguration<SiegePotionConsumeHandlerConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public SiegePotionConsumeHandlerConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.SiegePotion;

    /// <inheritdoc/>
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        var configuration = this.Configuration ??= new SiegePotionConsumeHandlerConfiguration();
        if (item.Level == 0
            && player.GameContext.Configuration.MagicEffects.FirstOrDefault(e => e.Number == configuration.BlessEffectNumber) is { } blessEffectDefinition)
        {
            return await this.ConsumeItemCoreAsync(player, item, targetItem, fruitUsage, blessEffectDefinition).ConfigureAwait(false);
        }

        if (item.Level == 1
            && player.GameContext.Configuration.MagicEffects.FirstOrDefault(e => e.Number == configuration.SoulEffectNumber) is { } effectDefinition)
        {
            if (await this.ConsumeItemCoreAsync(player, item, targetItem, fruitUsage, effectDefinition).ConfigureAwait(false))
            {
                await player.InvokeViewPlugInAsync<IConsumeSpecialItemPlugIn>(p => p.ConsumeSpecialItemAsync(item, (ushort)(effectDefinition.Duration?.ConstantValue.Value ?? 0))).ConfigureAwait(false);
                return true;
            }
        }
        else
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ItemEffectNotFound)).ConfigureAwait(false);
        }

        return false;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new SiegePotionConsumeHandlerConfiguration();
}