// <copyright file="Wings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

/// <summary>
/// Initializer for wing items.
/// </summary>
public class Wings : WingsInitializerBase
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
    protected override int MaximumItemLevel => Constants.MaximumItemLevel;

    /// <summary>
    /// Initializes all wings.
    /// </summary>
    public override void Initialize()
    {
        this._absorbByLevelTable = this.CreateAbsorbBonusPerLevel();
        this._defenseBonusByLevelTable = this.CreateBonusDefensePerLevel();
        this._damageIncreaseByLevelTable = this.CreateDamageIncreaseBonusPerLevelFirstAndThirdWings();

        this.CreateWing(0, 3, 2, "Wings of Elf", 100, 10, 200, 180, 0, 0, 1, this.BuildOptions((0, OptionType.HealthRecover)), 12, 12);
        this.CreateWing(1, 5, 3, "Wings of Heaven", 100, 10, 200, 180, 1, 0, 0, this.BuildOptions((0, OptionType.WizDamage)), 12, 12);
        this.CreateWing(2, 5, 2, "Wings of Satan", 100, 20, 200, 180, 0, 1, 0, this.BuildOptions((0, OptionType.PhysDamage)), 12, 12);
    }

    private ItemDefinition CreateWing(byte number, byte width, byte height, string name, byte dropLevel, int defense, byte durability, int levelRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, IEnumerable<IncreasableItemOption> possibleOptions, int damageIncreaseInitial, int damageAbsorbInitial)
    {
        var wing = this.CreateWing(number, width, height, name, dropLevel, defense, durability, levelRequirement, darkWizardClassLevel, darkKnightClassLevel, elfClassLevel);

        if (damageAbsorbInitial > 0)
        {
            var powerUp = this.CreateItemBasePowerUpDefinition(Stats.DamageReceiveDecrement, 1f - (damageAbsorbInitial / 100f), AggregateType.Multiplicate);
            powerUp.BonusPerLevelTable = this._absorbByLevelTable;
            wing.BasePowerUpAttributes.Add(powerUp);
        }

        if (damageIncreaseInitial > 0)
        {
            var powerUp = this.CreateItemBasePowerUpDefinition(Stats.AttackDamageIncrease, 1f + damageIncreaseInitial / 100f, AggregateType.Multiplicate);
            powerUp.BonusPerLevelTable = this._damageIncreaseByLevelTable;
            wing.BasePowerUpAttributes.Add(powerUp);
        }

        var optionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(optionDefinition);
        optionDefinition.Name = $"{name} Options";
        optionDefinition.AddChance = 0.25f;
        optionDefinition.AddsRandomly = true;
        optionDefinition.MaximumOptionsPerItem = 1;
        wing.PossibleItemOptions.Add(optionDefinition);
        foreach (var option in possibleOptions)
        {
            optionDefinition.PossibleOptions.Add(option);
        }

        wing.PossibleItemOptions.Add(this.GameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o?.OptionType == ItemOptionTypes.Luck)));
        return wing;
    }

    private ItemDefinition CreateWing(byte number, byte width, byte height, string name, byte dropLevel, int defense, byte durability, int levelRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel)
    {
        var wing = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(wing);
        wing.Group = 12;
        wing.Number = number;
        wing.Width = width;
        wing.Height = height;
        wing.Name = name;
        wing.DropLevel = dropLevel;
        wing.MaximumItemLevel = Constants.MaximumItemLevel;
        wing.DropsFromMonsters = false;
        wing.Durability = durability;
        wing.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(st => st.ItemSlots.Contains(7));
        wing.SetGuid(wing.Group, wing.Number);

        //// TODO: each level increases the requirement by 5 Levels
        this.CreateItemRequirementIfNeeded(wing, Stats.Level, levelRequirement);

        if (defense > 0)
        {
            var powerUp = this.CreateItemBasePowerUpDefinition(Stats.DefenseBase, defense, AggregateType.AddRaw);
            wing.BasePowerUpAttributes.Add(powerUp);
            powerUp.BonusPerLevelTable = this._defenseBonusByLevelTable;
        }

        var classes = this.GameConfiguration.DetermineCharacterClasses(darkWizardClassLevel == 1, darkKnightClassLevel == 1, elfClassLevel == 1);
        foreach (var characterClass in classes)
        {
            wing.QualifiedCharacters.Add(characterClass);
        }

        // add CanFly Attribute to all wings
        var canFlyPowerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
        canFlyPowerUp.TargetAttribute = Stats.CanFly.GetPersistent(this.GameConfiguration);
        canFlyPowerUp.BaseValue = 1;
        wing.BasePowerUpAttributes.Add(canFlyPowerUp);

        return wing;
    }
}