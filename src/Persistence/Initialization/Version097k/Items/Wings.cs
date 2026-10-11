// <copyright file="Wings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097k.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

/// <summary>
/// Initializer for the wings of version 0.97k.
/// </summary>
/// <remarks>
/// Compared to version 0.95d, it adds the second wings and the Loch's Feather, which is required to craft them.
/// The values are the same as the ones of the second wings in <see cref="VersionSeasonSix.Items.Wings"/>.
/// Item option numbers of the second wings, which are transmitted to the client:
///             PDamage Recover WizDamage
/// Spirit       0x00    0x10
/// Soul                 0x00    0x10
/// Dragon       0x10    0x00
/// Darkness     0x10            0x00.
/// </remarks>
internal class Wings : Version095d.Items.Wings
{
    private ItemLevelBonusTable? _absorbByLevelTable;
    private ItemLevelBonusTable? _defenseBonusByLevelTable;
    private ItemLevelBonusTable? _damageIncreaseByLevelTable;

    /// <summary>
    /// Initializes a new instance of the <see cref="Wings"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Wings(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        // The second wings share the absorb and defense tables with the first wings.
        var wingsOfElf = this.GameConfiguration.Items.First(item => item is { Group: 12, Number: 0 });
        this._absorbByLevelTable = GetBonusTable(wingsOfElf, Stats.DamageReceiveDecrement);
        this._defenseBonusByLevelTable = GetBonusTable(wingsOfElf, Stats.DefenseBase);
        this._damageIncreaseByLevelTable = this.CreateDamageIncreaseBonusPerLevelSecondWings();

        var secondWingOptions = this.CreateSecondClassWingOptions();
        this.CreateSecondWing(3, 5, 3, LocalizedString.FromResource(() => ItemNames.WingsOfSpirits), 30, CharacterClasses.MuseElf, this.BuildOptions((0b10, OptionType.HealthRecover), (0b00, OptionType.PhysDamage)), secondWingOptions);
        this.CreateSecondWing(4, 5, 3, LocalizedString.FromResource(() => ItemNames.WingsOfSoul), 30, CharacterClasses.SoulMaster, this.BuildOptions((0b00, OptionType.HealthRecover), (0b10, OptionType.WizDamage)), secondWingOptions);
        this.CreateSecondWing(5, 3, 3, LocalizedString.FromResource(() => ItemNames.WingsOfDragon), 45, CharacterClasses.BladeKnight, this.BuildOptions((0b00, OptionType.HealthRecover), (0b10, OptionType.PhysDamage)), secondWingOptions, MovementSpeedConstants.FastWingMovementSpeed);
        this.CreateSecondWing(6, 4, 2, LocalizedString.FromResource(() => ItemNames.WingsOfDarkness), 40, CharacterClasses.MagicGladiator, this.BuildOptions((0b00, OptionType.WizDamage), (0b10, OptionType.PhysDamage)), secondWingOptions);

        this.CreateFeather();
    }

    private static ItemLevelBonusTable? GetBonusTable(ItemDefinition item, AttributeDefinition targetAttribute)
    {
        return item.BasePowerUpAttributes.FirstOrDefault(p => p.TargetAttribute == targetAttribute)?.BonusPerLevelTable;
    }

    private void CreateSecondWing(byte number, byte width, byte height, LocalizedString name, int defense, CharacterClasses qualifiedClasses, IEnumerable<IncreasableItemOption> possibleOptions, ItemOptionDefinition secondWingOptions, float movementSpeed = MovementSpeedConstants.DefaultWingMovementSpeed)
    {
        const byte dropLevel = 150;
        const byte durability = 200;
        const int levelRequirement = 215;
        const int damageIncreaseInitial = 32;
        const int damageAbsorbInitial = 25;

        var wing = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(wing);
        wing.Group = 12;
        wing.Number = number;
        wing.Width = width;
        wing.Height = height;
        wing.Name = name;
        wing.DropLevel = dropLevel;
        wing.MaximumItemLevel = (byte)this.MaximumItemLevel;
        wing.DropsFromMonsters = false;
        wing.Durability = durability;
        wing.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(st => st.ItemSlots.Contains(7));
        wing.SetGuid(wing.Group, wing.Number);
        this.CreateItemRequirementIfNeeded(wing, Stats.Level, levelRequirement);
        this.GameConfiguration.DetermineCharacterClasses(qualifiedClasses).ToList().ForEach(wing.QualifiedCharacters.Add);

        var defensePowerUp = this.CreateItemBasePowerUpDefinition(Stats.DefenseBase, defense, AggregateType.AddRaw);
        defensePowerUp.BonusPerLevelTable = this._defenseBonusByLevelTable;
        wing.BasePowerUpAttributes.Add(defensePowerUp);

        var absorbPowerUp = this.CreateItemBasePowerUpDefinition(Stats.DamageReceiveDecrement, 1f - (damageAbsorbInitial / 100f), AggregateType.Multiplicate);
        absorbPowerUp.BonusPerLevelTable = this._absorbByLevelTable;
        wing.BasePowerUpAttributes.Add(absorbPowerUp);

        var damagePowerUp = this.CreateItemBasePowerUpDefinition(Stats.AttackDamageIncrease, 1f + (damageIncreaseInitial / 100f), AggregateType.Multiplicate);
        damagePowerUp.BonusPerLevelTable = this._damageIncreaseByLevelTable;
        wing.BasePowerUpAttributes.Add(damagePowerUp);

        var canFlyPowerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
        canFlyPowerUp.TargetAttribute = Stats.CanFly.GetPersistent(this.GameConfiguration);
        canFlyPowerUp.BaseValue = 1;
        wing.BasePowerUpAttributes.Add(canFlyPowerUp);
        wing.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.MovementSpeed, movementSpeed, AggregateType.Maximum));
        wing.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.MovementSpeedUnderwater, movementSpeed, AggregateType.Maximum));

        wing.PossibleItemOptions.Add(secondWingOptions);

        var optionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
        optionDefinition.SetGuid(wing.GetItemId());
        this.GameConfiguration.ItemOptions.Add(optionDefinition);
        optionDefinition.Name = number switch
        {
            3 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfSpiritsOptions),
            4 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfSoulOptions),
            5 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfDragonOptions),
            _ => LocalizedString.FromResource(() => ItemOptionNames.WingsOfDarknessOptions),
        };
        optionDefinition.AddChance = 0.25f;
        optionDefinition.AddsRandomly = true;
        optionDefinition.MaximumOptionsPerItem = 1;
        wing.PossibleItemOptions.Add(optionDefinition);
        byte i = 0;
        foreach (var option in possibleOptions)
        {
            i++;
            option.SetGuid(ItemOptionDefinitionNumbers.WingDefense, wing.GetItemId(), i);
            optionDefinition.PossibleOptions.Add(option);
        }

        wing.PossibleItemOptions.Add(this.GameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o?.OptionType == ItemOptionTypes.Luck)));
    }

    /// <summary>
    /// Creates the options which a second wing can additionally have.
    /// They are transmitted to the client like excellent options.
    /// </summary>
    /// <returns>The created option definition.</returns>
    private ItemOptionDefinition CreateSecondClassWingOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.Wing2nd);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = LocalizedString.FromResource(() => ItemOptionNames.Level2ndWingOptions);
        definition.AddChance = 0.1f;
        definition.AddsRandomly = true;
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateWingOption(1, Stats.MaximumHealth, 50f, AggregateType.AddRaw, 5f));
        definition.PossibleOptions.Add(this.CreateWingOption(2, Stats.MaximumMana, 50f, AggregateType.AddRaw, 5f));
        definition.PossibleOptions.Add(this.CreateWingOption(3, Stats.DefenseIgnoreChance, 0.03f, AggregateType.AddRaw));

        return definition;
    }

    private IncreasableItemOption CreateWingOption(byte number, AttributeDefinition attributeDefinition, float value, AggregateType aggregateType, float? valueIncrementPerLevel = null)
    {
        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(ItemOptionDefinitionNumbers.Wing2nd, attributeDefinition.Id.ExtractFirstTwoBytes(), number);
        itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Wing);
        itemOption.Number = number;
        var targetAttribute = attributeDefinition.GetPersistent(this.GameConfiguration);
        itemOption.PowerUpDefinition = this.CreatePowerUpDefinition(targetAttribute, value, aggregateType);

        if (valueIncrementPerLevel.HasValue)
        {
            itemOption.LevelType = LevelType.ItemLevel;
            for (int level = 1; level <= this.MaximumItemLevel; level++)
            {
                var optionOfLevel = this.Context.CreateNew<ItemOptionOfLevel>();
                optionOfLevel.Level = level;
                optionOfLevel.PowerUpDefinition = this.CreatePowerUpDefinition(targetAttribute, value + (level * valueIncrementPerLevel.Value), aggregateType);
                itemOption.LevelDependentOptions.Add(optionOfLevel);
            }
        }

        return itemOption;
    }

    private void CreateFeather()
    {
        var feather = this.Context.CreateNew<ItemDefinition>();
        feather.Name = LocalizedString.FromResource(() => ItemNames.LochSFeather);
        feather.Number = 14;
        feather.Group = 13;
        feather.DropLevel = 78;
        feather.Width = 1;
        feather.Height = 2;
        feather.Durability = 1;
        feather.SetGuid(feather.Group, feather.Number);
        this.GameConfiguration.Items.Add(feather);
    }
}
