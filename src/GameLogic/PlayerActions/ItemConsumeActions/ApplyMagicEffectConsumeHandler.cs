// <copyright file="ApplyMagicEffectConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// <see cref="IItemConsumeHandler"/> for <see cref="Item"/>s which have a defined <see cref="ItemDefinition.ConsumeEffect"/>.
/// </summary>
public class ApplyMagicEffectConsumeHandler : BaseConsumeHandler
{
    /// <inheritdoc />
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (!await base.ConsumeItemAsync(player, item, targetItem, fruitUsage).ConfigureAwait(false))
        {
            return false;
        }

        if (item.Definition?.ConsumeEffect is not { } effectDefinition
            || effectDefinition.PowerUpDefinition?.Boost is not { } boostDefinition
            || effectDefinition.PowerUpDefinition.Duration?.ConstantValue?.Value is not { } durationInSeconds)
        {
            return false;
        }

        if (await player.MagicEffectList.TryGetActiveEffectOfSubTypeAsync(effectDefinition.SubType).ConfigureAwait(false) is { } existingEffect)
        {
            await existingEffect.DisposeAsync().ConfigureAwait(false);
        }

        var boost = player.Attributes!.CreateElement(boostDefinition);

        var effect = new MagicEffect(boost, effectDefinition, TimeSpan.FromSeconds(durationInSeconds));
        await player.MagicEffectList.AddEffectAsync(effect).ConfigureAwait(false);
        return true;
    }
}