// <copyright file="Potions.cs" company="MUnique">
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
        alcohol.Durability = 1;
        alcohol.Value = 30;
        alcohol.Width = 1;
        alcohol.Height = 2;
        alcohol.SetGuid(alcohol.Group, alcohol.Number);
        alcohol.ConsumeEffect = this.GameConfiguration.MagicEffects.First(effect => effect.Number == (short)MagicEffectNumber.Alcohol);
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
        apple.Durability = 3;
        apple.Value = 5;
        apple.Width = 1;
        apple.Height = 1;
        apple.SetGuid(apple.Group, apple.Number);
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
        potion.Durability = 3;
        potion.Value = 10;
        potion.Width = 1;
        potion.Height = 1;
        potion.SetGuid(potion.Group, potion.Number);
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
        potion.Durability = 3;
        potion.Value = 20;
        potion.Width = 1;
        potion.Height = 1;
        potion.SetGuid(potion.Group, potion.Number);
        return potion;
    }

    /// <summary>
    /// Gets the large healing potion definition.
    /// </summary>
    /// <returns>The created large healing definition.</returns>
    private ItemDefinition CreateLargeHealingPotion()
    {
        var potion = this.Context.CreateNew<ItemDefinition>();
        potion.Name = "Large Healing Potion";
        potion.Number = 3;
        potion.Group = 14;
        potion.DropsFromMonsters = true;
        potion.MaximumItemLevel = 1;
        potion.DropLevel = 40;
        potion.Durability = 3;
        potion.Value = 30;
        potion.Width = 1;
        potion.Height = 1;
        potion.SetGuid(potion.Group, potion.Number);
        return potion;
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
        potion.Durability = 3;
        potion.Value = 10;
        potion.Width = 1;
        potion.Height = 1;
        potion.SetGuid(potion.Group, potion.Number);
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
        potion.Durability = 3;
        potion.Value = 20;
        potion.Width = 1;
        potion.Height = 1;
        potion.SetGuid(potion.Group, potion.Number);
        return potion;
    }

    /// <summary>
    /// Gets the large mana potion definition.
    /// </summary>
    /// <returns>The created large mana definition.</returns>
    private ItemDefinition CreateLargeManaPotion()
    {
        var potion = this.Context.CreateNew<ItemDefinition>();
        potion.Name = "Large Mana Potion";
        potion.Number = 6;
        potion.Group = 14;
        potion.DropsFromMonsters = true;
        potion.MaximumItemLevel = 1;
        potion.DropLevel = 40;
        potion.Durability = 3;
        potion.Value = 30;
        potion.Width = 1;
        potion.Height = 1;
        potion.SetGuid(potion.Group, potion.Number);
        return potion;
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
        potion.Durability = 3;
        potion.Width = 1;
        potion.Height = 1;
        potion.SetGuid(potion.Group, potion.Number);
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
        potion.Durability = 3;
        potion.Width = 1;
        potion.Height = 1;
        potion.SetGuid(potion.Group, potion.Number);
        return potion;
    }

    /// <summary>
    /// Gets the large shield potion definition.
    /// </summary>
    /// <returns>The created large shield definition.</returns>
    private ItemDefinition CreateLargeShieldPotion()
    {
        var potion = this.Context.CreateNew<ItemDefinition>();
        potion.Name = "Large Shield Potion";
        potion.Number = 37;
        potion.Group = 14;
        potion.DropsFromMonsters = false;
        potion.DropLevel = 100;
        potion.Durability = 3;
        potion.Width = 1;
        potion.Height = 1;
        potion.SetGuid(potion.Group, potion.Number);
        return potion;
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
        potion.Durability = 3;
        potion.Width = 1;
        potion.Height = 1;
        potion.SetGuid(potion.Group, potion.Number);
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
        potion.Durability = 3;
        potion.Width = 1;
        potion.Height = 1;
        potion.SetGuid(potion.Group, potion.Number);
        return potion;
    }

    /// <summary>
    /// Gets the large complex potion definition.
    /// </summary>
    /// <returns>The created large complex definition.</returns>
    private ItemDefinition CreateLargeComplexPotion()
    {
        var potion = this.Context.CreateNew<ItemDefinition>();
        potion.Name = "Large Complex Potion";
        potion.Number = 40;
        potion.Group = 14;
        potion.DropsFromMonsters = true;
        potion.DropLevel = 118;
        potion.Value = 60;
        potion.Durability = 3;
        potion.Width = 1;
        potion.Height = 1;
        potion.SetGuid(potion.Group, potion.Number);
        return potion;
    }

    /// <summary>
    /// Gets the antidote definition.
    /// </summary>
    /// <returns>The created antidote definition.</returns>
    private ItemDefinition CreateAntidotePotion()
    {
        var potion = this.Context.CreateNew<ItemDefinition>();
        potion.Name = "Antidote";
        potion.Number = 8;
        potion.Group = 14;
        potion.DropsFromMonsters = true;
        potion.DropLevel = 10;
        potion.Durability = 3;
        potion.Value = 10;
        potion.Width = 1;
        potion.Height = 1;
        potion.SetGuid(potion.Group, potion.Number);
        return potion;
    }

    private ItemDefinition CreateTownPortalScroll()
    {
        var definition = this.Context.CreateNew<ItemDefinition>();
        definition.Name = "Town Portal Scroll";
        definition.Number = 10;
        definition.Group = 14;
        definition.DropsFromMonsters = true;
        definition.DropLevel = 30;
        definition.Durability = 1;
        definition.Value = 30;
        definition.Width = 1;
        definition.Height = 2;
        definition.SetGuid(definition.Group, definition.Number);
        return definition;
    }

    private ItemDefinition CreateSiegePotion()
    {
        var definition = this.Context.CreateNew<ItemDefinition>();
        definition.Name = "Potion of Bless;Potion of Soul";
        definition.Number = 7;
        definition.Group = 14;
        definition.Durability = 10;
        definition.MaximumItemLevel = 1;
        definition.Value = 30;
        definition.Width = 1;
        definition.Height = 1;
        definition.SetGuid(definition.Group, definition.Number);
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
        fruits.Durability = 1;
        fruits.Width = 1;
        fruits.Height = 1;
        fruits.SetGuid(fruits.Group, fruits.Number);
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
        item.SetGuid(item.Group, item.Number);
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.JackOlanternBlessing, TimeSpan.FromMinutes(32), (Stats.AttackSpeedAny, 10));
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
        item.SetGuid(item.Group, item.Number);
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.JackOlanternWrath, TimeSpan.FromMinutes(30), (Stats.BaseDamageBonus, 25));
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
        item.SetGuid(item.Group, item.Number);
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.JackOlanternCry, TimeSpan.FromMinutes(30), (Stats.DefenseBase, 100));
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
        item.SetGuid(item.Group, item.Number);
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.JackOlanternFood, TimeSpan.FromMinutes(30), (Stats.MaximumHealth, 500));
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
        item.SetGuid(item.Group, item.Number);
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.JackOlanternDrink, TimeSpan.FromMinutes(30), (Stats.MaximumMana, 500));
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
        item.SetGuid(item.Group, item.Number);
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.CherryBlossomWine, TimeSpan.FromMinutes(30), (Stats.MaximumMana, 700));
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
        item.SetGuid(item.Group, item.Number);
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.CherryBlossomRiceCake, TimeSpan.FromMinutes(30), (Stats.MaximumHealth, 700));
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
        item.SetGuid(item.Group, item.Number);
        this.CreateConsumeEffect(item, 16, MagicEffectNumber.CherryBlossomFlowerPetal, TimeSpan.FromMinutes(30), (Stats.BaseDamageBonus, 40));
        return item;
    }

    private MagicEffectDefinition CreateConsumeEffect(ItemDefinition item, byte subType, MagicEffectNumber effectNumber, TimeSpan duration, params (AttributeDefinition TargetAttribute, float BoostValue)[] boosts)
    {
        var effect = this.Context.CreateNew<MagicEffectDefinition>();
        effect.SetGuid(item.Number, (short)effectNumber);
        this.GameConfiguration.MagicEffects.Add(effect);
        item.ConsumeEffect = effect;
        effect.Name = item.Name;
        effect.InformObservers = false;
        effect.Number = (short)effectNumber;
        effect.StopByDeath = true;
        effect.SubType = subType;
        effect.SendDuration = true;
        effect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        effect.Duration.ConstantValue.Value = (float)duration.TotalSeconds;

        foreach (var (targetAttribute, boostValue) in boosts)
        {
            var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
            effect.PowerUpDefinitions.Add(powerUpDefinition);
            powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            powerUpDefinition.Boost.ConstantValue.Value = boostValue;
            powerUpDefinition.TargetAttribute = targetAttribute.GetPersistent(this.GameConfiguration);
        }

        return effect;
    }
}