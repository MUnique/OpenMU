// -----------------------------------------------------------------------
// <copyright file="HealthPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;

/// <summary>
/// The consume handler for a potion that recovers health.
/// </summary>
public abstract class HealthPotionConsumeHandlerPlugIn : RecoverConsumeHandlerPlugIn.ManaHealthConsumeHandlerPlugIn, IItemConsumeHandlerPlugIn
{
    /// <inheritdoc/>
    protected override AttributeDefinition MaximumAttribute => Stats.MaximumHealth;

    /// <inheritdoc/>
    protected override AttributeDefinition CurrentAttribute => Stats.CurrentHealth;

    /// <inheritdoc/>
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (await base.ConsumeItemAsync(player, item, targetItem, fruitUsage).ConfigureAwait(false))
        {
            // maybe instead of calling UpdateCurrentHealth etc. provide a more general method where we pass this.CurrentAttribute. The view can then decide what to do with it.
            await player.InvokeViewPlugInAsync<IUpdateCurrentHealthPlugIn>(p => p.UpdateCurrentHealthAsync()).ConfigureAwait(false);
            return true;
        }

        return false;
    }
}