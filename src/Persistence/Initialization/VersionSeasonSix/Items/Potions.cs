﻿// <copyright file="Potions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Class which contains item definitions for jewels.
/// </summary>
public class Potions : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Potions"/> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Potions(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Creates the potion item definitions.
    /// </summary>
    public override void Initialize()
    {
        this.GameConfiguration.Items.Add(this.CreateApple());
        this.GameConfiguration.Items.Add(this.CreateSmallHealingPotion());
        this.GameConfiguration.Items.Add(this.CreateMediumHealingPotion());
        this.GameConfiguration.Items.Add(this.CreateLargeHealingPotion());
        this.GameConfiguration.Items.Add(this.CreateSmallManaPotion());
        this.GameConfiguration.Items.Add(this.CreateMediumManaPotion());
        this.GameConfiguration.Items.Add(this.CreateLargeManaPotion());
        this.GameConfiguration.Items.Add(this.CreateSmallShieldPotion());
        this.GameConfiguration.Items.Add(this.CreateMediumShieldPotion());
        this.GameConfiguration.Items.Add(this.CreateLargeShieldPotion());
        this.GameConfiguration.Items.Add(this.CreateSmallComplexPotion());
        this.GameConfiguration.Items.Add(this.CreateMediumComplexPotion());
        this.GameConfiguration.Items.Add(this.CreateLargeComplexPotion());
        this.GameConfiguration.Items.Add(this.CreateAlcohol());
        this.GameConfiguration.Items.Add(this.CreateAntidotePotion());
        this.GameConfiguration.Items.Add(this.CreateTownPortalScroll());
        this.GameConfiguration.Items.Add(this.CreateFruits());
        this.GameConfiguration.Items.Add(this.CreateSiegePotion());

        this.GameConfiguration.Items.Add(this.CreateJackOLanternBlessings());
        this.GameConfiguration.Items.Add(this.CreateJackOLanternWrath());
        this.GameConfiguration.Items.Add(this.CreateJackOLanternCry());
        this.GameConfiguration.Items.Add(this.CreateJackOLanternFood());
        this.GameConfiguration.Items.Add(this.CreateJackOLanternDrink());

        this.GameConfiguration.Items.Add(this.CreateCherryBlossomWine());
        this.GameConfiguration.Items.Add(this.CreateCherryBlossomRiceCake());
        this.GameConfiguration.Items.Add(this.CreateCherryBlossomFlowerPetal());
    }

    private ItemDefinition CreateAlcohol()
    {
        var alcohol = this.Context.CreateNew<ItemDefinition>();
        alcohol.Name = "Ale";
        alcohol.Number = 9;
        alcohol.Group = 14;
        alcohol.DropsFromMonsters = true;
        alcohol.DropLevel = 15;
        alcohol.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.AlcoholConsumeHandler).FullName;
        alcohol.Durability = 1;
        alcohol.Value = 30;
        alcohol.Width = 1;
        alcohol.Height = 2;
        return alcohol;
    }

    /// <summary>
    /// Creates the apple definition.
    /// </summary>
    /// <returns>The created apple definition.</returns>
    private ItemDefinition CreateApple()
    {
        var apple = this.Context.CreateNew<ItemDefinition>();
        apple.Name = "Apple";
        apple.Number = 0;
        apple.Group = 14;
        apple.DropsFromMonsters = true;
        apple.DropLevel = 1;
        apple.MaximumItemLevel = 1;
        apple.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.AppleConsumeHandler).FullName;
        apple.Durability = 3;
        apple.Value = 5;
        apple.Width = 1;
        apple.Height = 1;
        return apple;
    }

    /// <summary>
    /// Gets the small healing potion definition.
    /// </summary>
    /// <returns>The created small healing potion definition.</returns>
    private ItemDefinition CreateSmallHealingPotion()
    {
        var potion = this.Context.CreateNew<ItemDefinition>();
        potion.Name = "Small Healing Potion";
        potion.Number = 1;
        potion.Group = 14;
        potion.DropsFromMonsters = true;
        potion.MaximumItemLevel = 1;
        potion.DropLevel = 10;
        potion.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.SmallHealthPotionConsumeHandler).FullName;
        potion.Durability = 3;
        potion.Value = 10;
        potion.Width = 1;
        potion.Height = 1;
        return potion;
    }

    /// <summary>
    /// Gets the medium healing potion definition.
    /// </summary>
    /// <returns>The created medium healing potion definition.</returns>
    private ItemDefinition CreateMediumHealingPotion()
    {
        var potion = this.Context.CreateNew<ItemDefinition>();
        potion.Name = "Medium Healing Potion";
        potion.Number = 2;
        potion.Group = 14;
        potion.DropsFromMonsters = true;
        potion.MaximumItemLevel = 1;
        potion.DropLevel = 25;
        potion.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.MiddleHealthPotionConsumeHandler).FullName;
        potion.Durability = 3;
        potion.Value = 20;
        potion.Width = 1;
        potion.Height = 1;
        return potion;
    }

    /// <summary>
    /// Gets the large healing potion definition.
    /// </summary>
    /// <returns>The created large healing definition.</returns>
    private ItemDefinition CreateLargeHealingPotion()
    {
        var definition = this.Context.CreateNew<ItemDefinition>();
        definition.Name = "Large Healing Potion";
        definition.Number = 3;
        definition.Group = 14;
        definition.DropsFromMonsters = true;
        definition.MaximumItemLevel = 1;
        definition.DropLevel = 40;
        definition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.BigHealthPotionConsumeHandler).FullName;
        definition.Durability = 3;
        definition.Value = 30;
        definition.Width = 1;
        definition.Height = 1;
        return definition;
    }

    /// <summary>
    /// Gets the small mana potion definition.
    /// </summary>
    /// <returns>The created small mana potion definition.</returns>
    private ItemDefinition CreateSmallManaPotion()
    {
        var potion = this.Context.CreateNew<ItemDefinition>();
        potion.Name = "Small Mana Potion";
        potion.Number = 4;
        potion.Group = 14;
        potion.DropsFromMonsters = true;
        potion.MaximumItemLevel = 1;
        potion.DropLevel = 10;
        potion.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.SmallManaPotionConsumeHandler).FullName;
        potion.Durability = 3;
        potion.Value = 10;
        potion.Width = 1;
        potion.Height = 1;
        return potion;
    }

    /// <summary>
    /// Gets the medium mana potion definition.
    /// </summary>
    /// <returns>The created medium mana potion definition.</returns>
    private ItemDefinition CreateMediumManaPotion()
    {
        var potion = this.Context.CreateNew<ItemDefinition>();
        potion.Name = "Medium Mana Potion";
        potion.Number = 5;
        potion.Group = 14;
        potion.DropsFromMonsters = true;
        potion.MaximumItemLevel = 1;
        potion.DropLevel = 25;
        potion.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.MiddleManaPotionConsumeHandler).FullName;
        potion.Durability = 3;
        potion.Value = 20;
        potion.Width = 1;
        potion.Height = 1;
        return potion;
    }

    /// <summary>
    /// Gets the large mana potion definition.
    /// </summary>
    /// <returns>The created large mana definition.</returns>
    private ItemDefinition CreateLargeManaPotion()
    {
        var definition = this.Context.CreateNew<ItemDefinition>();
        definition.Name = "Large Mana Potion";
        definition.Number = 6;
        definition.Group = 14;
        definition.DropsFromMonsters = true;
        definition.MaximumItemLevel = 1;
        definition.DropLevel = 40;
        definition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.BigManaPotionConsumeHandler).FullName;
        definition.Durability = 3;
        definition.Value = 30;
        definition.Width = 1;
        definition.Height = 1;
        return definition;
    }

    /// <summary>
    /// Gets the small shield potion definition.
    /// </summary>
    /// <returns>The created small shield potion definition.</returns>
    private ItemDefinition CreateSmallShieldPotion()
    {
        var potion = this.Context.CreateNew<ItemDefinition>();
        potion.Name = "Small Shield Potion";
        potion.Number = 35;
        potion.Group = 14;
        potion.DropsFromMonsters = true;
        potion.DropLevel = 50;
        potion.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.SmallShieldPotionConsumeHandler).FullName;
        potion.Durability = 3;
        potion.Width = 1;
        potion.Height = 1;
        return potion;
    }

    /// <summary>
    /// Gets the medium shield potion definition.
    /// </summary>
    /// <returns>The created medium shield potion definition.</returns>
    private ItemDefinition CreateMediumShieldPotion()
    {
        var potion = this.Context.CreateNew<ItemDefinition>();
        potion.Name = "Medium Shield Potion";
        potion.Number = 36;
        potion.Group = 14;
        potion.DropsFromMonsters = false;
        potion.DropLevel = 80;
        potion.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.MiddleShieldPotionConsumeHandler).FullName;
        potion.Durability = 3;
        potion.Width = 1;
        potion.Height = 1;
        return potion;
    }

    /// <summary>
    /// Gets the large shield potion definition.
    /// </summary>
    /// <returns>The created large shield definition.</returns>
    private ItemDefinition CreateLargeShieldPotion()
    {
        var definition = this.Context.CreateNew<ItemDefinition>();
        definition.Name = "Large Shield Potion";
        definition.Number = 37;
        definition.Group = 14;
        definition.DropsFromMonsters = false;
        definition.DropLevel = 100;
        definition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.BigShieldPotionConsumeHandler).FullName;
        definition.Durability = 3;
        definition.Width = 1;
        definition.Height = 1;
        return definition;
    }

    /// <summary>
    /// Gets the small complex potion definition.
    /// </summary>
    /// <returns>The created small complex potion definition.</returns>
    private ItemDefinition CreateSmallComplexPotion()
    {
        var potion = this.Context.CreateNew<ItemDefinition>();
        potion.Name = "Small Complex Potion";
        potion.Number = 38;
        potion.Group = 14;
        potion.DropsFromMonsters = true;
        potion.DropLevel = 68;
        potion.Value = 20;
        potion.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.SmallComplexPotionConsumeHandler).FullName;
        potion.Durability = 3;
        potion.Width = 1;
        potion.Height = 1;
        return potion;
    }

    /// <summary>
    /// Gets the medium complex potion definition.
    /// </summary>
    /// <returns>The created medium complex potion definition.</returns>
    private ItemDefinition CreateMediumComplexPotion()
    {
        var potion = this.Context.CreateNew<ItemDefinition>();
        potion.Name = "Medium Complex Potion";
        potion.Number = 39;
        potion.Group = 14;
        potion.DropsFromMonsters = true;
        potion.DropLevel = 96;
        potion.Value = 40;
        potion.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.MediumComplexPotionConsumeHandler).FullName;
        potion.Durability = 3;
        potion.Width = 1;
        potion.Height = 1;
        return potion;
    }

    /// <summary>
    /// Gets the large complex potion definition.
    /// </summary>
    /// <returns>The created large complex definition.</returns>
    private ItemDefinition CreateLargeComplexPotion()
    {
        var definition = this.Context.CreateNew<ItemDefinition>();
        definition.Name = "Large Complex Potion";
        definition.Number = 40;
        definition.Group = 14;
        definition.DropsFromMonsters = true;
        definition.DropLevel = 118;
        definition.Value = 60;
        definition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.LargeComplexPotionConsumeHandler).FullName;
        definition.Durability = 3;
        definition.Width = 1;
        definition.Height = 1;
        return definition;
    }

    /// <summary>
    /// Gets the antidote definition.
    /// </summary>
    /// <returns>The created antidote definition.</returns>
    private ItemDefinition CreateAntidotePotion()
    {
        var definition = this.Context.CreateNew<ItemDefinition>();
        definition.Name = "Antidote";
        definition.Number = 8;
        definition.Group = 14;
        definition.DropsFromMonsters = true;
        definition.DropLevel = 10;
        definition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.AntidoteConsumeHandler).FullName;
        definition.Durability = 3;
        definition.Value = 10;
        definition.Width = 1;
        definition.Height = 1;
        return definition;
    }

    private ItemDefinition CreateTownPortalScroll()
    {
        var definition = this.Context.CreateNew<ItemDefinition>();
        definition.Name = "Town Portal Scroll";
        definition.Number = 10;
        definition.Group = 14;
        definition.DropsFromMonsters = true;
        definition.DropLevel = 30;
        definition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.TownPortalScrollConsumeHandler).FullName;
        definition.Durability = 1;
        definition.Value = 30;
        definition.Width = 1;
        definition.Height = 2;
        return definition;
    }

    private ItemDefinition CreateSiegePotion()
    {
        var definition = this.Context.CreateNew<ItemDefinition>();
        definition.Name = "Siege Potion";
        definition.Number = 7;
        definition.Group = 14;

        // todo definition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.SiegePotionConsumeHandler).FullName;
        definition.Durability = 1;
        definition.Value = 30;
        definition.Width = 1;
        definition.Height = 1;
        return definition;
    }

    /// <summary>
    /// Creates the fruit definition.
    /// </summary>
    /// <returns>The created fruit definition.</returns>
    private ItemDefinition CreateFruits()
    {
        var fruits = this.Context.CreateNew<ItemDefinition>();
        fruits.Name = "Fruits";
        fruits.Number = 15;
        fruits.Group = 13;
        fruits.MaximumItemLevel = 4;
        fruits.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.FruitConsumeHandler).FullName;
        fruits.Durability = 1;
        fruits.Width = 1;
        fruits.Height = 1;
        return fruits;
    }

    private ItemDefinition CreateJackOLanternBlessings()
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Name = "Jack O'Lantern Blessings";
        item.Number = 46;
        item.Group = 14;
        item.Durability = 10;
        item.Width = 1;
        item.Height = 2;
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.JackOlanternBlessing, Stats.AttackSpeed, 10, TimeSpan.FromMinutes(32));
        return item;
    }

    private ItemDefinition CreateJackOLanternWrath()
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Name = "Jack O'Lantern Wrath";
        item.Number = 47;
        item.Group = 14;
        item.Durability = 10;
        item.Width = 1;
        item.Height = 2;
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.JackOlanternWrath, Stats.BaseDamageBonus, 25, TimeSpan.FromMinutes(30));
        return item;
    }

    private ItemDefinition CreateJackOLanternCry()
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Name = "Jack O'Lantern Cry";
        item.Number = 48;
        item.Group = 14;
        item.Durability = 10;
        item.Width = 1;
        item.Height = 2;
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.JackOlanternCry, Stats.DefenseBase, 100, TimeSpan.FromMinutes(30));
        return item;
    }

    private ItemDefinition CreateJackOLanternFood()
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Name = "Jack O'Lantern Food";
        item.Number = 49;
        item.Group = 14;
        item.Durability = 10;
        item.Width = 1;
        item.Height = 1;
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.JackOlanternFood, Stats.MaximumHealth, 500, TimeSpan.FromMinutes(30));
        return item;
    }

    private ItemDefinition CreateJackOLanternDrink()
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Name = "Jack O'Lantern Drink";
        item.Number = 50;
        item.Group = 14;
        item.Durability = 10;
        item.Width = 1;
        item.Height = 1;
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.JackOlanternDrink, Stats.MaximumMana, 500, TimeSpan.FromMinutes(30));
        return item;
    }

    private ItemDefinition CreateCherryBlossomWine()
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Name = "Cherry Blossom Wine";
        item.Number = 85;
        item.Group = 14;
        item.Durability = 10;
        item.Width = 1;
        item.Height = 2;
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.CherryBlossomWine, Stats.MaximumMana, 700, TimeSpan.FromMinutes(30));
        return item;
    }

    private ItemDefinition CreateCherryBlossomRiceCake()
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Name = "Cherry Blossom Rice Cake";
        item.Number = 86;
        item.Group = 14;
        item.Durability = 10;
        item.Width = 1;
        item.Height = 1;
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.CherryBlossomRiceCake, Stats.MaximumHealth, 700, TimeSpan.FromMinutes(30));
        return item;
    }

    private ItemDefinition CreateCherryBlossomFlowerPetal()
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Name = "Cherry Blossom Flower Petal";
        item.Number = 87;
        item.Group = 14;
        item.Durability = 10;
        item.Width = 1;
        item.Height = 1;
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.CherryBlossomFlowerPetal, Stats.BaseDamageBonus, 40, TimeSpan.FromMinutes(30));
        return item;
    }

    private MagicEffectDefinition CreateConsumeEffect(ItemDefinition item, byte subType, MagicEffectNumber effectNumber, AttributeDefinition targetAttribute, float boostValue, TimeSpan duration)
    {
        var effect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(effect);
        item.ConsumeEffect = effect;
        effect.Name = item.Name;
        effect.InformObservers = false;
        effect.Number = (short)effectNumber;
        effect.StopByDeath = true;
        effect.SubType = subType;
        effect.SendDuration = true;
        effect.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinitionWithDuration>();
        effect.PowerUpDefinition.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        effect.PowerUpDefinition.Duration.ConstantValue.Value = (float)duration.TotalSeconds;
        effect.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        effect.PowerUpDefinition.Boost.ConstantValue.Value = boostValue;
        effect.PowerUpDefinition.TargetAttribute = targetAttribute.GetPersistent(this.GameConfiguration);
        return effect;
    }
}