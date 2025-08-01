// <copyright file="DinorantCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Crafting for Dinorant.
/// </summary>
public class DinorantCrafting : SimpleItemCraftingHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DinorantCrafting"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public DinorantCrafting(SimpleCraftingSettings settings)
        : base(settings)
    {
    }

    /// <inheritdoc />
    public override CraftingResult? TryGetRequiredItems(Player player, out IList<CraftingRequiredItemLink> items, out byte successRate)
    {
        var craftingResult = base.TryGetRequiredItems(player, out items, out successRate);
        if (craftingResult is null)
        {
            var uniriaLink = items.Where(i => i.ItemRequirement.PossibleItems.Any(i => i.Name == "Horn of Uniria"));
            foreach (var item in uniriaLink.First().Items)
            {
                if (item.Durability < 255)
                {
                    return CraftingResult.IncorrectMixItems;
                }
            }
        }

        return craftingResult;
    }

    /// <inheritdoc/>
    protected override void AddRandomItemOption(Item resultItem, Player player, byte successRate)
    {
        if (Rand.NextRandomBool(30)
            && resultItem.Definition!.PossibleItemOptions.FirstOrDefault(o =>
                    o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Option))
                is { } option)
        {
            var link = player.PersistenceContext.CreateNew<ItemOptionLink>();
            link.ItemOption = option.PossibleOptions.SelectRandom();
            resultItem.ItemOptions.Add(link);

            // There is a second rollout for an additional bonus option to the first (but only if it doesn't coincide).
            if (Rand.NextRandomBool(20))
            {
                var bonusOpt = option.PossibleOptions.SelectRandom();
                if (bonusOpt != link.ItemOption)
                {
                    var bonusLink = player.PersistenceContext.CreateNew<ItemOptionLink>();
                    bonusLink.ItemOption = bonusOpt;
                    resultItem.ItemOptions.Add(bonusLink);
                }
            }
        }

        // Dinorant options were originally coded within the normal item option; each has a different level.
        foreach (var dinoOption in resultItem.ItemOptions)
        {
            if (dinoOption.ItemOption!.PowerUpDefinition!.TargetAttribute == Stats.DamageReceiveDecrement)
            {
                dinoOption.Level = 1;
            }
            else if (dinoOption.ItemOption!.PowerUpDefinition!.TargetAttribute == Stats.MaximumAbility)
            {
                dinoOption.Level = 2;
            }
            else
            {
                dinoOption.Level = 4;
            }
        }
    }
}