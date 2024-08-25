// <copyright file="SocketSystem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

/// <summary>
/// Initialization of seeds, spheres and seed spheres, their possible options, the adaption of the socket items
/// and the craftings to modify them.
/// </summary>
public class SocketSystem : InitializerBase
{
    /// <summary>
    /// Gets the seed sphere item number start (inclusive).
    /// </summary>
    private const int SeedSphereNumberStart = 100;

    /// <summary>
    /// Gets the number of sphere levels.
    /// </summary>
    private const int SphereLevels = 5;

    /// <summary>
    /// Gets the number of seed types.
    /// </summary>
    private const int SeedTypes = 6;

    /// <summary>
    /// Gets the seed sphere item number end (inclusive).
    /// </summary>
    private const int SeedSphereNumberEnd = SeedSphereNumberStart + (SphereLevels * SeedTypes) - 1;

    private ItemOptionDefinition? _fireOptions;
    private ItemOptionDefinition? _iceOptions;
    private ItemOptionDefinition? _lightningOptions;
    private ItemOptionDefinition? _windOptions;
    private ItemOptionDefinition? _waterOptions;
    private ItemOptionDefinition? _earthOptions;
    private ItemOptionDefinition? _bonusArmorOptions;
    private ItemOptionDefinition? _bonusPhysOptions;
    private ItemOptionDefinition? _bonusWizOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="SocketSystem"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public SocketSystem(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.CreateSocketOptions();
        var types = new[]
        {
            ("Fire", this._fireOptions!),
            ("Water", this._waterOptions!),
            ("Ice", this._iceOptions!),
            ("Wind", this._windOptions!),
            ("Lightning", this._lightningOptions!),
            ("Earth", this._earthOptions!),
        };

        const int seedNumberStart = 60;
        for (byte number = seedNumberStart; number < seedNumberStart + types.Length; number++)
        {
            var type = types[number - 60];
            this.CreateSeed(number, $"Seed ({type.Item1})", type.Item2!);
        }

        this.CreateSphere(70, "Sphere (Mono)", 102);
        this.CreateSphere(71, "Sphere (Di)", 122);
        this.CreateSphere(72, "Sphere (Tri)", 132);
        this.CreateSphere(73, "Sphere (4)", null);
        this.CreateSphere(74, "Sphere (5)", null);

        for (byte level = 0; level < SphereLevels; level++)
        {
            var number = SeedSphereNumberStart + (level * types.Length);
            foreach (var type in types)
            {
                this.CreateSeedSphere((byte)number, $"Seed Sphere ({type.Item1}) ({level + 1})", level, type.Item2!);
                number++;
            }
        }

        this.AddSocketsToItems();
        this.AddSocketPackageOptions();

        var seedMaster = this.GameConfiguration.Monsters.Single(m => m.NpcWindow == NpcWindow.SeedMaster);
        seedMaster.ItemCraftings.Add(this.SeedCrafting());
        seedMaster.ItemCraftings.Add(this.SeedSphereCrafting());

        var seedResearcher = this.GameConfiguration.Monsters.Single(m => m.NpcWindow == NpcWindow.SeedResearcher);
        seedResearcher.ItemCraftings.Add(this.MountSeedSphereCrafting());
        seedResearcher.ItemCraftings.Add(this.RemoveSeedSphereCrafting());
    }

    private void AddSocketPackageOptions()
    {
        var doubleDamageChance = this.Context.CreateNew<ItemOptionCombinationBonus>();
        doubleDamageChance.Number = 1;
        doubleDamageChance.Description = "Socket package option: Double Damage Chance 3%";
        doubleDamageChance.AppliesMultipleTimes = false;
        doubleDamageChance.Requirements.Add(this.CreateBonusRequirement(SocketSubOptionType.Fire, 1));
        doubleDamageChance.Requirements.Add(this.CreateBonusRequirement(SocketSubOptionType.Lightning, 1));
        doubleDamageChance.Requirements.Add(this.CreateBonusRequirement(SocketSubOptionType.Ice, 1));
        doubleDamageChance.Requirements.Add(this.CreateBonusRequirement(SocketSubOptionType.Water, 1));
        doubleDamageChance.Requirements.Add(this.CreateBonusRequirement(SocketSubOptionType.Wind, 1));
        doubleDamageChance.Requirements.Add(this.CreateBonusRequirement(SocketSubOptionType.Earth, 1));
        var doubleDamageBonus = this.Context.CreateNew<PowerUpDefinition>();
        doubleDamageBonus.TargetAttribute = Stats.DoubleDamageChance.GetPersistent(this.GameConfiguration);
        doubleDamageBonus.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        doubleDamageBonus.Boost.ConstantValue.Value = 0.03f;
        doubleDamageBonus.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;
        doubleDamageChance.Bonus = doubleDamageBonus;
        this.GameConfiguration.ItemOptionCombinationBonuses.Add(doubleDamageChance);

        var ignoreDamageChance = this.Context.CreateNew<ItemOptionCombinationBonus>();
        ignoreDamageChance.Number = 2;
        ignoreDamageChance.Description = "Socket package option: Ignore Defense Chance 1%";
        ignoreDamageChance.AppliesMultipleTimes = false;
        ignoreDamageChance.Requirements.Add(this.CreateBonusRequirement(SocketSubOptionType.Fire, 1));
        ignoreDamageChance.Requirements.Add(this.CreateBonusRequirement(SocketSubOptionType.Lightning, 1));
        ignoreDamageChance.Requirements.Add(this.CreateBonusRequirement(SocketSubOptionType.Ice, 1));
        ignoreDamageChance.Requirements.Add(this.CreateBonusRequirement(SocketSubOptionType.Water, 3));
        ignoreDamageChance.Requirements.Add(this.CreateBonusRequirement(SocketSubOptionType.Wind, 3));
        ignoreDamageChance.Requirements.Add(this.CreateBonusRequirement(SocketSubOptionType.Earth, 2));
        var ignoreDamageBonus = this.Context.CreateNew<PowerUpDefinition>();
        ignoreDamageBonus.TargetAttribute = Stats.DefenseIgnoreChance.GetPersistent(this.GameConfiguration);
        ignoreDamageBonus.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        ignoreDamageBonus.Boost.ConstantValue.Value = 0.01f;
        ignoreDamageBonus.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;
        ignoreDamageChance.Bonus = ignoreDamageBonus;
        this.GameConfiguration.ItemOptionCombinationBonuses.Add(ignoreDamageChance);
    }

    private CombinationBonusRequirement CreateBonusRequirement(SocketSubOptionType element, int amount)
    {
        var requirement = this.Context.CreateNew<CombinationBonusRequirement>();
        requirement.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.SocketOption);
        requirement.SubOptionType = (int)element;
        requirement.MinimumCount = amount;
        return requirement;
    }

    private void AddSocketsToItems()
    {
        this.AddWeaponSockets(0, 26, 3); // Flameberge
        this.AddWeaponSockets(0, 27, 3); // Sword Breaker
        this.AddWeaponSockets(0, 28, 3); // Imperial Sword
        this.AddWeaponSockets(2, 16, 3); // Frost Mace
        this.AddWeaponSockets(2, 17, 3); // Absolute Scepter
        this.AddWeaponSockets(4, 23, 5); // Stinger Bow
        this.AddWeaponSockets(5, 20, 3); // Eternal Wing Stick
        this.AddWeaponSockets(5, 30, 3); // Deadly Staff
        this.AddWeaponSockets(5, 31, 3); // Imperial Staff

        this.AddArmorSockets(6, 17); // Crimson Glory
        this.AddArmorSockets(6, 18); // Salamander Shield
        this.AddArmorSockets(6, 19); // Frost Barrier
        this.AddArmorSockets(6, 20); // Guardian Shield
        this.AddArmorSockets(7, 45); // Titan Helm
        this.AddArmorSockets(7, 46); // Brave Helm
        this.AddArmorSockets(7, 49); // Seraphim Helm
        this.AddArmorSockets(7, 50); // Faith Helm
        this.AddArmorSockets(7, 51); // Phaewang Helm
        this.AddArmorSockets(7, 52); // Hades Helm
        this.AddArmorSockets(7, 53); // Queen Helm
        this.AddArmorSockets(8, 45); // Titan Armor
        this.AddArmorSockets(8, 46); // Brave Armor
        this.AddArmorSockets(8, 47); // Destory Armor
        this.AddArmorSockets(8, 48); // Phantom Armor
        this.AddArmorSockets(8, 49); // Seraphim Armor
        this.AddArmorSockets(8, 50); // Faith Armor
        this.AddArmorSockets(8, 51); // Phaewang Armor
        this.AddArmorSockets(8, 52); // Hades Armor
        this.AddArmorSockets(8, 53); // Queen Armor
        this.AddArmorSockets(9, 45); // Titan Pants
        this.AddArmorSockets(9, 46); // Brave Pants
        this.AddArmorSockets(9, 47); // Destory Pants
        this.AddArmorSockets(9, 48); // Phantom Pants
        this.AddArmorSockets(9, 49); // Seraphim Pants
        this.AddArmorSockets(9, 50); // Faith Pants
        this.AddArmorSockets(9, 51); // Phaewang Pants
        this.AddArmorSockets(9, 52); // Hades Pants
        this.AddArmorSockets(9, 53); // Queen Pants
        this.AddArmorSockets(10, 45); // Titan Gloves
        this.AddArmorSockets(10, 46); // Brave Gloves
        this.AddArmorSockets(10, 47); // Destory Gloves
        this.AddArmorSockets(10, 48); // Phantom Gloves
        this.AddArmorSockets(10, 49); // Seraphim Gloves
        this.AddArmorSockets(10, 50); // Faith Gloves
        this.AddArmorSockets(10, 51); // Phaewang Gloves
        this.AddArmorSockets(10, 52); // Hades Gloves
        this.AddArmorSockets(10, 53); // Queen Gloves
        this.AddArmorSockets(11, 45); // Titan Boots
        this.AddArmorSockets(11, 46); // Brave Boots
        this.AddArmorSockets(11, 47); // Destory Boots
        this.AddArmorSockets(11, 48); // Phantom Boots
        this.AddArmorSockets(11, 49); // Seraphim Boots
        this.AddArmorSockets(11, 50); // Faith Boots
        this.AddArmorSockets(11, 51); // Phaewang Boots
        this.AddArmorSockets(11, 52); // Hades Boots
        this.AddArmorSockets(11, 53); // Queen Boots
    }

    private void AddArmorSockets(byte group, short number)
    {
        var item = this.GameConfiguration.Items.FirstOrDefault(i => i.Group == group && i.Number == number);
        if (item is null)
        {
            // item not yet implemented
            return;
        }

        item.MaximumSockets = 3;
        item.PossibleItemOptions.Add(this._waterOptions!);
        item.PossibleItemOptions.Add(this._earthOptions!);
        item.PossibleItemOptions.Add(this._windOptions!);
        item.PossibleItemOptions.Add(this._bonusArmorOptions!);
    }

    private void AddWeaponSockets(byte group, short number, int socketCount)
    {
        var item = this.GameConfiguration.Items.FirstOrDefault(i => i.Group == group && i.Number == number);
        if (item is null)
        {
            // item not yet implemented
            return;
        }

        item.MaximumSockets = socketCount;
        item.PossibleItemOptions.Add(this._fireOptions!);
        item.PossibleItemOptions.Add(this._lightningOptions!);
        item.PossibleItemOptions.Add(this._iceOptions!);

        if (item.Name.Contains("Staff") || item.Name.Contains("Stick"))
        {
            item.PossibleItemOptions.Add(this._bonusWizOptions!);
        }
        else
        {
            item.PossibleItemOptions.Add(this._bonusPhysOptions!);
        }
    }

    private void CreateSocketOptions()
    {
        this._fireOptions = this.CreateFireOptions();
        this._iceOptions = this.CreateIceOptions();
        this._lightningOptions = this.CreateLightningOptions();
        this._windOptions = this.CreateWindOptions();
        this._waterOptions = this.CreateWaterOptions();
        this._earthOptions = this.CreateEarthOptions();
        this._bonusArmorOptions = this.CreateBonusOptionForArmorsOptionDefinition();
        this._bonusPhysOptions = this.CreateBonusOptionForPhysicalWeapons();
        this._bonusWizOptions = this.CreateBonusOptionForStaffs();
    }

    private ItemOptionDefinition CreateFireOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.SocketFire);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "Socket Options (Fire)";
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateRelatedSocketOption(0, SocketSubOptionType.Fire, Stats.MaximumPhysBaseDmg, Stats.Level, 1f / 20f, 1f / 19f, 1f / 18f, 1f / 17f, 1f / 14f));
        definition.PossibleOptions.Add(this.CreateSocketOption(1, SocketSubOptionType.Fire, Stats.AttackSpeed, AggregateType.AddRaw, 7, 8, 9, 10, 11));
        definition.PossibleOptions.Add(this.CreateSocketOption(2, SocketSubOptionType.Fire, Stats.BaseMaxDamageBonus, AggregateType.AddRaw, 30, 32, 35, 40, 50));
        definition.PossibleOptions.Add(this.CreateSocketOption(3, SocketSubOptionType.Fire, Stats.BaseMinDamageBonus, AggregateType.AddRaw, 20, 22, 25, 30, 35));
        definition.PossibleOptions.Add(this.CreateSocketOption(4, SocketSubOptionType.Fire, Stats.BaseDamageBonus, AggregateType.AddRaw, 20, 22, 25, 30, 35));
        definition.PossibleOptions.Add(this.CreateSocketOption(5, SocketSubOptionType.Fire, Stats.AbilityUsageReduction, AggregateType.AddRaw, 0.40f, 0.41f, 0.42f, 0.43f, 0.44f));
        return definition;
    }

    private ItemOptionDefinition CreateIceOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.SocketIce);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "Socket Options (Ice)";
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateSocketOption(0, SocketSubOptionType.Ice, Stats.HealthAfterMonsterKillMultiplier, AggregateType.AddRaw, 1f / 8f, 1f / 7f, 1f / 6f, 1f / 5f, 1f / 4f));
        definition.PossibleOptions.Add(this.CreateSocketOption(1, SocketSubOptionType.Ice, Stats.ManaAfterMonsterKillMultiplier, AggregateType.AddRaw, 1f / 8f, 1f / 7f, 1f / 6f, 1f / 5f, 1f / 4f));
        definition.PossibleOptions.Add(this.CreateSocketOption(2, SocketSubOptionType.Ice, Stats.SkillDamageBonus, AggregateType.AddRaw, 37, 40, 45, 50, 60));
        definition.PossibleOptions.Add(this.CreateSocketOption(3, SocketSubOptionType.Ice, Stats.AttackRatePvm, AggregateType.AddRaw, 25, 27, 30, 35, 40));
        definition.PossibleOptions.Add(this.CreateSocketOption(4, SocketSubOptionType.Ice, Stats.ItemDurationIncrease, AggregateType.Multiplicate, 1.30f, 1.32f, 1.34f, 1.36f, 1.38f));
        return definition;
    }

    private ItemOptionDefinition CreateLightningOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.SocketLightning);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "Socket Options (Lightning)";
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateSocketOption(0, SocketSubOptionType.Lightning, Stats.ExcellentDamageBonus, AggregateType.AddRaw, 15, 20, 25, 30, 40));
        definition.PossibleOptions.Add(this.CreateSocketOption(1, SocketSubOptionType.Lightning, Stats.ExcellentDamageChance, AggregateType.AddRaw, 0.10f, 0.11f, 0.12f, 0.13f, 0.14f));
        definition.PossibleOptions.Add(this.CreateSocketOption(2, SocketSubOptionType.Lightning, Stats.CriticalDamageBonus, AggregateType.AddRaw, 15, 20, 25, 30, 40));
        definition.PossibleOptions.Add(this.CreateSocketOption(3, SocketSubOptionType.Lightning, Stats.CriticalDamageChance, AggregateType.AddRaw, 0.08f, 0.09f, 0.10f, 0.11f, 0.12f));
        return definition;
    }

    private ItemOptionDefinition CreateWindOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.SocketWind);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "Socket Options (Wind)";
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateSocketOption(0, SocketSubOptionType.Wind, Stats.HealthRecoveryAbsolute, AggregateType.AddRaw, 8, 10, 13, 16, 20));
        definition.PossibleOptions.Add(this.CreateSocketOption(1, SocketSubOptionType.Wind, Stats.MaximumHealth, AggregateType.Multiplicate, 1.04f, 1.05f, 1.06f, 1.07f, 1.08f));
        definition.PossibleOptions.Add(this.CreateSocketOption(2, SocketSubOptionType.Wind, Stats.MaximumMana, AggregateType.Multiplicate, 1.04f, 1.05f, 1.06f, 1.07f, 1.08f));
        definition.PossibleOptions.Add(this.CreateSocketOption(3, SocketSubOptionType.Wind, Stats.ManaRecoveryAbsolute, AggregateType.AddRaw, 7, 14, 21, 28, 35));
        definition.PossibleOptions.Add(this.CreateSocketOption(4, SocketSubOptionType.Wind, Stats.MaximumAbility, AggregateType.AddRaw, 25, 30, 35, 40, 50));
        definition.PossibleOptions.Add(this.CreateSocketOption(5, SocketSubOptionType.Wind, Stats.AbilityRecoveryAbsolute, AggregateType.AddRaw, 3, 5, 7, 10, 15));
        return definition;
    }

    private ItemOptionDefinition CreateWaterOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.SocketWater);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "Socket Options (Water)";
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateSocketOption(0, SocketSubOptionType.Water, Stats.DefenseRatePvm, AggregateType.Multiplicate, 1.10f, 1.11f, 1.12f, 1.13f, 1.14f));
        definition.PossibleOptions.Add(this.CreateSocketOption(1, SocketSubOptionType.Water, Stats.DefenseBase, AggregateType.AddRaw, 30, 33, 36, 39, 42));
        definition.PossibleOptions.Add(this.CreateSocketOption(2, SocketSubOptionType.Water, Stats.ShieldDecreaseRateIncrease, AggregateType.AddRaw, 0.07f, 0.10f, 0.15f, 0.20f, 0.30f));
        definition.PossibleOptions.Add(this.CreateSocketOption(3, SocketSubOptionType.Water, Stats.DamageReceiveDecrement, AggregateType.Multiplicate, 0.96f, 0.95f, 0.94f, 0.93f, 0.92f));
        definition.PossibleOptions.Add(this.CreateSocketOption(4, SocketSubOptionType.Water, Stats.DamageReflection, AggregateType.AddRaw, 0.05f, 0.06f, 0.07f, 0.08f, 0.09f));
        return definition;
    }

    private ItemOptionDefinition CreateEarthOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.SocketEarth);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "Socket Options (Earth)";
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateSocketOption(0, SocketSubOptionType.Earth, Stats.MaximumHealth, AggregateType.AddRaw, 30, 32, 34, 36, 38));
        return definition;
    }

    private ItemOptionDefinition CreateBonusOptionForPhysicalWeapons()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.SocketBonus, 1);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "Socket Bonus Options (Physical)";
        definition.MaximumOptionsPerItem = 1;
        definition.AddChance = 0.30f;
        definition.PossibleOptions.Add(this.CreateSocketBonusOption(0, Stats.BaseDamageBonus, 11));
        definition.PossibleOptions.Add(this.CreateSocketBonusOption(1, Stats.SkillDamageBonus, 11));
        return definition;
    }

    private ItemOptionDefinition CreateBonusOptionForStaffs()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.SocketBonus, 2);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "Socket Bonus Options (Wizardry)";
        definition.MaximumOptionsPerItem = 1;
        definition.AddChance = 0.30f;
        definition.PossibleOptions.Add(this.CreateSocketBonusOption(2, Stats.BaseDamageBonus, 5));
        definition.PossibleOptions.Add(this.CreateSocketBonusOption(3, Stats.SkillDamageBonus, 11));
        return definition;
    }

    private ItemOptionDefinition CreateBonusOptionForArmorsOptionDefinition()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.SocketBonus, 3);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "Socket Bonus Options (Armors)";
        definition.MaximumOptionsPerItem = 1;
        definition.AddChance = 0.30f;
        definition.PossibleOptions.Add(this.CreateSocketBonusOption(4, Stats.DefenseBase, 24));
        definition.PossibleOptions.Add(this.CreateSocketBonusOption(5, Stats.MaximumHealth, 29));
        return definition;
    }

    private IncreasableItemOption CreateSocketBonusOption(short number, AttributeDefinition attributeDefinition, float value)
    {
        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(ItemOptionDefinitionNumbers.SocketBonus, number);
        itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.SocketBonusOption);
        itemOption.Number = number;

        var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        powerUpDefinition.TargetAttribute = attributeDefinition.GetPersistent(this.GameConfiguration);
        powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition.Boost.ConstantValue.Value = value;
        powerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;
        itemOption.PowerUpDefinition = powerUpDefinition;

        return itemOption;
    }

    private IncreasableItemOption CreateSocketOption(short number, SocketSubOptionType subOptionType, AttributeDefinition attributeDefinition, AggregateType aggregateType, params float[] values)
    {
        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid((short)(ItemOptionDefinitionNumbers.SocketFire + subOptionType), number);
        itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.SocketOption);
        itemOption.Number = number;
        itemOption.SubOptionType = (int)subOptionType;
        var level = 1;
        foreach (var value in values)
        {
            var optionOfLevel = this.Context.CreateNew<ItemOptionOfLevel>();
            optionOfLevel.Level = level++;
            var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
            powerUpDefinition.TargetAttribute = attributeDefinition.GetPersistent(this.GameConfiguration);
            powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            powerUpDefinition.Boost.ConstantValue.Value = value;
            powerUpDefinition.Boost.ConstantValue.AggregateType = aggregateType;
            optionOfLevel.PowerUpDefinition = powerUpDefinition;
            itemOption.LevelDependentOptions.Add(optionOfLevel);
            if (optionOfLevel.Level == 1)
            {
                itemOption.PowerUpDefinition = powerUpDefinition;
            }
        }

        return itemOption;
    }

    private IncreasableItemOption CreateRelatedSocketOption(short number, SocketSubOptionType subOptionType, AttributeDefinition targetAttribute, AttributeDefinition sourceAttribute, params float[] multipliers)
    {
        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid((short)(ItemOptionDefinitionNumbers.SocketFire + subOptionType), number);
        itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.SocketOption);
        itemOption.Number = number;
        itemOption.SubOptionType = (int)subOptionType;
        var level = 1;
        foreach (var multiplier in multipliers)
        {
            var optionOfLevel = this.Context.CreateNew<ItemOptionOfLevel>();
            optionOfLevel.Level = level++;
            var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
            powerUpDefinition.TargetAttribute = targetAttribute.GetPersistent(this.GameConfiguration);
            powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();

            var relationship = this.Context.CreateNew<AttributeRelationship>();
            powerUpDefinition.Boost.RelatedValues.Add(relationship);
            relationship.TargetAttribute = targetAttribute.GetPersistent(this.GameConfiguration);
            relationship.InputAttribute = sourceAttribute.GetPersistent(this.GameConfiguration);
            relationship.InputOperand = multiplier;
            relationship.InputOperator = InputOperator.Multiply;

            optionOfLevel.PowerUpDefinition = powerUpDefinition;
            itemOption.LevelDependentOptions.Add(optionOfLevel);
        }

        return itemOption;
    }

    private void CreateSeed(byte number, string name, ItemOptionDefinition options)
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = name;
        itemDefinition.Number = number;
        itemDefinition.Group = 12;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.DropLevel = 150;
        itemDefinition.MaximumItemLevel = (byte)options.PossibleOptions.Max(o => o.Number);
        itemDefinition.PossibleItemOptions.Add(options);
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        this.GameConfiguration.Items.Add(itemDefinition);
    }

    private void CreateSphere(byte number, string name, byte? dropLevel)
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = name;
        itemDefinition.Number = number;
        itemDefinition.Group = 12;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.DropLevel = dropLevel ?? 0;
        itemDefinition.DropsFromMonsters = dropLevel.HasValue;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        this.GameConfiguration.Items.Add(itemDefinition);
    }

    private void CreateSeedSphere(byte number, string name, byte level, ItemOptionDefinition options)
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = name;
        itemDefinition.Number = number;
        itemDefinition.Group = 12;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.DropLevel = level;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        itemDefinition.PossibleItemOptions.Add(options);
        this.GameConfiguration.Items.Add(itemDefinition);
    }

    private ItemCrafting SeedCrafting()
    {
        var crafting = this.Context.CreateNew<ItemCrafting>();
        crafting.Name = "Seed Creation";
        crafting.Number = 42;
        crafting.SetGuid(crafting.Number);
        var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
        crafting.SimpleCraftingSettings = craftingSettings;
        craftingSettings.SuccessPercent = 80;
        craftingSettings.MaximumSuccessPercent = 90;
        craftingSettings.Money = 1_000_000;
        craftingSettings.SetGuid(crafting.Number);

        var randomExcItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
        randomExcItem.MinimumAmount = 1;
        randomExcItem.MaximumAmount = 1;
        randomExcItem.MinimumItemLevel = 4;
        randomExcItem.MaximumItemLevel = 15;
        randomExcItem.NpcPriceDivisor = 2_000_000;
        randomExcItem.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Excellent));
        craftingSettings.RequiredItems.Add(randomExcItem);

        var randomAncientItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
        randomAncientItem.MinimumAmount = 1;
        randomAncientItem.MaximumAmount = 1;
        randomAncientItem.MinimumItemLevel = 4;
        randomAncientItem.MaximumItemLevel = 15;
        randomAncientItem.NpcPriceDivisor = 2_000_000;
        randomAncientItem.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.AncientBonus));
        craftingSettings.RequiredItems.Add(randomAncientItem);

        var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
        chaos.MinimumAmount = 1;
        chaos.MaximumAmount = 1;
        chaos.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos"));
        craftingSettings.RequiredItems.Add(chaos);

        var creation = this.Context.CreateNew<ItemCraftingRequiredItem>();
        creation.MinimumAmount = 1;
        creation.MaximumAmount = 1;
        creation.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Creation"));
        craftingSettings.RequiredItems.Add(creation);

        var harmony = this.Context.CreateNew<ItemCraftingRequiredItem>();
        harmony.MinimumAmount = 1;
        harmony.MaximumAmount = 1;
        harmony.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Harmony"));
        craftingSettings.RequiredItems.Add(harmony);

        craftingSettings.ResultItemSelect = ResultItemSelection.Any;
        foreach (var itemDefinition in this.GetSeeds())
        {
            var randomSeed = this.Context.CreateNew<ItemCraftingResultItem>();
            randomSeed.ItemDefinition = itemDefinition;
            randomSeed.RandomMaximumLevel = itemDefinition.MaximumItemLevel;
            craftingSettings.ResultItems.Add(randomSeed);
        }

        return crafting;
    }

    private ItemCrafting SeedSphereCrafting()
    {
        var crafting = this.Context.CreateNew<ItemCrafting>();
        crafting.Name = "Seed Sphere Creation";
        crafting.Number = 43;
        crafting.ItemCraftingHandlerClassName = typeof(SeedSphereCrafting).FullName!;
        crafting.SetGuid(crafting.Number);
        var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
        crafting.SimpleCraftingSettings = craftingSettings;
        craftingSettings.SetGuid(crafting.Number);
        craftingSettings.SuccessPercent = 80;
        craftingSettings.MaximumSuccessPercent = 90;
        craftingSettings.Money = 1_000_000;

        var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
        chaos.MinimumAmount = 1;
        chaos.MaximumAmount = 1;
        chaos.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos"));
        craftingSettings.RequiredItems.Add(chaos);

        var creation = this.Context.CreateNew<ItemCraftingRequiredItem>();
        creation.MinimumAmount = 1;
        creation.MaximumAmount = 1;
        creation.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Creation"));
        craftingSettings.RequiredItems.Add(creation);

        var seed = this.Context.CreateNew<ItemCraftingRequiredItem>();
        seed.MinimumAmount = 1;
        seed.MaximumAmount = 1;
        seed.MinimumItemLevel = 0;
        seed.MaximumItemLevel = 15;
        seed.NpcPriceDivisor = 200000;
        seed.Reference = OpenMU.GameLogic.PlayerActions.Craftings.SeedSphereCrafting.SeedReference;
        craftingSettings.RequiredItems.Add(seed);
        foreach (var itemDefinition in this.GetSeeds())
        {
            seed.PossibleItems.Add(itemDefinition);
        }

        var sphere = this.Context.CreateNew<ItemCraftingRequiredItem>();
        sphere.MinimumAmount = 1;
        sphere.MaximumAmount = 1;
        sphere.NpcPriceDivisor = 200000;
        sphere.Reference = OpenMU.GameLogic.PlayerActions.Craftings.SeedSphereCrafting.SphereReference;
        craftingSettings.RequiredItems.Add(sphere);
        foreach (var itemDefinition in this.GetSpheres())
        {
            sphere.PossibleItems.Add(itemDefinition);
        }

        return crafting;
    }

    private IEnumerable<ItemDefinition> GetSeeds()
    {
        for (int number = 60; number <= 65; number++)
        {
            if (this.GameConfiguration.Items.FirstOrDefault(item => item.Group == 12 && item.Number == number) is { } itemDefinition)
            {
                yield return itemDefinition;
            }
        }
    }

    private IEnumerable<ItemDefinition> GetSpheres()
    {
        for (int number = 70; number <= 75; number++)
        {
            if (this.GameConfiguration.Items.FirstOrDefault(item => item.Group == 12 && item.Number == number) is { } itemDefinition)
            {
                yield return itemDefinition;
            }
        }
    }

    private ItemCrafting MountSeedSphereCrafting()
    {
        var crafting = this.Context.CreateNew<ItemCrafting>();
        crafting.Name = "Mount Seed Sphere";
        crafting.Number = 44;
        crafting.ItemCraftingHandlerClassName = typeof(MountSeedSphereCrafting).FullName!;
        crafting.SetGuid(crafting.Number);
        var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
        crafting.SimpleCraftingSettings = craftingSettings;
        craftingSettings.SuccessPercent = 100;
        craftingSettings.Money = 1_000_000;
        craftingSettings.SetGuid(crafting.Number);

        var seedSphere = this.Context.CreateNew<ItemCraftingRequiredItem>();
        seedSphere.MinimumAmount = 1;
        seedSphere.MaximumAmount = 1;
        seedSphere.MinimumItemLevel = 0;
        seedSphere.MaximumItemLevel = 15;
        seedSphere.Reference = OpenMU.GameLogic.PlayerActions.Craftings.MountSeedSphereCrafting.SeedSphereReference;
        this.GameConfiguration.Items
            .Where(item => item.Group == 12)
            .Where(item => item.Number >= SeedSphereNumberStart)
            .Where(item => item.Number <= SeedSphereNumberEnd)
            .ForEach(seedSphere.PossibleItems.Add);
        craftingSettings.RequiredItems.Add(seedSphere);

        var socketItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
        socketItem.MinimumAmount = 1;
        socketItem.MaximumAmount = 1;
        socketItem.MinimumItemLevel = 0;
        socketItem.MaximumItemLevel = 15;
        socketItem.FailResult = MixResult.StaysAsIs;
        socketItem.SuccessResult = MixResult.StaysAsIs;
        socketItem.Reference = OpenMU.GameLogic.PlayerActions.Craftings.MountSeedSphereCrafting.SocketItemReference;
        this.GameConfiguration.Items.Where(item => item.MaximumSockets > 0).ForEach(socketItem.PossibleItems.Add);
        craftingSettings.RequiredItems.Add(socketItem);

        var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
        chaos.MinimumAmount = 1;
        chaos.MaximumAmount = 1;
        chaos.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos"));
        craftingSettings.RequiredItems.Add(chaos);

        var creation = this.Context.CreateNew<ItemCraftingRequiredItem>();
        creation.MinimumAmount = 1;
        creation.MaximumAmount = 1;
        creation.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Creation"));
        craftingSettings.RequiredItems.Add(creation);

        return crafting;
    }

    private ItemCrafting RemoveSeedSphereCrafting()
    {
        var crafting = this.Context.CreateNew<ItemCrafting>();
        crafting.Name = "Remove Seed Sphere";
        crafting.Number = 45;
        crafting.ItemCraftingHandlerClassName = typeof(RemoveSeedSphereCrafting).FullName!;
        crafting.SetGuid(crafting.Number);
        var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
        crafting.SimpleCraftingSettings = craftingSettings;
        craftingSettings.SuccessPercent = 100;
        craftingSettings.Money = 1_000_000;
        craftingSettings.SetGuid(crafting.Number);

        var socketItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
        socketItem.MinimumAmount = 1;
        socketItem.MaximumAmount = 1;
        socketItem.MinimumItemLevel = 0;
        socketItem.MaximumItemLevel = 15;
        socketItem.FailResult = MixResult.StaysAsIs;
        socketItem.SuccessResult = MixResult.StaysAsIs;
        socketItem.Reference = OpenMU.GameLogic.PlayerActions.Craftings.RemoveSeedSphereCrafting.SocketItemReference;
        this.GameConfiguration.Items.Where(item => item.MaximumSockets > 0).ForEach(socketItem.PossibleItems.Add);
        craftingSettings.RequiredItems.Add(socketItem);

        return crafting;
    }
}