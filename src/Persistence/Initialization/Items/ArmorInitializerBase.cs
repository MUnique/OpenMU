// <copyright file="ArmorInitializerBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

/// <summary>
/// Base class for an initializer for armor items.
/// </summary>
public abstract class ArmorInitializerBase : InitializerBase
{
    private static readonly float[] DefenseIncreaseByLevel = { 0, 3, 6, 9, 12, 15, 18, 21, 24, 27, 31, 36, 42, 49, 57, 66 };
    private static readonly float[] ShieldDefenseIncreaseByLevel = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
    private ItemLevelBonusTable? _defenseIncreaseTable;
    private ItemLevelBonusTable? _shieldDefenseIncreaseTable;
    private ItemLevelBonusTable? _shieldDefenseRateIncreaseTable;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArmorInitializerBase"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    protected ArmorInitializerBase(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the maximum armor level.
    /// </summary>
    protected abstract byte MaximumArmorLevel { get; }

    /// <inheritdoc />
    public override void Initialize()
    {
        this._defenseIncreaseTable = this.CreateItemBonusTable(DefenseIncreaseByLevel, "Defense Increase (Armors)", "Defines the defense increase per item level for armors. It's 3 per item level until level 9, then it's always 1 more for each level.");
        this._shieldDefenseIncreaseTable = this.CreateItemBonusTable(ShieldDefenseIncreaseByLevel, "Defense Increase (Shields)", "Defines the defense increase per item level for shields. It's always 1 per item level.");
        this._shieldDefenseRateIncreaseTable = this.CreateItemBonusTable(DefenseIncreaseByLevel, "Defense Rate Increase (Shields)", "Defines the defense rate increase per item level for shields. It's 3 per item level until level 9, then it's always 1 more for each level.");
    }

    /// <summary>
    /// Builds the <see cref="ItemSetGroup"/> for whole sets..
    /// </summary>
    protected void BuildSets()
    {
        var sets = this.GameConfiguration.Items.Where(item => item.Group is >= 7 and <= 11).GroupBy(item => item.Number);

        var defenseRateBonusDef = this.Context.CreateNew<ItemOptionDefinition>();
        defenseRateBonusDef.SetGuid(ItemOptionDefinitionNumbers.DefenseRateSetBonusOption);
        defenseRateBonusDef.Name = "Complete Set Bonus (any level)";
        defenseRateBonusDef.AddChance = 0;
        defenseRateBonusDef.AddsRandomly = false;
        this.GameConfiguration.ItemOptions.Add(defenseRateBonusDef);

        var defenseRateBonus = this.Context.CreateNew<IncreasableItemOption>();
        defenseRateBonus.SetGuid(ItemOptionDefinitionNumbers.DefenseRateSetBonusOption);
        defenseRateBonus.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        defenseRateBonus.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        defenseRateBonus.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
        defenseRateBonus.PowerUpDefinition.Boost.ConstantValue.Value = 1.1f;
        defenseRateBonus.PowerUpDefinition.TargetAttribute = Stats.DefenseRatePvm.GetPersistent(this.GameConfiguration);
        defenseRateBonusDef.PossibleOptions.Add(defenseRateBonus);

        var defenseBonus = new Dictionary<int, ItemOptionDefinition>();
        for (byte setLevel = 10; setLevel <= this.MaximumArmorLevel; setLevel++)
        {
            var def = this.Context.CreateNew<ItemOptionDefinition>();
            def.SetGuid(setLevel);
            def.Name = $"Complete Set Bonus (Level {setLevel})";
            def.PossibleOptions.Add(this.BuildDefenseBonusOption(1 + (setLevel - 9) * 0.05f, setLevel));
            defenseBonus.Add(setLevel, def);
            this.GameConfiguration.ItemOptions.Add(def);
        }

        foreach (var group in sets)
        {
            var setForDefenseRate = this.Context.CreateNew<ItemSetGroup>();
            this.GameConfiguration.ItemSetGroups.Add(setForDefenseRate);
            setForDefenseRate.Name = group.First().Name.Split(' ')[0] + " Defense Rate Bonus";
            setForDefenseRate.MinimumItemCount = group.Count();
            setForDefenseRate.Options = defenseRateBonusDef;
            setForDefenseRate.AlwaysApplies = true;
            foreach (var item in group)
            {
                item.PossibleItemSetGroups.Add(setForDefenseRate);
                var itemOfSet = this.Context.CreateNew<ItemOfItemSet>();
                itemOfSet.SetGuid(item.Group, item.Number, 0xFF);
                itemOfSet.ItemDefinition = item;
                setForDefenseRate.Items.Add(itemOfSet);
            }

            for (byte setLevel = 10; setLevel <= this.MaximumArmorLevel; setLevel++)
            {
                this.CreateSetGroup(setLevel, defenseBonus[setLevel], group.ToList());
            }
        }
    }

    protected void CreateShield(byte number, byte slot, byte skill, byte width, byte height, string name, byte dropLevel, int defense, int defenseRate, byte durability, int strengthRequirement, int agilityRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel)
    {
        this.CreateShield(number, slot, skill, width, height, name, dropLevel, defense, defenseRate, durability, 0, strengthRequirement, agilityRequirement, 0, 0, 0, darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, 0, 0, 0, 0);
    }

    protected void CreateShield(byte number, byte slot, byte skill, byte width, byte height, string name, byte dropLevel, int defense, int defenseRate, byte durability, int levelRequirement, int strengthRequirement, int agilityRequirement, int energyRequirement, int vitalityRequirement, int leadershipRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, int magicGladiatorClassLevel, int darkLordClassLevel, int summonerClassLevel, int ragefighterClassLevel)
    {
        var shield = this.CreateArmor(number, slot, width, height, name, dropLevel, 0, durability, levelRequirement, strengthRequirement, agilityRequirement, energyRequirement, vitalityRequirement, leadershipRequirement, darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, magicGladiatorClassLevel, darkLordClassLevel, summonerClassLevel, ragefighterClassLevel, true);
        if (skill != 0)
        {
            shield.Skill = this.GameConfiguration.Skills.First(s => s.Number == skill);
        }

        if (defense > 0)
        {
            var powerUp = this.CreateItemBasePowerUpDefinition(Stats.DefenseBase, defense, AggregateType.AddRaw);
            powerUp.BonusPerLevelTable = this._shieldDefenseIncreaseTable;
            shield.BasePowerUpAttributes.Add(powerUp);
        }

        if (defenseRate > 0)
        {
            var powerUp = this.CreateItemBasePowerUpDefinition(Stats.DefenseRatePvm, defenseRate, AggregateType.AddRaw);
            powerUp.BonusPerLevelTable = this._shieldDefenseRateIncreaseTable;
            shield.BasePowerUpAttributes.Add(powerUp);
        }

        var isShieldEquipped = this.Context.CreateNew<ItemBasePowerUpDefinition>();
        isShieldEquipped.TargetAttribute = Stats.IsShieldEquipped.GetPersistent(this.GameConfiguration);
        isShieldEquipped.BaseValue = 1;
        shield.BasePowerUpAttributes.Add(isShieldEquipped);

        shield.PossibleItemOptions.Add(this.GameConfiguration.GetDefenseRateOption());
    }

    protected ItemDefinition CreateGloves(byte number, string name, byte dropLevel, int defense, int attackSpeed, byte durability, int strengthRequirement, int agilityRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel)
    {
        var gloves = this.CreateArmor(number, 5, 2, 2, name, dropLevel, defense, durability, strengthRequirement, agilityRequirement, darkWizardClassLevel, darkKnightClassLevel, elfClassLevel);
        if (attackSpeed > 0)
        {
            gloves.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.AttackSpeedAny, attackSpeed, AggregateType.AddRaw));
        }

        return gloves;
    }

    protected ItemDefinition CreateGloves(byte number, string name, byte dropLevel, int defense, int attackSpeed, byte durability, int levelRequirement, int strengthRequirement, int agilityRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, int magicGladiatorClassLevel, int darkLordClassLevel, int summonerClassLevel)
    {
        var gloves = this.CreateArmor(number, 5, 2, 2, name, dropLevel, defense, durability, levelRequirement, strengthRequirement, agilityRequirement, 0, 0, 0, darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, magicGladiatorClassLevel, darkLordClassLevel, summonerClassLevel, 0);
        if (attackSpeed > 0)
        {
            gloves.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.AttackSpeedAny, attackSpeed, AggregateType.AddRaw));
        }

        return gloves;
    }

    protected ItemDefinition CreateBoots(byte number, byte slot, byte width, byte height, string name, byte dropLevel, int defense, int walkSpeed, byte durability, int strengthRequirement, int agilityRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel)
    {
        var boots = this.CreateArmor(number, 6, 2, 2, name, dropLevel, defense, durability, strengthRequirement, agilityRequirement, darkWizardClassLevel, darkKnightClassLevel, elfClassLevel);
        if (walkSpeed > 0)
        {
            boots.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.WalkSpeed, walkSpeed, AggregateType.AddRaw));
        }

        return boots;
    }

    protected ItemDefinition CreateBoots(byte number, string name, byte dropLevel, int defense, int walkSpeed, byte durability, int levelRequirement, int strengthRequirement, int agilityRequirement, int energyRequirement, int vitalityRequirement, int leadershipRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, int magicGladiatorClassLevel, int darkLordClassLevel, int summonerClassLevel, int ragefighterClassLevel)
    {
        var boots = this.CreateArmor(number, 6, 2, 2, name, dropLevel, defense, durability, levelRequirement, strengthRequirement, agilityRequirement, energyRequirement, vitalityRequirement, leadershipRequirement, darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, magicGladiatorClassLevel, darkLordClassLevel, summonerClassLevel, ragefighterClassLevel);
        if (walkSpeed > 0)
        {
            boots.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.WalkSpeed, walkSpeed, AggregateType.AddRaw));
        }

        return boots;
    }

    protected ItemDefinition CreateArmor(byte number, byte slot, byte width, byte height, string name, byte dropLevel, int defense, byte durability, int strengthRequirement, int agilityRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel)
    {
        var magicGladiatorClassLevel = 0;
        if ((slot + 5) != (int)ItemGroups.Helm
            && (darkKnightClassLevel == 1 || darkWizardClassLevel == 1)
            && this.GameConfiguration.CharacterClasses.Any(c => c.Number == (byte)CharacterClassNumber.MagicGladiator))
        {
            magicGladiatorClassLevel = 1;
        }

        return this.CreateArmor(number, slot, width, height, name, dropLevel, defense, durability, 0, strengthRequirement, agilityRequirement, 0, 0, 0, darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, magicGladiatorClassLevel, 0, 0, 0);
    }

    protected ItemDefinition CreateArmor(byte number, byte slot, byte width, byte height, string name, byte dropLevel, int defense, byte durability, int levelRequirement, int strengthRequirement, int agilityRequirement, int energyRequirement, int vitalityRequirement, int leadershipRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, int magicGladiatorClassLevel, int darkLordClassLevel, int summonerClassLevel, int ragefighterClassLevel, bool isShield = false)
    {
        var armor = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(armor);
        armor.Group = (byte)(slot + 5);
        armor.Number = number;
        armor.Width = width;
        armor.Height = height;
        armor.Name = name;
        armor.DropLevel = dropLevel;
        armor.MaximumItemLevel = this.MaximumArmorLevel;
        armor.DropsFromMonsters = true;
        armor.Durability = durability;
        armor.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(st => st.ItemSlots.Contains(slot));
        this.CreateItemRequirementIfNeeded(armor, Stats.Level, levelRequirement);
        this.CreateItemRequirementIfNeeded(armor, Stats.TotalStrengthRequirementValue, strengthRequirement);
        this.CreateItemRequirementIfNeeded(armor, Stats.TotalAgilityRequirementValue, agilityRequirement);
        this.CreateItemRequirementIfNeeded(armor, Stats.TotalEnergyRequirementValue, energyRequirement);
        this.CreateItemRequirementIfNeeded(armor, Stats.TotalVitalityRequirementValue, vitalityRequirement);
        this.CreateItemRequirementIfNeeded(armor, Stats.TotalLeadershipRequirementValue, leadershipRequirement);

        if (defense > 0)
        {
            var powerUp = this.CreateItemBasePowerUpDefinition(Stats.DefenseBase, defense, AggregateType.AddRaw);
            powerUp.BonusPerLevelTable = this._defenseIncreaseTable;
            armor.BasePowerUpAttributes.Add(powerUp);
        }

        var classes = this.GameConfiguration.DetermineCharacterClasses(darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, magicGladiatorClassLevel, darkLordClassLevel, summonerClassLevel, ragefighterClassLevel);
        foreach (var characterClass in classes)
        {
            armor.QualifiedCharacters.Add(characterClass);
        }

        armor.PossibleItemOptions.Add(this.GameConfiguration.GetLuck());

        if (!isShield)
        {
            armor.PossibleItemOptions.Add(this.GameConfiguration.GetDefenseOption());
        }

        if (this.GameConfiguration.ItemOptions.FirstOrDefault(o => o.Name == ExcellentOptions.DefenseOptionsName) is { } excellentOption)
        {
            armor.PossibleItemOptions.Add(excellentOption);
        }

        if (this.GameConfiguration.ItemOptions.FirstOrDefault(o => o.Name == HarmonyOptions.DefenseOptionsName) is { } harmonyOptionsName)
        {
            armor.PossibleItemOptions.Add(harmonyOptionsName);
        }

        armor.SetGuid(armor.Group, armor.Number);
        return armor;
    }

    private IncreasableItemOption BuildDefenseBonusOption(float bonus, short level)
    {
        var defenseBonus = this.Context.CreateNew<IncreasableItemOption>();
        defenseBonus.SetGuid(ItemOptionDefinitionNumbers.DefenseSetBonusOption, level);
        defenseBonus.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        defenseBonus.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        defenseBonus.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
        defenseBonus.PowerUpDefinition.Boost.ConstantValue.Value = bonus;
        defenseBonus.PowerUpDefinition.TargetAttribute = Stats.DefenseBase.GetPersistent(this.GameConfiguration);
        return defenseBonus;
    }

    private void CreateSetGroup(byte setLevel, ItemOptionDefinition options, ICollection<ItemDefinition> group)
    {
        var setForDefense = this.Context.CreateNew<ItemSetGroup>();
        this.GameConfiguration.ItemSetGroups.Add(setForDefense);
        setForDefense.Name = $"{group.First().Name.Split(' ')[0]} Defense Bonus (Level {setLevel})";
        setForDefense.MinimumItemCount = group.Count;
        setForDefense.Options = options;
        setForDefense.AlwaysApplies = true;
        setForDefense.SetLevel = setLevel;

        foreach (var item in group)
        {
            var itemOfSet = this.Context.CreateNew<ItemOfItemSet>();
            itemOfSet.SetGuid(item.Group, item.Number, setLevel);
            itemOfSet.ItemDefinition = item;
            setForDefense.Items.Add(itemOfSet);
            item.PossibleItemSetGroups.Add(setForDefense);
        }
    }
}