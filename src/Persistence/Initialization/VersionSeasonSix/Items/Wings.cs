// <copyright file="Wings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

using MUnique.OpenMU.Persistence.Initialization;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

/// <summary>
/// Initializer for wing items.
/// </summary>
public class Wings : WingsInitializerBase
{
    private ItemLevelBonusTable? _absorbByLevelTable;
    private ItemLevelBonusTable? _defenseBonusByLevelTable;
    private ItemLevelBonusTable? _damageIncreaseByLevelTable;
    private ItemLevelBonusTable? _damageIncreaseByLevelTableSecond;
    private ItemLevelBonusTable? _defenseBonusByLevelTableThird;

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
    protected override int MaximumItemLevel => 15;

    /// <summary>
    /// Initializes all wings.
    /// </summary>
    /// <remarks>
    /// Regex: (?m)^\s*(\d+)\s+(-*\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+\"(.+?)\"\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+).*$
    /// Replace by: this.CreateWing($1, $4, $5, "$9", $10, $11, $12, $13, $19, $20, $21, $22, $23, $24, $25);
    /// Sources for stats not provided by the Item.txt:
    /// http://muonlinefanz.com/tools/items/
    /// https://wiki.infinitymu.net/index.php?title=2nd_Level_Wing
    /// https://wiki.infinitymu.net/index.php?title=3rd_Level_Wing
    /// http://www.guiamuonline.com/items-de-mu-online/wings
    /// Item option numbers:
    /// 3rd wings:
    /// 0x11 (3) -> Damage
    /// 0x00 (0) -> Recover
    /// 0x10 (2) -> Defense (for Ruin it's WizDamage)
    /// 2nd wings:
    ///             PDamage Recover WizDamage   CurseDmg
    /// Spirit       0x00    0x10
    /// Soul                 0x00    0x10
    /// Dragon       0x10    0x00
    /// Darkness     0x10            0x00
    /// Warrior Cl   0x10    0x00
    /// Despair              0x00               0x10.
    /// </remarks>
    public override void Initialize()
    {
        this._absorbByLevelTable = this.CreateAbsorbBonusPerLevel();
        this._defenseBonusByLevelTable = this.CreateBonusDefensePerLevel();
        this._defenseBonusByLevelTableThird = this.CreateBonusDefensePerLevelThirdWings();
        this._damageIncreaseByLevelTable = this.CreateDamageIncreaseBonusPerLevelFirstAndThirdWings();
        this._damageIncreaseByLevelTableSecond = this.CreateDamageIncreaseBonusPerLevelSecondWings();

        // First class wings:
        this.CreateWing(0, 3, 2, "Wings of Elf", 100, 10, 200, 180, 0, 0, 1, 0, 0, 0, 0, this.BuildOptions((0, OptionType.HealthRecover)), 12, 12, this._damageIncreaseByLevelTable, null);
        this.CreateWing(1, 5, 3, "Wings of Heaven", 100, 10, 200, 180, 1, 0, 0, 1, 0, 0, 0, this.BuildOptions((0, OptionType.WizDamage)), 12, 12, this._damageIncreaseByLevelTable, null);
        this.CreateWing(2, 5, 2, "Wings of Satan", 100, 20, 200, 180, 0, 1, 0, 1, 0, 0, 0, this.BuildOptions((0, OptionType.PhysDamage)), 12, 12, this._damageIncreaseByLevelTable, null);
        this.CreateWing(41, 4, 2, "Wing of Curse", 100, 10, 200, 180, 0, 0, 0, 0, 0, 1, 0, this.BuildOptions((0, OptionType.CurseDamage)), 12, 12, this._damageIncreaseByLevelTable, null);

        // Second class wings:
        var secondWingOptions = this.CreateSecondClassWingOptions();
        this.CreateWing(3, 5, 3, "Wings of Spirits", 150, 30, 200, 215, 0, 0, 2, 0, 0, 0, 0, this.BuildOptions((0b10, OptionType.HealthRecover), (0b00, OptionType.PhysDamage)), 32, 25, this._damageIncreaseByLevelTableSecond, secondWingOptions);
        this.CreateWing(4, 5, 3, "Wings of Soul", 150, 30, 200, 215, 2, 0, 0, 0, 0, 0, 0, this.BuildOptions((0b00, OptionType.HealthRecover), (0b10, OptionType.WizDamage)), 32, 25, this._damageIncreaseByLevelTableSecond, secondWingOptions);
        this.CreateWing(5, 3, 3, "Wings of Dragon", 150, 45, 200, 215, 0, 2, 0, 0, 0, 0, 0, this.BuildOptions((0b00, OptionType.HealthRecover), (0b10, OptionType.PhysDamage)), 32, 25, this._damageIncreaseByLevelTableSecond, secondWingOptions);
        this.CreateWing(6, 4, 2, "Wings of Darkness", 150, 40, 200, 215, 0, 0, 0, 1, 0, 0, 0, this.BuildOptions((0b00, OptionType.WizDamage), (0b10, OptionType.PhysDamage)), 32, 25, this._damageIncreaseByLevelTableSecond, secondWingOptions);
        this.CreateWing(42, 4, 3, "Wings of Despair", 150, 30, 200, 215, 0, 0, 0, 0, 0, 2, 0, this.BuildOptions((0b00, OptionType.HealthRecover), (0b10, OptionType.CurseDamage)), 32, 25, this._damageIncreaseByLevelTableSecond, secondWingOptions);

        // The capes are a bit of a hybrid. Their damage gets increased like first wings, but they start slightly lower than 2nd wings.
        this.CreateWing(49, 2, 3, "Cape of Fighter", 180, 15, 200, 180, 0, 0, 0, 0, 0, 0, 1, this.BuildOptions((0b00, OptionType.HealthRecover), (0b10, OptionType.PhysDamage)), 20, 10, this._damageIncreaseByLevelTable, secondWingOptions);
        var capeOfLord = this.CreateWing(30, 2, 3, "Cape of Lord", 180, 15, 200, 180, 0, 0, 0, 0, 1, 0, 0, this.BuildOptions((0b00, OptionType.PhysDamage)), 20, 10, this._damageIncreaseByLevelTable, this.CreateCapeOptions());
        capeOfLord.Group = 13;
        capeOfLord.SetGuid(capeOfLord.Group, capeOfLord.Number);

        this.CreateFeather();
        this.CreateFeatherOfCondor();
        this.CreateFlameOfCondor();

        // Third class wings:
        var thirdWingOptions = this.CreateThirdClassWingOptions();
        this.CreateWing(36, 4, 3, "Wing of Storm", 150, 60, 220, 300, 0, 3, 0, 0, 0, 0, 0, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.Defense)), 39, 39, this._damageIncreaseByLevelTable, thirdWingOptions);
        this.CreateWing(37, 4, 3, "Wing of Eternal", 150, 45, 220, 300, 3, 0, 0, 0, 0, 0, 0, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.WizDamage), (0b10, OptionType.Defense)), 39, 39, this._damageIncreaseByLevelTable, thirdWingOptions);
        this.CreateWing(38, 4, 3, "Wing of Illusion", 150, 45, 220, 300, 0, 0, 3, 0, 0, 0, 0, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.Defense)), 39, 39, this._damageIncreaseByLevelTable, thirdWingOptions);
        this.CreateWing(39, 4, 3, "Wing of Ruin", 150, 55, 220, 300, 0, 0, 0, 3, 0, 0, 0, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.WizDamage)), 39, 39, this._damageIncreaseByLevelTable, thirdWingOptions);
        this.CreateWing(40, 2, 3, "Cape of Emperor", 150, 45, 220, 300, 0, 0, 0, 0, 3, 0, 0, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.Defense)), 39, 39, this._damageIncreaseByLevelTable, thirdWingOptions);
        this.CreateWing(43, 4, 3, "Wing of Dimension", 150, 45, 220, 300, 0, 0, 0, 0, 0, 3, 0, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.CurseDamage), (0b10, OptionType.Defense)), 42, 24, this._damageIncreaseByLevelTable, thirdWingOptions);
        this.CreateWing(50, 2, 3, "Cape of Overrule", 150, 45, 220, 300, 0, 0, 0, 0, 0, 0, 3, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.Defense)), 42, 24, this._damageIncreaseByLevelTable, thirdWingOptions);
    }

    private void CreateFeather()
    {
        var feather = this.Context.CreateNew<ItemDefinition>();
        feather.Name = "Loch's Feather";
        feather.MaximumItemLevel = 1;
        feather.Number = 14;
        feather.Group = 13;
        feather.DropLevel = 78;
        feather.Width = 1;
        feather.Height = 2;
        feather.Durability = 1;
        feather.SetGuid(feather.Group, feather.Number);
        this.GameConfiguration.Items.Add(feather);
    }

    private void CreateFeatherOfCondor()
    {
        var feather = this.Context.CreateNew<ItemDefinition>();
        feather.Name = "Feather of Condor";
        feather.MaximumItemLevel = 1;
        feather.Number = 53;
        feather.Group = 13;
        feather.DropLevel = 120;
        feather.Width = 1;
        feather.Height = 2;
        feather.Durability = 1;
        feather.SetGuid(feather.Group, feather.Number);
        this.GameConfiguration.Items.Add(feather);
    }

    private void CreateFlameOfCondor()
    {
        var feather = this.Context.CreateNew<ItemDefinition>();
        feather.Name = "Flame of Condor";
        feather.MaximumItemLevel = 1;
        feather.Number = 52;
        feather.Group = 13;
        feather.DropLevel = 120;
        feather.Width = 1;
        feather.Height = 2;
        feather.Durability = 1;
        feather.SetGuid(feather.Group, feather.Number);
        this.GameConfiguration.Items.Add(feather);
    }

    private ItemDefinition CreateWing(byte number, byte width, byte height, string name, byte dropLevel, int defense, byte durability, int levelRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, int magicGladiatorClassLevel, int darkLordClassLevel, int summonerClassLevel, int ragefighterClassLevel, IEnumerable<IncreasableItemOption> possibleOptions, int damageIncreaseInitial, int damageAbsorbInitial, ItemLevelBonusTable damageIncreasePerLevel, ItemOptionDefinition? wingOptionDefinition)
    {
        var wing = this.CreateWing(number, width, height, name, dropLevel, defense, durability, levelRequirement, darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, magicGladiatorClassLevel, darkLordClassLevel, summonerClassLevel, ragefighterClassLevel);
        if (wingOptionDefinition != null)
        {
            wing.PossibleItemOptions.Add(wingOptionDefinition);
        }

        if (damageAbsorbInitial > 0)
        {
            var powerUp = this.CreateItemBasePowerUpDefinition(Stats.DamageReceiveDecrement, 1f - (damageAbsorbInitial / 100f), AggregateType.Multiplicate);
            powerUp.BonusPerLevelTable = this._absorbByLevelTable;
            wing.BasePowerUpAttributes.Add(powerUp);
        }

        if (damageIncreaseInitial > 0)
        {
            var powerUp = this.CreateItemBasePowerUpDefinition(Stats.AttackDamageIncrease, 1f + damageIncreaseInitial / 100f, AggregateType.Multiplicate);
            powerUp.BonusPerLevelTable = damageIncreasePerLevel;
            wing.BasePowerUpAttributes.Add(powerUp);
        }

        var optionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
        optionDefinition.SetGuid(wing.GetItemId());
        this.GameConfiguration.ItemOptions.Add(optionDefinition);

        optionDefinition.Name = $"{name} Options";
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
        return wing;
    }

    private ItemDefinition CreateWing(byte number, byte width, byte height, string name, byte dropLevel, int defense, byte durability, int levelRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, int magicGladiatorClassLevel, int darkLordClassLevel, int summonerClassLevel, int ragefighterClassLevel)
    {
        var wing = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(wing);
        wing.Group = 12;
        wing.Number = number;
        wing.Width = width;
        wing.Height = height;
        wing.Name = name;
        wing.DropLevel = dropLevel;
        wing.MaximumItemLevel = 15;
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
            powerUp.BonusPerLevelTable = levelRequirement == 300 ? this._defenseBonusByLevelTableThird : this._defenseBonusByLevelTable;
        }

        var classes = this.GameConfiguration.DetermineCharacterClasses(darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, magicGladiatorClassLevel, darkLordClassLevel, summonerClassLevel, ragefighterClassLevel);
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

    private ItemOptionDefinition CreateCapeOptions()
    {
        var definition = this.CreateSecondClassWingOptions();
        definition.SetGuid(ItemOptionDefinitionNumbers.Cape);
        definition.Name = "Cape of Lord Options";
        definition.PossibleOptions.Add(this.CreateWingOption(4, Stats.TotalLeadership, 10f, AggregateType.AddRaw, 5f)); // Increase Command +10~75 Increases your Command by 10 plus 5 for each level. Only Cape of Lord can have it (PvM, PvP)
        this.GameConfiguration.ItemOptions.Add(definition);
        foreach (var option in definition.PossibleOptions)
        {
            option.SetGuid(ItemOptionDefinitionNumbers.Cape, option.PowerUpDefinition!.TargetAttribute!.Id.ExtractFirstTwoBytes());
        }

        return definition;
    }

    private ItemOptionDefinition CreateSecondClassWingOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.Wing2nd);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "2nd Wing Options";
        definition.AddChance = 0.1f;
        definition.AddsRandomly = true;
        definition.MaximumOptionsPerItem = 1;

        // "Excellent" 2nd wing options:
        definition.PossibleOptions.Add(this.CreateWingOption(1, Stats.MaximumHealth, 50f, AggregateType.AddRaw, 5f)); // Increase max HP +50~115 Increases your maximum amount of life by 50 plus 5 for each level (PvM, PvP)
        definition.PossibleOptions.Add(this.CreateWingOption(2, Stats.MaximumMana, 50f, AggregateType.AddRaw, 5f)); // Increase max mana +50~115 Increases your maximum amount of mana by 50 plus 5 for each level (PvM, PvP)
        definition.PossibleOptions.Add(this.CreateWingOption(3, Stats.DefenseIgnoreChance, 0.03f, AggregateType.AddRaw)); // Ignore opponent's defensive power by 3% Gives you 3% chance to lower your opponent's defence to 0 for a strike. This strike is shown with yellow colour (PvP)

        return definition;
    }

    private ItemOptionDefinition CreateThirdClassWingOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.Wing3rd);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "3rd Wing Options";
        definition.AddChance = 0.1f;
        definition.AddsRandomly = true;
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateWingOption(1, Stats.DefenseIgnoreChance, 0.05f, AggregateType.AddRaw)); // Ignore opponent's defensive power by 5%
        definition.PossibleOptions.Add(this.CreateWingOption(2, Stats.DamageReflection, 0.05f, AggregateType.AddRaw)); // Ignore opponent's defensive power by 5%
        definition.PossibleOptions.Add(this.CreateWingOption(3, Stats.FullyRecoverHealthAfterHitChance, 0.05f, AggregateType.AddRaw)); // Fully restore health when hit by 5 %
        definition.PossibleOptions.Add(this.CreateWingOption(4, Stats.FullyRecoverManaAfterHitChance, 0.05f, AggregateType.AddRaw)); // Fully recover mana when hit by 5 %

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
}