// <copyright file="AddMovementSpeedAttributesPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using AtlansMap = MUnique.OpenMU.Persistence.Initialization.Version075.Maps.Atlans;
using Doppelgaenger3Map = MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps.Doppelgaenger3;
using Kalima1Map = MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps.Kalima1;
using Kalima2Map = MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps.Kalima2;
using Kalima3Map = MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps.Kalima3;
using Kalima4Map = MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps.Kalima4;
using Kalima5Map = MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps.Kalima5;
using Kalima6Map = MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps.Kalima6;
using Kalima7Map = MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps.Kalima7;

/// <summary>
/// Adds movement speed attributes and configuration values.
/// </summary>
public abstract class AddMovementSpeedAttributesPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Add Movement Speed Attributes";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "Adds attribute-based movement speed configuration for players, monsters, items, effects, and underwater maps.";

    private const byte PetItemGroup = (byte)ItemGroups.Misc1;
    private const byte UniriaNumber = 2;
    private const byte DinorantNumber = 3;
    private const byte DarkHorseNumber = 4;
    private const byte WingsOfDragonNumber = 5;
    private const byte WingOfStormNumber = 36;
    private const byte FenrirNumber = 37;
    private const string RunningMovementSpeedTableName = "Running Movement Speed";
    private const int BlackFenrirMovementSpeedCombinationBonusNumber = 101;
    private const int BlackFenrirUnderwaterMovementSpeedCombinationBonusNumber = 102;
    private const int BlueFenrirMovementSpeedCombinationBonusNumber = 103;
    private const int BlueFenrirUnderwaterMovementSpeedCombinationBonusNumber = 104;
    private const int GoldFenrirMovementSpeedCombinationBonusNumber = 105;
    private const int GoldFenrirUnderwaterMovementSpeedCombinationBonusNumber = 106;

    private static readonly short[] UnderwaterMapNumbers =
    [
        AtlansMap.Number,
        Kalima1Map.Number,
        Kalima2Map.Number,
        Kalima3Map.Number,
        Kalima4Map.Number,
        Kalima5Map.Number,
        Kalima6Map.Number,
        Kalima7Map.Number,
        Doppelgaenger3Map.Number,
    ];

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 05, 15, 20, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Gets the maximum item level of the target game version.
    /// </summary>
    protected abstract int MaximumItemLevel { get; }

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        this.AddStatIfNotExists(context, gameConfiguration, Stats.MovementSpeed);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.MovementSpeedUnderwater);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.MovementSpeedFactor);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.IsUnderwater);

        this.AddGlobalMovementSpeedFactor(context, gameConfiguration);
        this.AddEffectMovementSpeedFactors(context, gameConfiguration);
        this.AddItemMovementSpeeds(context, gameConfiguration);
        this.AddUnderwaterMapPowerUps(context, gameConfiguration);
        return ValueTask.CompletedTask;
    }

    private static bool IsWingSlotItem(ItemDefinition item)
    {
        return item.ItemSlot?.ItemSlots.Contains(InventoryConstants.WingsSlot) ?? false;
    }

    private static float GetWingMovementSpeed(ItemDefinition wing)
    {
        return wing.Number is WingsOfDragonNumber or WingOfStormNumber
            ? MovementSpeedConstants.FastWingMovementSpeed
            : MovementSpeedConstants.DefaultWingMovementSpeed;
    }

    private static float GetPetMovementSpeed(ItemDefinition pet)
    {
        return pet.Number switch
        {
            UniriaNumber or DinorantNumber => MovementSpeedConstants.BasicMountMovementSpeed,
            DarkHorseNumber or FenrirNumber => MovementSpeedConstants.HorseOrFenrirMovementSpeed,
            _ => 0f,
        };
    }

    private void AddGlobalMovementSpeedFactor(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.GlobalBaseAttributeValues.Any(a => a.Definition?.Id == Stats.MovementSpeedFactor.Id))
        {
            return;
        }

        gameConfiguration.GlobalBaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(1f, Stats.MovementSpeedFactor.GetPersistent(gameConfiguration)));
    }

    private void AddEffectMovementSpeedFactors(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var icedEffect in gameConfiguration.MagicEffects.Where(e => e.Number == (short)MagicEffectNumber.Iced))
        {
            this.AddMovementSpeedFactorPowerUp(context, gameConfiguration, icedEffect, MovementSpeedConstants.IcedMovementSpeedFactor);
        }

        var coldEffect = gameConfiguration.MagicEffects.FirstOrDefault(e => e.Number == (short)MagicEffectNumber.Cold);
        if (coldEffect is null
            && gameConfiguration.Skills.Any(s => s.Number == (short)SkillNumber.StrikeofDestruction))
        {
            coldEffect = this.CreateEffect(context, gameConfiguration, ElementalType.Ice, MagicEffectNumber.Cold, Stats.IsIced, 10);
        }

        if (coldEffect is not null)
        {
            this.AddMovementSpeedFactorPowerUp(context, gameConfiguration, coldEffect, MovementSpeedConstants.ColdMovementSpeedFactor);
            foreach (var skill in gameConfiguration.Skills.Where(s => s.Number == (short)SkillNumber.StrikeofDestruction || s.Number == (short)SkillNumber.StrikeofDestrStr))
            {
                skill.MagicEffectDef = coldEffect;
            }
        }
    }

    private void AddMovementSpeedFactorPowerUp(IContext context, GameConfiguration gameConfiguration, MagicEffectDefinition magicEffect, float value)
    {
        if (magicEffect.PowerUpDefinitions.Any(p => p.TargetAttribute?.Id == Stats.MovementSpeedFactor.Id))
        {
            return;
        }

        magicEffect.PowerUpDefinitions.Add(this.CreatePowerUpDefinition(context, gameConfiguration, Stats.MovementSpeedFactor, value, AggregateType.Multiplicate));
    }

    private void AddItemMovementSpeeds(IContext context, GameConfiguration gameConfiguration)
    {
        var runningMovementSpeedTable = this.GetOrCreateRunningMovementSpeedTable(context, gameConfiguration);

        foreach (var boots in gameConfiguration.Items.Where(item => item.Group == (byte)ItemGroups.Boots))
        {
            this.AddItemBasePowerUp(context, gameConfiguration, boots, Stats.MovementSpeed, 0, AggregateType.Maximum, runningMovementSpeedTable);
        }

        foreach (var gloves in gameConfiguration.Items.Where(item => item.Group == (byte)ItemGroups.Gloves))
        {
            this.AddItemBasePowerUp(context, gameConfiguration, gloves, Stats.MovementSpeedUnderwater, 0, AggregateType.Maximum, runningMovementSpeedTable);
        }

        foreach (var wing in gameConfiguration.Items.Where(IsWingSlotItem))
        {
            this.AddMovementSpeedPowerUps(context, gameConfiguration, wing, GetWingMovementSpeed(wing));
        }

        foreach (var pet in gameConfiguration.Items.Where(item => item.Group == PetItemGroup))
        {
            var speed = GetPetMovementSpeed(pet);

            if (speed > 0)
            {
                this.AddMovementSpeedPowerUps(context, gameConfiguration, pet, speed);
            }
        }

        this.AddFenrirMovementSpeedCombinationBonuses(context, gameConfiguration);
    }

    private ItemLevelBonusTable GetOrCreateRunningMovementSpeedTable(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.ItemLevelBonusTables.FirstOrDefault(t => t.Name == RunningMovementSpeedTableName) is { } existingTable)
        {
            return existingTable;
        }

        var table = context.CreateNew<ItemLevelBonusTable>();
        gameConfiguration.ItemLevelBonusTables.Add(table);
        table.Name = RunningMovementSpeedTableName;
        table.Description = "Defines the running movement speed for boots and underwater gloves from item level 5.";
        for (int level = MovementSpeedConstants.RunningGearMinimumLevel; level <= this.MaximumItemLevel; level++)
        {
            var levelBonus = context.CreateNew<LevelBonus>();
            levelBonus.Level = level;
            levelBonus.AdditionalValue = MovementSpeedConstants.RunningGearMovementSpeed;
            table.BonusPerLevel.Add(levelBonus);
        }

        return table;
    }

    private void AddMovementSpeedPowerUps(IContext context, GameConfiguration gameConfiguration, ItemDefinition item, float speed)
    {
        this.AddItemBasePowerUp(context, gameConfiguration, item, Stats.MovementSpeed, speed, AggregateType.Maximum);
        this.AddItemBasePowerUp(context, gameConfiguration, item, Stats.MovementSpeedUnderwater, speed, AggregateType.Maximum);
    }

    private void AddItemBasePowerUp(
        IContext context,
        GameConfiguration gameConfiguration,
        ItemDefinition item,
        AttributeDefinition targetAttribute,
        float value,
        AggregateType aggregateType,
        ItemLevelBonusTable? bonusPerLevelTable = null)
    {
        if (item.BasePowerUpAttributes.Any(p => p.TargetAttribute?.Id == targetAttribute.Id))
        {
            return;
        }

        var powerUp = context.CreateNew<ItemBasePowerUpDefinition>();
        powerUp.TargetAttribute = targetAttribute.GetPersistent(gameConfiguration);
        powerUp.BaseValue = value;
        powerUp.AggregateType = aggregateType;
        powerUp.BonusPerLevelTable = bonusPerLevelTable;
        item.BasePowerUpAttributes.Add(powerUp);
    }

    private void AddFenrirMovementSpeedCombinationBonuses(IContext context, GameConfiguration gameConfiguration)
    {
        this.AddFenrirMovementSpeedCombinationBonus(context, gameConfiguration, ItemOptionTypes.BlackFenrir, BlackFenrirMovementSpeedCombinationBonusNumber, Stats.MovementSpeed);
        this.AddFenrirMovementSpeedCombinationBonus(context, gameConfiguration, ItemOptionTypes.BlackFenrir, BlackFenrirUnderwaterMovementSpeedCombinationBonusNumber, Stats.MovementSpeedUnderwater);
        this.AddFenrirMovementSpeedCombinationBonus(context, gameConfiguration, ItemOptionTypes.BlueFenrir, BlueFenrirMovementSpeedCombinationBonusNumber, Stats.MovementSpeed);
        this.AddFenrirMovementSpeedCombinationBonus(context, gameConfiguration, ItemOptionTypes.BlueFenrir, BlueFenrirUnderwaterMovementSpeedCombinationBonusNumber, Stats.MovementSpeedUnderwater);
        this.AddFenrirMovementSpeedCombinationBonus(context, gameConfiguration, ItemOptionTypes.GoldFenrir, GoldFenrirMovementSpeedCombinationBonusNumber, Stats.MovementSpeed);
        this.AddFenrirMovementSpeedCombinationBonus(context, gameConfiguration, ItemOptionTypes.GoldFenrir, GoldFenrirUnderwaterMovementSpeedCombinationBonusNumber, Stats.MovementSpeedUnderwater);
    }

    private void AddFenrirMovementSpeedCombinationBonus(
        IContext context,
        GameConfiguration gameConfiguration,
        ItemOptionType optionType,
        int number,
        AttributeDefinition targetAttribute)
    {
        if (gameConfiguration.ItemOptionTypes.FirstOrDefault(t => t == optionType) is not { } persistentOptionType)
        {
            return;
        }

        if (gameConfiguration.ItemOptionCombinationBonuses.Any(b =>
                b.Bonus?.TargetAttribute?.Id == targetAttribute.Id
                && b.Requirements.Any(r => r.OptionType == persistentOptionType)))
        {
            return;
        }

        var combinationBonus = context.CreateNew<ItemOptionCombinationBonus>();
        combinationBonus.Number = number;
        combinationBonus.Description = $"{persistentOptionType.Name}: {targetAttribute.Designation}";
        combinationBonus.AppliesMultipleTimes = false;
        combinationBonus.Requirements.Add(this.CreateFenrirMovementSpeedRequirement(context, persistentOptionType));
        combinationBonus.Bonus = this.CreatePowerUpDefinition(context, gameConfiguration, targetAttribute, MovementSpeedConstants.UpgradedFenrirMovementSpeed, AggregateType.Maximum);
        gameConfiguration.ItemOptionCombinationBonuses.Add(combinationBonus);
    }

    private CombinationBonusRequirement CreateFenrirMovementSpeedRequirement(IContext context, ItemOptionType optionType)
    {
        var requirement = context.CreateNew<CombinationBonusRequirement>();
        requirement.OptionType = optionType;
        requirement.MinimumCount = 1;
        return requirement;
    }

    private void AddUnderwaterMapPowerUps(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var map in gameConfiguration.Maps.Where(m => UnderwaterMapNumbers.Contains(m.Number)))
        {
            if (map.CharacterPowerUpDefinitions.Any(p => p.TargetAttribute?.Id == Stats.IsUnderwater.Id))
            {
                continue;
            }

            map.CharacterPowerUpDefinitions.Add(this.CreatePowerUpDefinition(context, gameConfiguration, Stats.IsUnderwater, 1, AggregateType.AddRaw));
        }
    }

    private PowerUpDefinition CreatePowerUpDefinition(IContext context, GameConfiguration gameConfiguration, AttributeDefinition targetAttribute, float value, AggregateType aggregateType)
    {
        var powerUp = context.CreateNew<PowerUpDefinition>();
        powerUp.TargetAttribute = targetAttribute.GetPersistent(gameConfiguration);
        powerUp.Boost = context.CreateNew<PowerUpDefinitionValue>();
        powerUp.Boost.ConstantValue.Value = value;
        powerUp.Boost.ConstantValue.AggregateType = aggregateType;
        return powerUp;
    }

    private MagicEffectDefinition CreateEffect(IContext context, GameConfiguration gameConfiguration, ElementalType type, MagicEffectNumber effectNumber, AttributeDefinition targetAttribute, float durationInSeconds, float chance = 0)
    {
        if (gameConfiguration.MagicEffects.FirstOrDefault(
                e => e.Number == (short)effectNumber
                     && e.SubType == (byte)(0xFF - type)
                     && Equals(e.Duration?.ConstantValue.Value, durationInSeconds)
                     && Equals(e.Chance?.ConstantValue.Value, chance)
                     && e.PowerUpDefinitions.FirstOrDefault()?.TargetAttribute == targetAttribute) is { } existingEffect)
        {
            return existingEffect;
        }

        var effect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(effect);
        effect.Name = Enum.GetName(effectNumber) ?? string.Empty;
        effect.InformObservers = true;
        effect.Number = (short)effectNumber;
        effect.StopByDeath = true;
        effect.SubType = (byte)(0xFF - type);
        effect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        effect.Duration.ConstantValue.Value = durationInSeconds;
        var powerUpDefinition = context.CreateNew<PowerUpDefinition>();
        effect.PowerUpDefinitions.Add(powerUpDefinition);
        powerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition.Boost.ConstantValue.Value = 1;
        powerUpDefinition.TargetAttribute = targetAttribute.GetPersistent(gameConfiguration);
        if (targetAttribute == Stats.IsIced)
        {
            var movementSpeedFactorPowerUp = context.CreateNew<PowerUpDefinition>();
            effect.PowerUpDefinitions.Add(movementSpeedFactorPowerUp);
            movementSpeedFactorPowerUp.Boost = context.CreateNew<PowerUpDefinitionValue>();
            movementSpeedFactorPowerUp.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
            movementSpeedFactorPowerUp.TargetAttribute = Stats.MovementSpeedFactor.GetPersistent(gameConfiguration);

            if (effectNumber == MagicEffectNumber.Cold)
            {
                movementSpeedFactorPowerUp.Boost.ConstantValue.Value = MovementSpeedConstants.ColdMovementSpeedFactor;
            }
            else
            {
                movementSpeedFactorPowerUp.Boost.ConstantValue.Value = MovementSpeedConstants.IcedMovementSpeedFactor;
            }
        }

        if (chance > 0)
        {
            effect.Chance = context.CreateNew<PowerUpDefinitionValue>();
            effect.Chance.ConstantValue.Value = chance;
        }

        return effect;
    }
}
