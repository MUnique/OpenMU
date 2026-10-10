// <copyright file="ChaosMixes.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097k;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

/// <summary>
/// Initializer for chaos mixes of version 0.97k.
/// </summary>
/// <remarks>
/// Compared to version 0.95d, it adds the mixes for the second wings and the Blood Castle ticket.
/// They are configured like in <see cref="VersionSeasonSix.ChaosMixes"/>, without the items of later versions.
/// </remarks>
public class ChaosMixes : Version095d.ChaosMixes
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChaosMixes"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public ChaosMixes(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();
        var chaosGoblin = this.GameConfiguration.Monsters.First(m => m.NpcWindow == NpcWindow.ChaosMachine);
        chaosGoblin.ItemCraftings.Add(this.SecondWingsCrafting());
        chaosGoblin.ItemCraftings.Add(this.BloodCastleTicketCrafting());
    }

    private ItemCrafting SecondWingsCrafting()
    {
        var crafting = this.Context.CreateNew<ItemCrafting>();
        crafting.Name = "2nd Level Wings";
        crafting.Number = 7;
        crafting.ItemCraftingHandlerClassName = typeof(SecondWingsCrafting).FullName!;
        var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
        crafting.SimpleCraftingSettings = craftingSettings;
        craftingSettings.Money = 5_000_000;
        craftingSettings.MaximumSuccessPercent = 90;

        // Requirements:
        var firstWing = this.Context.CreateNew<ItemCraftingRequiredItem>();
        firstWing.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == 12 && item.Number == 0));
        firstWing.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == 12 && item.Number == 1));
        firstWing.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == 12 && item.Number == 2));
        firstWing.MinimumAmount = 1;
        firstWing.MaximumAmount = 1;
        firstWing.MinimumItemLevel = 0;
        firstWing.MaximumItemLevel = 15;
        firstWing.NpcPriceDivisor = 4_000_000;
        firstWing.FailResult = MixResult.Disappear;
        firstWing.SuccessResult = MixResult.Disappear;
        craftingSettings.RequiredItems.Add(firstWing);

        var randomExcItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
        randomExcItem.MinimumAmount = 0;
        randomExcItem.MinimumItemLevel = 4;
        randomExcItem.MaximumItemLevel = 15;
        randomExcItem.NpcPriceDivisor = 40_000;
        randomExcItem.FailResult = MixResult.Disappear;
        randomExcItem.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Excellent));
        randomExcItem.SuccessResult = MixResult.Disappear;
        craftingSettings.RequiredItems.Add(randomExcItem);

        var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
        chaos.MinimumAmount = 1;
        chaos.MaximumAmount = 1;
        chaos.SuccessResult = MixResult.Disappear;
        chaos.FailResult = MixResult.Disappear;
        chaos.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name.ValueInNeutralLanguage == "Jewel of Chaos"));
        craftingSettings.RequiredItems.Add(chaos);

        var feather = this.Context.CreateNew<ItemCraftingRequiredItem>();
        feather.MinimumAmount = 1;
        feather.MaximumAmount = 1;
        feather.SuccessResult = MixResult.Disappear;
        feather.FailResult = MixResult.Disappear;
        feather.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Group == 13 && i.Number == 14));
        craftingSettings.RequiredItems.Add(feather);

        // Result:
        craftingSettings.ResultItemSelect = ResultItemSelection.Any;
        craftingSettings.ResultItemLuckOptionChance = 20;
        craftingSettings.ResultItemExcellentOptionChance = 20;
        craftingSettings.ResultItemMaxExcOptionCount = 1;

        for (short wingNumber = 3; wingNumber <= 6; wingNumber++)
        {
            var number = wingNumber;
            var resultItem = this.Context.CreateNew<ItemCraftingResultItem>();
            resultItem.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == number);
            craftingSettings.ResultItems.Add(resultItem);
        }

        return crafting;
    }

    private ItemCrafting BloodCastleTicketCrafting()
    {
        var crafting = this.Context.CreateNew<ItemCrafting>();
        crafting.Name = "Blood Castle Ticket";
        crafting.Number = 8;
        crafting.ItemCraftingHandlerClassName = typeof(BloodCastleTicketCrafting).FullName!;
        return crafting;
    }
}
