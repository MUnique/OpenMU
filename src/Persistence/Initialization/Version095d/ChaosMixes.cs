// <copyright file="ChaosMixes.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Items;

/// <summary>
/// Initializer for chaos mixes.
/// </summary>
/// <remarks>
/// Currently, there are no Talisman of Luck supported. If someone wants to add them, you'd have to do the following steps:
///   - The <see cref="ItemDefinition"/> for the TOL needs to be created somewhere.
///   - The created item definition needs to be added as <see cref="ItemCraftingRequiredItem"/>
///     to the corresponding <see cref="ItemCrafting"/>s with MinimumAmount of 0, Maximum of 1, AddPercentage of 25.
/// </remarks>
public class ChaosMixes : InitializerBase
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
        var chaosGoblin = this.GameConfiguration.Monsters.First(m => m.NpcWindow == NpcWindow.ChaosMachine);
        chaosGoblin.ItemCraftings.Add(this.ChaosWeaponCrafting());
        chaosGoblin.ItemCraftings.Add(this.DinorantCrafting());

        chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(3, 10));
        chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(4, 11));
        chaosGoblin.ItemCraftings.Add(this.FirstWingsCrafting());

        chaosGoblin.ItemCraftings.Add(this.DevilSquareTicketCrafting());
    }

    private ItemCrafting ItemLevelUpgradeCrafting(byte craftingNumber, byte targetLevel)
    {
        var crafting = this.Context.CreateNew<ItemCrafting>();
        crafting.Name = $"+{targetLevel} Item Combination";
        crafting.Number = craftingNumber;
        var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();

        crafting.SimpleCraftingSettings = craftingSettings;
        craftingSettings.Money = 2_000_000 * (targetLevel - 9);
        craftingSettings.SuccessPercent = (byte)(targetLevel == 10 ? 50 : 45);
        craftingSettings.SuccessPercentageAdditionForLuck = 25;

        // Requirements:
        var item = this.Context.CreateNew<ItemCraftingRequiredItem>();
        item.Reference = 1;
        item.MinimumAmount = 1;
        item.MaximumAmount = 1;
        item.MinimumItemLevel = (byte)(targetLevel - 1);
        item.MaximumItemLevel = item.MinimumItemLevel;
        item.FailResult = MixResult.Disappear;
        item.SuccessResult = MixResult.StaysAsIs;
        craftingSettings.RequiredItems.Add(item);

        var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
        chaos.MinimumAmount = 1;
        chaos.MaximumAmount = 1;
        chaos.SuccessResult = MixResult.Disappear;
        chaos.FailResult = MixResult.Disappear;
        chaos.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos"));
        craftingSettings.RequiredItems.Add(chaos);

        var bless = this.Context.CreateNew<ItemCraftingRequiredItem>();
        bless.MinimumAmount = (byte)(targetLevel - 9);
        bless.MaximumAmount = bless.MinimumAmount;
        bless.SuccessResult = MixResult.Disappear;
        bless.FailResult = MixResult.Disappear;
        bless.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Bless"));
        craftingSettings.RequiredItems.Add(bless);

        var soul = this.Context.CreateNew<ItemCraftingRequiredItem>();
        soul.MinimumAmount = (byte)(targetLevel - 9);
        soul.MaximumAmount = soul.MinimumAmount;
        soul.SuccessResult = MixResult.Disappear;
        soul.FailResult = MixResult.Disappear;
        soul.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Soul"));
        craftingSettings.RequiredItems.Add(soul);

        // Result:
        var upgrade = this.Context.CreateNew<ItemCraftingResultItem>();
        upgrade.Reference = 1;
        upgrade.AddLevel = 1;
        craftingSettings.ResultItems.Add(upgrade);
        return crafting;
    }

    private ItemCrafting ChaosWeaponCrafting()
    {
        var chaosWeapon = this.Context.CreateNew<ItemCrafting>();
        chaosWeapon.Name = "Chaos Weapon";
        chaosWeapon.Number = 1;
        chaosWeapon.ItemCraftingHandlerClassName = typeof(ChaosWeaponAndFirstWingsCrafting).FullName!;
        var chaosWeaponSettings = this.Context.CreateNew<SimpleCraftingSettings>();
        chaosWeapon.SimpleCraftingSettings = chaosWeaponSettings;
        chaosWeaponSettings.MoneyPerFinalSuccessPercentage = 10_000;
        chaosWeaponSettings.NpcPriceDivisor = 20_000;

        // Requirements:
        var randomItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
        randomItem.MinimumAmount = 1;
        randomItem.MinimumItemLevel = 4;
        randomItem.MaximumItemLevel = Constants.MaximumItemLevel;
        randomItem.FailResult = MixResult.ChaosWeaponAndFirstWingsDowngradedRandom;
        randomItem.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Option));
        randomItem.SuccessResult = MixResult.Disappear;
        chaosWeaponSettings.RequiredItems.Add(randomItem);

        var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
        chaos.MinimumAmount = 1;
        chaos.SuccessResult = MixResult.Disappear;
        chaos.FailResult = MixResult.Disappear;
        chaos.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos"));
        chaosWeaponSettings.RequiredItems.Add(chaos);

        var bless = this.Context.CreateNew<ItemCraftingRequiredItem>();
        bless.MinimumAmount = 0;
        bless.SuccessResult = MixResult.Disappear;
        bless.FailResult = MixResult.Disappear;
        bless.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Bless"));
        chaosWeaponSettings.RequiredItems.Add(bless);

        var soul = this.Context.CreateNew<ItemCraftingRequiredItem>();
        soul.MinimumAmount = 0;
        soul.SuccessResult = MixResult.Disappear;
        soul.FailResult = MixResult.Disappear;
        soul.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Soul"));
        chaosWeaponSettings.RequiredItems.Add(soul);

        // Result:
        chaosWeaponSettings.ResultItemSelect = ResultItemSelection.Any;

        var chaosDragonAxe = this.Context.CreateNew<ItemCraftingResultItem>();
        chaosDragonAxe.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Chaos Dragon Axe");
        chaosDragonAxe.RandomMinimumLevel = 0;
        chaosDragonAxe.RandomMaximumLevel = 4;
        chaosWeaponSettings.ResultItems.Add(chaosDragonAxe);

        var chaosNatureBow = this.Context.CreateNew<ItemCraftingResultItem>();
        chaosNatureBow.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Chaos Nature Bow");
        chaosNatureBow.RandomMinimumLevel = 0;
        chaosNatureBow.RandomMaximumLevel = 4;
        chaosWeaponSettings.ResultItems.Add(chaosNatureBow);

        var chaosLightningStaff = this.Context.CreateNew<ItemCraftingResultItem>();
        chaosLightningStaff.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Chaos Lightning Staff");
        chaosLightningStaff.RandomMinimumLevel = 0;
        chaosLightningStaff.RandomMaximumLevel = 4;
        chaosWeaponSettings.ResultItems.Add(chaosLightningStaff);

        return chaosWeapon;
    }

    private ItemCrafting FirstWingsCrafting()
    {
        var crafting = this.Context.CreateNew<ItemCrafting>();
        crafting.Name = "1st Level Wings";
        crafting.Number = 11;
        crafting.ItemCraftingHandlerClassName = typeof(ChaosWeaponAndFirstWingsCrafting).FullName!;
        var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
        crafting.SimpleCraftingSettings = craftingSettings;
        craftingSettings.MoneyPerFinalSuccessPercentage = 10_000;
        craftingSettings.NpcPriceDivisor = 20_000;

        // Requirements:
        var chaosWeapon = this.Context.CreateNew<ItemCraftingRequiredItem>();
        chaosWeapon.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == 4 && item.Number == 6));
        chaosWeapon.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == 2 && item.Number == 6));
        chaosWeapon.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == 5 && item.Number == 7));
        chaosWeapon.MinimumAmount = 1;
        chaosWeapon.MaximumAmount = 1;
        chaosWeapon.MinimumItemLevel = 4;
        chaosWeapon.MaximumItemLevel = Constants.MaximumItemLevel;
        chaosWeapon.FailResult = MixResult.ChaosWeaponAndFirstWingsDowngradedRandom;
        chaosWeapon.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Option));
        chaosWeapon.SuccessResult = MixResult.Disappear;
        craftingSettings.RequiredItems.Add(chaosWeapon);

        var randomItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
        randomItem.MinimumAmount = 0;
        randomItem.MinimumItemLevel = 4;
        randomItem.MaximumItemLevel = Constants.MaximumItemLevel;
        randomItem.FailResult = MixResult.Disappear;
        randomItem.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Option));
        randomItem.SuccessResult = MixResult.Disappear;
        craftingSettings.RequiredItems.Add(randomItem);

        var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
        chaos.MinimumAmount = 1;
        chaos.SuccessResult = MixResult.Disappear;
        chaos.FailResult = MixResult.Disappear;
        chaos.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos"));
        craftingSettings.RequiredItems.Add(chaos);

        var bless = this.Context.CreateNew<ItemCraftingRequiredItem>();
        bless.MinimumAmount = 0;
        bless.SuccessResult = MixResult.Disappear;
        bless.FailResult = MixResult.Disappear;
        bless.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Bless"));
        craftingSettings.RequiredItems.Add(bless);

        var soul = this.Context.CreateNew<ItemCraftingRequiredItem>();
        soul.MinimumAmount = 0;
        soul.SuccessResult = MixResult.Disappear;
        soul.FailResult = MixResult.Disappear;
        soul.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Soul"));
        craftingSettings.RequiredItems.Add(soul);

        // Result:
        craftingSettings.ResultItemSelect = ResultItemSelection.Any;

        var fairyWings = this.Context.CreateNew<ItemCraftingResultItem>();
        fairyWings.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 0);
        craftingSettings.ResultItems.Add(fairyWings);

        var heavenWings = this.Context.CreateNew<ItemCraftingResultItem>();
        heavenWings.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 1);
        craftingSettings.ResultItems.Add(heavenWings);

        var satanWings = this.Context.CreateNew<ItemCraftingResultItem>();
        satanWings.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 2);
        craftingSettings.ResultItems.Add(satanWings);

        return crafting;
    }

    private ItemCrafting DinorantCrafting()
    {
        var crafting = this.Context.CreateNew<ItemCrafting>();
        crafting.Name = "Dinorant";
        crafting.Number = 5;
        crafting.ItemCraftingHandlerClassName = typeof(DinorantCrafting).FullName!;
        var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
        crafting.SimpleCraftingSettings = craftingSettings;
        craftingSettings.Money = 250_000;
        craftingSettings.SuccessPercent = 70;

        // Requirements:
        var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
        chaos.MinimumAmount = 1;
        chaos.MaximumAmount = 1;
        chaos.SuccessResult = MixResult.Disappear;
        chaos.FailResult = MixResult.Disappear;
        chaos.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos"));
        craftingSettings.RequiredItems.Add(chaos);

        var horn = this.Context.CreateNew<ItemCraftingRequiredItem>();
        horn.MinimumAmount = 3;
        horn.MaximumAmount = 3;
        horn.SuccessResult = MixResult.Disappear;
        horn.FailResult = MixResult.Disappear;
        horn.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Horn of Uniria"));
        craftingSettings.RequiredItems.Add(horn);

        // Result:
        craftingSettings.ResultItemSkillChance = 100;

        var dinorant = this.Context.CreateNew<ItemCraftingResultItem>();
        dinorant.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Horn of Dinorant");
        craftingSettings.ResultItems.Add(dinorant);

        return crafting;
    }

    private ItemCrafting DevilSquareTicketCrafting()
    {
        var crafting = this.Context.CreateNew<ItemCrafting>();
        crafting.Name = "Devil's Square Ticket";
        crafting.Number = 2;
        crafting.ItemCraftingHandlerClassName = typeof(GameLogic.PlayerActions.Craftings.DevilSquareTicketCrafting).FullName!;
        return crafting;
    }
}