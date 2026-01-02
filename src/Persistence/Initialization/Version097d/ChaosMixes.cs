// <copyright file="ChaosMixes.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097d;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Items;

/// <summary>
/// Chaos mix initialization for 0.97d based on the classic server data.
/// </summary>
internal class ChaosMixes : InitializerBase
{
    public ChaosMixes(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    public override void Initialize()
    {
        var chaosGoblin = this.GameConfiguration.Monsters.First(m => m.NpcWindow == NpcWindow.ChaosMachine);
        chaosGoblin.ItemCraftings.Add(this.ChaosWeaponCrafting());
        chaosGoblin.ItemCraftings.Add(this.DinorantCrafting());
        chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(3, 10, 60, 2_000_000, 20));
        chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(4, 11, 60, 4_000_000, 20));
        chaosGoblin.ItemCraftings.Add(this.FirstWingsCrafting());
        chaosGoblin.ItemCraftings.Add(this.SecondWingsCrafting());
        chaosGoblin.ItemCraftings.Add(this.FruitCrafting());
        chaosGoblin.ItemCraftings.Add(this.BloodCastleTicketCrafting());
        chaosGoblin.ItemCraftings.Add(this.DevilSquareTicketCrafting());
    }

    private ItemCrafting ItemLevelUpgradeCrafting(byte craftingNumber, byte targetLevel, byte successPercent, int money, byte luckAddition)
    {
        var crafting = this.Context.CreateNew<ItemCrafting>();
        crafting.Name = $"+{targetLevel} Item Combination";
        crafting.Number = craftingNumber;
        var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();

        crafting.SimpleCraftingSettings = craftingSettings;
        craftingSettings.Money = money;
        craftingSettings.SuccessPercent = successPercent;
        craftingSettings.SuccessPercentageAdditionForLuck = luckAddition;

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

        var firstWing = this.Context.CreateNew<ItemCraftingRequiredItem>();
        firstWing.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == 12 && item.Number == 0));
        firstWing.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == 12 && item.Number == 1));
        firstWing.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == 12 && item.Number == 2));
        firstWing.MinimumAmount = 1;
        firstWing.MaximumAmount = 1;
        firstWing.MinimumItemLevel = 0;
        firstWing.MaximumItemLevel = Constants.MaximumItemLevel;
        firstWing.NpcPriceDivisor = 4_000_000;
        firstWing.FailResult = MixResult.Disappear;
        firstWing.SuccessResult = MixResult.Disappear;
        craftingSettings.RequiredItems.Add(firstWing);

        var randomExcItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
        randomExcItem.MinimumAmount = 0;
        randomExcItem.MinimumItemLevel = 4;
        randomExcItem.MaximumItemLevel = Constants.MaximumItemLevel;
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
        chaos.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos"));
        craftingSettings.RequiredItems.Add(chaos);

        var feather = this.Context.CreateNew<ItemCraftingRequiredItem>();
        feather.MinimumAmount = 1;
        feather.MaximumAmount = 1;
        feather.SuccessResult = MixResult.Disappear;
        feather.FailResult = MixResult.Disappear;
        feather.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Loch's Feather"));
        craftingSettings.RequiredItems.Add(feather);

        craftingSettings.ResultItemSelect = ResultItemSelection.Any;
        craftingSettings.ResultItemLuckOptionChance = 20;
        craftingSettings.ResultItemExcellentOptionChance = 20;
        craftingSettings.ResultItemMaxExcOptionCount = 1;

        var wingsOfSpirit = this.Context.CreateNew<ItemCraftingResultItem>();
        wingsOfSpirit.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 3);
        craftingSettings.ResultItems.Add(wingsOfSpirit);

        var wingsOfSoul = this.Context.CreateNew<ItemCraftingResultItem>();
        wingsOfSoul.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 4);
        craftingSettings.ResultItems.Add(wingsOfSoul);

        var wingsOfDragon = this.Context.CreateNew<ItemCraftingResultItem>();
        wingsOfDragon.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 5);
        craftingSettings.ResultItems.Add(wingsOfDragon);

        var wingsOfDarkness = this.Context.CreateNew<ItemCraftingResultItem>();
        wingsOfDarkness.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 6);
        craftingSettings.ResultItems.Add(wingsOfDarkness);

        return crafting;
    }

    private ItemCrafting FruitCrafting()
    {
        var fruitCrafting = this.Context.CreateNew<ItemCrafting>();
        fruitCrafting.Name = "Fruit";
        fruitCrafting.Number = 6;
        var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
        fruitCrafting.SimpleCraftingSettings = craftingSettings;
        craftingSettings.Money = 3_000_000;
        craftingSettings.SuccessPercent = 90;

        var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
        chaos.MinimumAmount = 1;
        chaos.MaximumAmount = 1;
        chaos.SuccessResult = MixResult.Disappear;
        chaos.FailResult = MixResult.Disappear;
        chaos.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos"));
        craftingSettings.RequiredItems.Add(chaos);

        var creation = this.Context.CreateNew<ItemCraftingRequiredItem>();
        creation.MinimumAmount = 1;
        creation.MaximumAmount = 1;
        creation.SuccessResult = MixResult.Disappear;
        creation.FailResult = MixResult.Disappear;
        creation.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Creation"));
        craftingSettings.RequiredItems.Add(creation);

        var fruit = this.Context.CreateNew<ItemCraftingResultItem>();
        fruit.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Fruit");
        craftingSettings.ResultItems.Add(fruit);

        return fruitCrafting;
    }

    private ItemCrafting DinorantCrafting()
    {
        var crafting = this.Context.CreateNew<ItemCrafting>();
        crafting.Name = "Dinorant";
        crafting.Number = 5;
        crafting.ItemCraftingHandlerClassName = typeof(GameLogic.PlayerActions.Craftings.DinorantCrafting).FullName!;
        var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
        crafting.SimpleCraftingSettings = craftingSettings;
        craftingSettings.Money = 500_000;
        craftingSettings.SuccessPercent = 70;

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

        craftingSettings.ResultItemSkillChance = 100;

        var dinorant = this.Context.CreateNew<ItemCraftingResultItem>();
        dinorant.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Horn of Dinorant");
        craftingSettings.ResultItems.Add(dinorant);

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

    private ItemCrafting DevilSquareTicketCrafting()
    {
        var crafting = this.Context.CreateNew<ItemCrafting>();
        crafting.Name = "Devil's Square Ticket";
        crafting.Number = 2;
        crafting.ItemCraftingHandlerClassName = typeof(DevilSquareTicketCrafting).FullName!;
        return crafting;
    }
}
