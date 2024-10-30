// <copyright file="FixAttackSpeedCalculationUpdate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Network.Packets;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;
using static CharacterClasses.CharacterClassHelper;

/// <summary>
/// This adds attributes and relations for attack speed. Adds effects for Ale and Potion of Soul
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("F9977AA7-F52A-4F42-BD6C-98DE700B5980")]
public class FixAttackSpeedCalculationUpdate : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Attack Speed Calculation";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Adds attributes and relations for attack speed. Adds effects for Ale and Potion of Soul.";

    private static readonly Dictionary<int, int> AttackSpeedByGloveNumber = new()
    {
        {0, 4}, // Bronze Gloves
        {1, 6}, // Dragon Gloves
        {5, 8}, // Leather Gloves
        {6, 10}, // Scale Gloves
        {8, 8}, // Brass Gloves
        {9, 4}, // Plate Gloves
        {10, 4}, // Vine Gloves
        {11, 8}, // Silk Gloves
        {12, 10}, // Wind Gloves
        {13, 4}, // Spirit Gloves
        {14, 6}, // Guardian Gloves
        {15, 6}, // Storm Crow Gloves
        {16, 6}, // Black Dragon Gloves
        {17, 6}, // Dark Phoenix Gloves
        {18, 5}, // Grand Soul Gloves
        {19, 6}, // Divine Gloves
        {20, 7}, // Thunder Hawk Gloves
        {21, 6}, // Great Dragon Gloves
        {22, 6}, // Dark Soul Gloves
        {23, 7}, // Hurricane Gloves
        {24, 6}, // Red Spirit Gloves
        {25, 7}, // Light Plate Gloves
        {26, 6}, // Adamantine Gloves
        {27, 5}, // Dark Steel Gloves
        {28, 4}, // Dark Master Gloves
        {29, 7}, // Dragon Knight Gloves
        {30, 7}, // Venom Mist Gloves
        {31, 7}, // Sylphid Ray Gloves
        {32, 7}, // Volcano Gloves
        {33, 5}, // Sunlight Gloves
        {34, 6}, // Ashcrow Gloves
        {35, 6}, // Eclipse Gloves
        {36, 6}, // Iris Gloves
        {37, 7}, // Valiant Gloves
        {38, 5}, // Glorious Gloves
        {39, 6}, // Violent Wind Gloves
        {40, 8}, // Red Wing Gloves
        {41, 7}, // Ancient Gloves
        {42, 6}, // Demonic Gloves
        {43, 6}, // Storm Blitz Gloves
        {45, 7}, // Titan  Gloves
        {46, 7}, // Brave Gloves
        {47, 7}, // Phantom  Gloves
        {48, 7}, // Destroy Gloves
        {49, 7}, // Seraphim Gloves
        {50, 7}, // Divine Gloves
        {51, 7}, // Royal Gloves
        {52, 7}, // Hades Gloves
    };

    private static readonly Dictionary<int, int> WalkSpeedByBootNumber = new()
    {
        { 0, 10 }, // Bronze Boots
        { 1, 2 }, // Dragon Boots
        { 2, 10 }, // Pad Boots
        { 3, 0 }, // Legendary Boots
        { 4, 6 }, // Bone Boots
        { 5, 12 }, // Leather Boots
        { 6, 8 }, // Scale Boots
        { 7, 8 }, // Sphinx Boots
        { 8, 6 }, // Brass Boots
        { 9, 4 }, // Plate Boots
        { 15, 2 }, // Storm Crow Boots
        { 16, 2 }, // Black Dragon Boots
        { 17, 2 }, // Dark Phoenix Boots
        { 20, 2 }, // Thunder Hawk Boots
    };

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixAttackSpeedCalculation;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 10, 19, 14, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Attributes.Contains(Stats.MagicSpeed))
        {
            return;
        }

        AddStatIfNotExists(context, gameConfiguration, Stats.MagicSpeed);
        AddStatIfNotExists(context, gameConfiguration, Stats.AttackSpeedByWeapon);
        AddStatIfNotExists(context, gameConfiguration, Stats.AreTwoWeaponsEquipped);
        AddStatIfNotExists(context, gameConfiguration, Stats.EquippedWeaponCount);
        AddStatIfNotExists(context, gameConfiguration, Stats.WalkSpeed);

        Stats.AttackSpeed.GetPersistent(gameConfiguration).MaximumValue = Stats.AttackSpeed.MaximumValue;

        foreach (var characterClass in gameConfiguration.CharacterClasses)
        {
            var attributeRelationships = characterClass.AttributeCombinations;
            attributeRelationships.Add(CreateAttributeRelationship(context, gameConfiguration, Stats.AttackSpeed, 1, Stats.AttackSpeedByWeapon));
            attributeRelationships.Add(CreateAttributeRelationship(context, gameConfiguration, Stats.MagicSpeed, 1, Stats.AttackSpeedByWeapon));

            // If two weapons are equipped we subtract the half of the sum of the speeds again from the attack speed
            attributeRelationships.Add(CreateAttributeRelationship(context, gameConfiguration, Stats.AreTwoWeaponsEquipped, 1, Stats.EquippedWeaponCount));
            var tempSpeed = context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Temp Half weapon attack speed", string.Empty);
            gameConfiguration.Attributes.Add(tempSpeed);
            attributeRelationships.Add(CreateAttributeRelationship(context, gameConfiguration, tempSpeed, -0.5f, Stats.AttackSpeedByWeapon));
            attributeRelationships.Add(CreateConditionalRelationship(context, gameConfiguration, Stats.AttackSpeed, Stats.AreTwoWeaponsEquipped, tempSpeed));
            attributeRelationships.Add(CreateConditionalRelationship(context, gameConfiguration, Stats.MagicSpeed, Stats.AreTwoWeaponsEquipped, tempSpeed));

            characterClass.BaseAttributeValues.Add(CreateConstValueAttribute(context, gameConfiguration, -1, Stats.AreTwoWeaponsEquipped));
        }

        foreach (var darkKnight in gameConfiguration.CharacterClasses.Where(c => ((CharacterClassNumber)c.Number) is CharacterClassNumber.BladeKnight or CharacterClassNumber.BladeMaster or CharacterClassNumber.DarkKnight))
        {
            darkKnight.AttributeCombinations.Add(CreateAttributeRelationship(context, gameConfiguration, Stats.MagicSpeed, 1.0f / 20, Stats.TotalAgility));
        }

        foreach (var darkLord in gameConfiguration.CharacterClasses.Where(c => ((CharacterClassNumber)c.Number) is CharacterClassNumber.DarkLord or CharacterClassNumber.LordEmperor))
        {
            darkLord.AttributeCombinations.Add(CreateAttributeRelationship(context, gameConfiguration, Stats.AttackSpeed, 1.0f / 10, Stats.TotalAgility));
            darkLord.AttributeCombinations.Add(CreateAttributeRelationship(context, gameConfiguration, Stats.MagicSpeed, 1.0f / 10, Stats.TotalAgility));
        }

        foreach (var darkWizard in gameConfiguration.CharacterClasses.Where(c => ((CharacterClassNumber)c.Number) is CharacterClassNumber.DarkWizard or CharacterClassNumber.SoulMaster or CharacterClassNumber.GrandMaster))
        {
            darkWizard.AttributeCombinations.Add(CreateAttributeRelationship(context, gameConfiguration, Stats.AttackSpeed, 1.0f / 20, Stats.TotalAgility));
            darkWizard.AttributeCombinations.Add(CreateAttributeRelationship(context, gameConfiguration, Stats.MagicSpeed, 1.0f / 10, Stats.TotalAgility));
        }

        foreach (var elf in gameConfiguration.CharacterClasses.Where(c => ((CharacterClassNumber)c.Number) is CharacterClassNumber.FairyElf or CharacterClassNumber.MuseElf or CharacterClassNumber.HighElf))
        {
            elf.AttributeCombinations.Add(CreateAttributeRelationship(context, gameConfiguration, Stats.AttackSpeed, 1.0f / 50, Stats.TotalAgility));
            elf.AttributeCombinations.Add(CreateAttributeRelationship(context, gameConfiguration, Stats.MagicSpeed, 1.0f / 50, Stats.TotalAgility));
        }

        foreach (var magicGladiator in gameConfiguration.CharacterClasses.Where(c => ((CharacterClassNumber)c.Number) is CharacterClassNumber.MagicGladiator or CharacterClassNumber.DuelMaster))
        {
            magicGladiator.AttributeCombinations.Add(CreateAttributeRelationship(context, gameConfiguration, Stats.MagicSpeed, 1.0f / 20, Stats.TotalAgility));
        }

        foreach (var rageFighter in gameConfiguration.CharacterClasses.Where(c => ((CharacterClassNumber)c.Number) is CharacterClassNumber.RageFighter or CharacterClassNumber.FistMaster))
        {
            rageFighter.AttributeCombinations.Add(CreateAttributeRelationship(context, gameConfiguration, Stats.AttackSpeed, 1.0f / 9, Stats.TotalAgility));
            rageFighter.AttributeCombinations.Add(CreateAttributeRelationship(context, gameConfiguration, Stats.MagicSpeed, 1.0f / 9, Stats.TotalAgility));
        }

        foreach (var summoner in gameConfiguration.CharacterClasses.Where(c => ((CharacterClassNumber)c.Number) is CharacterClassNumber.Summoner or CharacterClassNumber.BloodySummoner or CharacterClassNumber.DimensionMaster))
        {
            summoner.AttributeCombinations.Add(CreateAttributeRelationship(context, gameConfiguration, Stats.MagicSpeed, 1.0f / 20, Stats.TotalAgility));
        }

        // attack speed from gloves:
        var glovesWithSpeed = gameConfiguration.Items.Where(item => item.Group == (byte)ItemGroups.Gloves)
            .Select(item => (item, AttackSpeedByGloveNumber.GetValueOrDefault(item.Number)))
            .Where(pair => pair.Item2 > 0);
        foreach (var (gloves, attackSpeed) in glovesWithSpeed)
        {
            gloves.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(context, gameConfiguration, Stats.AttackSpeed, attackSpeed, AggregateType.AddRaw));
            gloves.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(context, gameConfiguration, Stats.MagicSpeed, attackSpeed, AggregateType.AddRaw));
        }

        // walk speed from boots:
        var bootsWithSpeed = gameConfiguration.Items.Where(item => item.Group == (byte)ItemGroups.Boots)
            .Select(item => (item, WalkSpeedByBootNumber.GetValueOrDefault(item.Number)))
            .Where(pair => pair.Item2 > 0);
        foreach (var (boots, walkSpeed) in bootsWithSpeed)
        {
            boots.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(context, gameConfiguration, Stats.WalkSpeed, walkSpeed, AggregateType.AddRaw));
        }

        new AlcoholEffectInitializer(context, gameConfiguration).Initialize();
        new BlessPotionEffectInitializer(context, gameConfiguration).Initialize();
        new SoulPotionEffectInitializer(context, gameConfiguration).Initialize();

        SetItemEffect(gameConfiguration, ItemConstants.Alcohol, MagicEffectNumber.Alcohol);
        var siegePotion = gameConfiguration.Items.First(item => item.Number == ItemConstants.SiegePotion.Number && item.Group == ItemConstants.SiegePotion.Group);
        siegePotion.Name = "Potion of Bless;Potion of Soul";
        siegePotion.Durability = 10;
        siegePotion.MaximumItemLevel = 1;

        var jackOlanternBlessingEffect = gameConfiguration.MagicEffects.First(item => item.Number == (int)MagicEffectNumber.JackOlanternBlessing);
        var powerUpDefinition = context.CreateNew<PowerUpDefinition>();
        jackOlanternBlessingEffect.PowerUpDefinitions.Add(powerUpDefinition);
        powerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition.Boost.ConstantValue.Value = 10;
        powerUpDefinition.TargetAttribute = Stats.MagicSpeed.GetPersistent(gameConfiguration);
    }

    private void SetItemEffect(GameConfiguration gameConfiguration, ItemIdentifier itemIdentifier, MagicEffectNumber effectNumber)
    {
        var item = gameConfiguration.Items.First(i => i.Number == itemIdentifier.Number && i.Group == itemIdentifier.Group);
        item.ConsumeEffect = gameConfiguration.MagicEffects.First(effect => effect.Number == (int)effectNumber);
    }

    private ItemBasePowerUpDefinition CreateItemBasePowerUpDefinition(IContext context, GameConfiguration gameConfiguration, AttributeDefinition attributeDefinition, float value, AggregateType aggregateType)
    {
        var powerUpDefinition = context.CreateNew<ItemBasePowerUpDefinition>();
        powerUpDefinition.TargetAttribute = attributeDefinition.GetPersistent(gameConfiguration);
        powerUpDefinition.BaseValue = value;
        powerUpDefinition.AggregateType = aggregateType;
        return powerUpDefinition;
    }
}