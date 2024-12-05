// <copyright file="ChaosWeaponAndFirstWingsCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Crafting for Chaos Weapon and First Wings.
/// </summary>
public class ChaosWeaponAndFirstWingsCrafting : SimpleItemCraftingHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChaosWeaponAndFirstWingsCrafting"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public ChaosWeaponAndFirstWingsCrafting(SimpleCraftingSettings settings)
        : base(settings)
    {
    }

    /// <inheritdoc/>
    protected override void AddRandomItemOption(Item resultItem, Player player, byte successRate)
    {
        if (resultItem.Definition!.PossibleItemOptions.FirstOrDefault(o =>
                    o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Option))
                is { } option)
        {
            int i = Rand.NextInt(0, 3);
            if (Rand.NextRandomBool((successRate / 5) + (4 * (i + 1))))
            {
                var link = player.PersistenceContext.CreateNew<ItemOptionLink>();
                link.ItemOption = option.PossibleOptions.First();
                link.Level = 3 - i;
                resultItem.ItemOptions.Add(link);
            }
        }
    }

    /// <inheritdoc/>
    protected override void AddRandomLuckOption(Item resultItem, Player player, byte successRate)
    {
        if (Rand.NextRandomBool((successRate / 5) + 4)
            && resultItem.Definition!.PossibleItemOptions.FirstOrDefault(o =>
                    o.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.Luck))
                is { } luck)
        {
            var luckOption = player.PersistenceContext.CreateNew<ItemOptionLink>();
            luckOption.ItemOption = luck.PossibleOptions.First();
            resultItem.ItemOptions.Add(luckOption);
        }
    }

    /// <inheritdoc/>
    protected override void AddRandomSkill(Item resultItem, byte successRate)
    {
        if (Rand.NextRandomBool((successRate / 5) + 6)
            && !resultItem.HasSkill
            && resultItem.Definition!.Skill is { })
        {
            resultItem.HasSkill = true;
        }
    }
}