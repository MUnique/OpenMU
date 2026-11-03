// <copyright file="ResetFruitConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Base class of the consume handlers of the reset fruits of the in-game shop.
/// A reset fruit takes back invested points of one stat, down to the base value of the character class,
/// and returns them as level up points. Unlike the fruits which remove points, it always succeeds and
/// doesn't count towards the maximum of removed fruit points.
/// </summary>
public abstract class ResetFruitConsumeHandlerPlugIn : BaseConsumeHandlerPlugIn, ISupportCustomConfiguration<ResetFruitConsumeHandlerConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <summary>
    /// Gets or sets the configuration.
    /// </summary>
    public ResetFruitConsumeHandlerConfiguration? Configuration { get; set; }

    /// <summary>
    /// Gets the stat attribute of which the points are taken back.
    /// </summary>
    protected abstract AttributeDefinition StatAttribute { get; }

    /// <inheritdoc />
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (!this.CheckPreconditions(player, item)
            || player.SelectedCharacter is not { } character
            || player.Attributes is not { } attributes)
        {
            return false;
        }

        var statAttribute = this.StatAttribute;
        var statDefinition = character.CharacterClass?.StatAttributes.FirstOrDefault(a => a.IncreasableByPlayer && a.Attribute == statAttribute);
        if (statDefinition is null)
        {
            await player.InvokeViewPlugInAsync<IFruitConsumptionResponsePlugIn>(p => p.ShowResponseAsync(FruitConsumptionResult.MinusPrevented, 0, statAttribute)).ConfigureAwait(false);
            return false;
        }

        if (player.Inventory?.EquippedItems.Any() is true)
        {
            await player.InvokeViewPlugInAsync<IFruitConsumptionResponsePlugIn>(p => p.ShowResponseAsync(FruitConsumptionResult.PreventedByEquippedItems, 0, statAttribute)).ConfigureAwait(false);
            return false;
        }

        var investedPoints = (int)(attributes[statAttribute] - statDefinition.BaseValue);
        if (investedPoints <= 0)
        {
            await player.InvokeViewPlugInAsync<IFruitConsumptionResponsePlugIn>(p => p.ShowResponseAsync(FruitConsumptionResult.MinusPreventedByDefault, 0, statAttribute)).ConfigureAwait(false);
            return false;
        }

        this.Configuration ??= new ResetFruitConsumeHandlerConfiguration();
        var pointsPerDurability = this.Configuration.PointsPerDurability;
        var points = (ushort)Math.Clamp((int)item.Durability * pointsPerDurability, 0, Math.Min(investedPoints, ushort.MaxValue));
        attributes[statAttribute] -= points;
        character.LevelUpPoints += points;
        await player.InvokeViewPlugInAsync<IFruitConsumptionResponsePlugIn>(p => p.ShowResponseAsync(FruitConsumptionResult.MinusSuccessCashShopFruit, points, statAttribute)).ConfigureAwait(false);

        // The whole fruit is used up, even if it took back fewer points than it could.
        item.Durability = 0;
        return true;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new ResetFruitConsumeHandlerConfiguration();
}
