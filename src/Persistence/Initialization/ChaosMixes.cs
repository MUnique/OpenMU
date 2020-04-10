// <copyright file="ChaosMixes.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
    using MUnique.OpenMU.DataModel.Configuration.Items;

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
            chaosGoblin.ItemCraftings.Add(this.FruitCrafting());
            chaosGoblin.ItemCraftings.Add(this.DinorantCrafting());
            chaosGoblin.ItemCraftings.Add(this.PotionOfBlessCrafting());
            chaosGoblin.ItemCraftings.Add(this.PotionOfSoulCrafting());
            chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(3, 10, 1_000_000));
            chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(4, 11, 2_000_000));
            chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(22, 12, 4_000_000));
            chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(23, 13, 8_000_000));
            chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(49, 14, 10_000_000));
            chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(50, 15, 12_000_000));
            chaosGoblin.ItemCraftings.Add(this.BloodCastleTicketCrafting());
            chaosGoblin.ItemCraftings.Add(this.DevilSquareTicketCrafting());
            chaosGoblin.ItemCraftings.Add(this.IllusionTempleTicketCrafting());

            var elphis = this.GameConfiguration.Monsters.Single(m => m.NpcWindow == NpcWindow.ElphisRefinery);
            elphis.ItemCraftings.Add(this.GemstoneRefinery());
        }

        private ItemCrafting ItemLevelUpgradeCrafting(byte craftingNumber, byte targetLevel, int money)
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = $"+{targetLevel} Item Combination";
            crafting.Number = craftingNumber;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();

            crafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.Money = money;
            craftingSettings.SuccessPercent = (byte)(60 - (((targetLevel - 10) / 2) * 5));
            craftingSettings.SuccessPercentageAdditionForExcellentItem = -10;
            craftingSettings.SuccessPercentageAdditionForAncientItem = -15;
            craftingSettings.SuccessPercentageAdditionForSocketItem = -20;

            // Requirements:
            var item = this.Context.CreateNew<ItemCraftingRequiredItem>();
            item.Reference = 1;
            item.MinimumAmount = 1;
            item.MaximumAmount = 1;
            item.MinimumItemLevel = (byte)(targetLevel - 1);
            item.MaximumItemLevel = item.MinimumItemLevel;
            item.FailResult = MixResult.DowngradedTo0;
            item.SuccessResult = MixResult.StaysAsIs;
            craftingSettings.RequiredItems.Add(item);

            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.MinimumAmount = 1;
            chaos.SuccessResult = MixResult.Disappear;
            chaos.FailResult = MixResult.Disappear;
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            craftingSettings.RequiredItems.Add(chaos);

            var bless = this.Context.CreateNew<ItemCraftingRequiredItem>();
            bless.MinimumAmount = (byte)(targetLevel - 9);
            bless.SuccessResult = MixResult.Disappear;
            bless.FailResult = MixResult.Disappear;
            bless.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Bless");
            craftingSettings.RequiredItems.Add(bless);

            var soul = this.Context.CreateNew<ItemCraftingRequiredItem>();
            soul.MinimumAmount = (byte)(targetLevel - 9);
            soul.SuccessResult = MixResult.Disappear;
            soul.FailResult = MixResult.Disappear;
            soul.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Soul");
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
            var chaosWeaponSettings = this.Context.CreateNew<SimpleCraftingSettings>();

            chaosWeapon.SimpleCraftingSettings = chaosWeaponSettings;
            chaosWeaponSettings.MoneyPerFinalSuccessPercentage = 10000;
            chaosWeaponSettings.SuccessPercent = 1;

            // Requirements:
            var randomItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
            randomItem.MinimumAmount = 1;
            randomItem.MinimumItemLevel = 4;
            randomItem.MaximumItemLevel = 15;
            randomItem.NpcPriceDivisor = 15000;
            randomItem.FailResult = MixResult.DowngradedRandom;
            randomItem.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Option));
            randomItem.SuccessResult = MixResult.Disappear;
            chaosWeaponSettings.RequiredItems.Add(randomItem);

            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.MinimumAmount = 1;
            chaos.AddPercentage = 2;
            chaos.SuccessResult = MixResult.Disappear;
            chaos.FailResult = MixResult.Disappear;
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            chaosWeaponSettings.RequiredItems.Add(chaos);

            var bless = this.Context.CreateNew<ItemCraftingRequiredItem>();
            bless.MinimumAmount = 0;
            bless.AddPercentage = 5;
            bless.SuccessResult = MixResult.Disappear;
            bless.FailResult = MixResult.Disappear;
            bless.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Bless");
            chaosWeaponSettings.RequiredItems.Add(bless);

            var soul = this.Context.CreateNew<ItemCraftingRequiredItem>();
            soul.MinimumAmount = 0;
            soul.AddPercentage = 4;
            soul.SuccessResult = MixResult.Disappear;
            soul.FailResult = MixResult.Disappear;
            soul.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Soul");
            chaosWeaponSettings.RequiredItems.Add(soul);

            // Result:
            chaosWeaponSettings.ResultItemSelect = ResultItemSelection.Any;
            chaosWeaponSettings.ResultItemLuckOptionChance = 10;
            chaosWeaponSettings.ResultItemSkillChance = 30;

            var chaosDragonAxe = this.Context.CreateNew<ItemCraftingResultItem>();
            chaosDragonAxe.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Chaos Dragon Axe");
            chaosWeaponSettings.ResultItems.Add(chaosDragonAxe);

            var chaosNatureBow = this.Context.CreateNew<ItemCraftingResultItem>();
            chaosNatureBow.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Chaos Nature Bow");
            chaosWeaponSettings.ResultItems.Add(chaosNatureBow);

            var chaosLightningStaff = this.Context.CreateNew<ItemCraftingResultItem>();
            chaosLightningStaff.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Chaos Lightning Staff");
            chaosWeaponSettings.ResultItems.Add(chaosLightningStaff);

            return chaosWeapon;
        }

        private ItemCrafting FruitCrafting()
        {
            var fruitCrafting = this.Context.CreateNew<ItemCrafting>();
            fruitCrafting.Name = "Fruits";
            fruitCrafting.Number = 6;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();

            fruitCrafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.Money = 3_000_000;
            craftingSettings.SuccessPercent = 90;

            // Requirements:
            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.MinimumAmount = 1;
            chaos.MaximumAmount = 1;
            chaos.SuccessResult = MixResult.Disappear;
            chaos.FailResult = MixResult.Disappear;
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            craftingSettings.RequiredItems.Add(chaos);

            var creation = this.Context.CreateNew<ItemCraftingRequiredItem>();
            creation.MinimumAmount = 1;
            creation.MaximumAmount = 1;
            creation.SuccessResult = MixResult.Disappear;
            creation.FailResult = MixResult.Disappear;
            creation.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Creation");
            craftingSettings.RequiredItems.Add(creation);

            // Result:
            var fruit = this.Context.CreateNew<ItemCraftingResultItem>();
            fruit.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Fruits");
            fruit.RandomMinimumLevel = 0;
            fruit.RandomMaximumLevel = 4;
            craftingSettings.ResultItems.Add(fruit);

            return fruitCrafting;
        }

        private ItemCrafting PotionOfBlessCrafting()
        {
            var potionCrafting = this.Context.CreateNew<ItemCrafting>();
            potionCrafting.Name = "Potion of Bless";
            potionCrafting.Number = 15;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();

            potionCrafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.Money = 100_000;
            craftingSettings.SuccessPercent = 100;
            craftingSettings.MultipleAllowed = true;

            // Requirements:
            var bless = this.Context.CreateNew<ItemCraftingRequiredItem>();
            bless.MinimumAmount = 1;
            bless.Reference = 1;
            bless.SuccessResult = MixResult.Disappear;
            bless.FailResult = MixResult.Disappear;
            bless.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Bless");
            craftingSettings.RequiredItems.Add(bless);

            // Result:
            var potion = this.Context.CreateNew<ItemCraftingResultItem>();
            potion.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Siege Potion");
            potion.Durability = 10;
            craftingSettings.ResultItems.Add(potion);

            return potionCrafting;
        }

        private ItemCrafting PotionOfSoulCrafting()
        {
            var potionCrafting = this.Context.CreateNew<ItemCrafting>();
            potionCrafting.Name = "Potion of Soul";
            potionCrafting.Number = 16;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();

            potionCrafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.Money = 50_000;
            craftingSettings.SuccessPercent = 100;
            craftingSettings.MultipleAllowed = true;

            // Requirements:
            var soul = this.Context.CreateNew<ItemCraftingRequiredItem>();
            soul.MinimumAmount = 1;
            soul.Reference = 1;
            soul.SuccessResult = MixResult.Disappear;
            soul.FailResult = MixResult.Disappear;
            soul.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Soul");
            craftingSettings.RequiredItems.Add(soul);

            // Result:
            var potion = this.Context.CreateNew<ItemCraftingResultItem>();
            potion.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Siege Potion");
            potion.Durability = 10;
            potion.RandomMinimumLevel = 1;
            potion.RandomMaximumLevel = 1;
            craftingSettings.ResultItems.Add(potion);

            return potionCrafting;
        }

        private ItemCrafting GemstoneRefinery()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Gemstone Refinery";
            crafting.Number = 33;
            crafting.SimpleCraftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
            crafting.SimpleCraftingSettings.SuccessPercent = 80;

            // Requirements:
            var gemstone = this.Context.CreateNew<ItemCraftingRequiredItem>();
            gemstone.MinimumAmount = 1;
            gemstone.MaximumAmount = 1;
            gemstone.SuccessResult = MixResult.Disappear;
            gemstone.FailResult = MixResult.Disappear;
            gemstone.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Gemstone");
            crafting.SimpleCraftingSettings.RequiredItems.Add(gemstone);

            // Result:
            var harmony = this.Context.CreateNew<ItemCraftingResultItem>();
            harmony.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Harmony");
            crafting.SimpleCraftingSettings.ResultItems.Add(harmony);

            return crafting;
        }

        private ItemCrafting DinorantCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Dinorant";
            crafting.Number = 5;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();

            crafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.Money = 500_000;
            craftingSettings.SuccessPercent = 70;
            craftingSettings.ResultItemExcellentOptionChance = 10;

            // Requirements:
            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.MinimumAmount = 1;
            chaos.MaximumAmount = 1;
            chaos.SuccessResult = MixResult.Disappear;
            chaos.FailResult = MixResult.Disappear;
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            craftingSettings.RequiredItems.Add(chaos);

            var horn = this.Context.CreateNew<ItemCraftingRequiredItem>();
            horn.MinimumAmount = 10;
            horn.MaximumAmount = 10;
            horn.SuccessResult = MixResult.Disappear;
            horn.FailResult = MixResult.Disappear;
            horn.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Horn of Uniria");
            craftingSettings.RequiredItems.Add(horn);

            // Result:
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
            crafting.ItemCraftingHandlerClassName = typeof(GameLogic.PlayerActions.Craftings.BloodCastleTicketCrafting).FullName;
            return crafting;
        }

        private ItemCrafting DevilSquareTicketCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Devil's Square Ticket";
            crafting.Number = 2;
            crafting.ItemCraftingHandlerClassName = typeof(GameLogic.PlayerActions.Craftings.DevilSquareTicketCrafting).FullName;
            return crafting;
        }

        private ItemCrafting IllusionTempleTicketCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Illusion Temple Ticket";
            crafting.Number = 37;
            crafting.ItemCraftingHandlerClassName = typeof(GameLogic.PlayerActions.Craftings.IllusionTempleTicketCrafting).FullName;
            return crafting;
        }
    }
}
