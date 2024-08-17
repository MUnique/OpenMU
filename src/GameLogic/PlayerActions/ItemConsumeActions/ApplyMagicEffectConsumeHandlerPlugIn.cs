// <copyright file="ApplyMagicEffectConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// <see cref="IItemConsumeHandlerPlugIn"/> for <see cref="Item"/>s which have a defined <see cref="ItemDefinition.ConsumeEffect"/>.
/// </summary>
public class ApplyMagicEffectConsumeHandlerPlugIn : BaseConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => new(0xFF, 0xFF);

    /// <inheritdoc />
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (!await base.ConsumeItemAsync(player, item, targetItem, fruitUsage).ConfigureAwait(false))
        {
            return false;
        }

        if (item.Definition?.ConsumeEffect is not { } effectDefinition
            || !effectDefinition.PowerUpDefinitions.Any()
            || effectDefinition.Duration?.ConstantValue.Value is not { } durationInSeconds)
        {
            return false;
        }

        if (await player.MagicEffectList.TryGetActiveEffectOfSubTypeAsync(effectDefinition.SubType).ConfigureAwait(false) is { } existingEffect)
        {
            await existingEffect.DisposeAsync().ConfigureAwait(false);
        }

        var boosts = effectDefinition.PowerUpDefinitions
            .Where(def => def.Boost is not null && def.TargetAttribute is not null)
            .Select(def => new MagicEffect.ElementWithTarget(player.Attributes!.CreateElement(def), def.TargetAttribute!))
            .ToArray();
        if (boosts.Length == 0)
        {
            return false;
        }

        var effect = new MagicEffect(TimeSpan.FromSeconds(durationInSeconds), effectDefinition, boosts!);
        await player.MagicEffectList.AddEffectAsync(effect).ConfigureAwait(false);
        return true;
    }
}