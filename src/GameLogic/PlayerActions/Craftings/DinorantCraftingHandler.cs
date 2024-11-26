// <copyright file="DinorantCraftingHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Crafting for Dinorant.
/// </summary>
public class DinorantCraftingHandler : SimpleItemCraftingHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DinorantCraftingHandler"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public DinorantCraftingHandler(SimpleCraftingSettings settings)
        : base(settings)
    {
    }

    /// <inheritdoc/>
    protected override void AddRandomExcellentOptions(Item resultItem, Player player)
    {
        base.AddRandomExcellentOptions(resultItem, player);

        // For Dinorant there is a second rollout for an additional bonus option to the first.
        if (resultItem.ItemOptions.Count == 1 && Rand.NextRandomBool(13)) // 0.2*0.66 = 13.2%
        {
            var dinoOptions = resultItem.Definition!.PossibleItemOptions.First(o =>
                o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Excellent));
            var bonusLink = player.PersistenceContext.CreateNew<ItemOptionLink>();
            bonusLink.ItemOption = dinoOptions.PossibleOptions
                .Except(resultItem.ItemOptions.Select(io => io.ItemOption)).SelectRandom();
            resultItem.ItemOptions.Add(bonusLink);
        }
    }
}