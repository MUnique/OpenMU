// <copyright file="HarmonyOptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for harmony options.
/// </summary>
public class HarmonyOptions : InitializerBase
{
    /// <summary>
    /// The name of the <see cref="ItemOptionDefinition"/> of harmony defense options.
    /// </summary>
    public static readonly string DefenseOptionsName = "Harmony Defense Options";

    /// <summary>
    /// The name of the <see cref="ItemOptionDefinition"/> of harmony physical attack options.
    /// </summary>
    public static readonly string PhysicalAttackOptionsName = "Harmony Physical Attack Options";

    /// <summary>
    /// The name of the <see cref="ItemOptionDefinition"/> of harmony wizardry attack options.
    /// </summary>
    public static readonly string WizardryAttackOptionsName = "Harmony Wizardry Attack Options";

    /// <summary>
    /// The name of the <see cref="ItemOptionDefinition"/> of harmony curse attack options.
    /// </summary>
    public static readonly string CurseAttackOptionsName = "Harmony Curse Attack Options";

    /// <summary>
    /// Initializes a new instance of the <see cref="HarmonyOptions"/> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public HarmonyOptions(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        this.CreateDefenseOptions();
        this.CreatePhysicalAttackOptions();
        this.CreateWizardryAttackOptions();
        this.CreateCurseAttackOptions();
    }

    private void CreateCurseAttackOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = CurseAttackOptionsName;
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateHarmonyOptions(1, ItemOptionDefinitionNumbers.HarmonyCurse, Stats.CurseAttackDamageIncrease, AggregateType.AddRaw, 0, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.17f, 0.18f, 0.19f, 0.21f, 0.23f, 0.25f, 0.27f, 0.31f)); // todo
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(2, ItemOptionDefinitionNumbers.HarmonyCurse, Stats.RequiredStrengthReduction, AggregateType.AddRaw, 0, 6, 8, 10, 12, 14, 16, 20, 23, 26, 29, 32, 35, 37, 40));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(3, ItemOptionDefinitionNumbers.HarmonyCurse, Stats.RequiredAgilityReduction, AggregateType.AddRaw, 0, 6, 8, 10, 12, 14, 16, 20, 23, 26, 29, 32, 35, 37, 40));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(4, ItemOptionDefinitionNumbers.HarmonyCurse, Stats.SkillDamageBonus, AggregateType.AddRaw, 6, 7, 10, 13, 16, 19, 22, 25, 30));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(5, ItemOptionDefinitionNumbers.HarmonyCurse, Stats.CriticalDamageBonus, AggregateType.AddRaw, 6, 10, 12, 14, 16, 18, 20, 22, 28));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(6, ItemOptionDefinitionNumbers.HarmonyCurse, Stats.ShieldDecreaseRateIncrease, AggregateType.AddRaw, 9, 3, 5, 7, 9, 10));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(7, ItemOptionDefinitionNumbers.HarmonyCurse, Stats.AttackRatePvp, AggregateType.AddRaw, 9, 5, 7, 9, 11, 14));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(8, ItemOptionDefinitionNumbers.HarmonyCurse, Stats.ShieldBypassChance, AggregateType.AddRaw, 13, 0.15f));
    }

    private void CreateWizardryAttackOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = WizardryAttackOptionsName;
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateHarmonyOptions(1, ItemOptionDefinitionNumbers.HarmonyWizardry, Stats.WizardryAttackDamageIncrease, AggregateType.AddRaw, 0, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.17f, 0.18f, 0.19f, 0.21f, 0.23f, 0.25f, 0.27f, 0.31f));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(2, ItemOptionDefinitionNumbers.HarmonyWizardry, Stats.RequiredStrengthReduction, AggregateType.AddRaw, 0, 6, 8, 10, 12, 14, 16, 20, 23, 26, 29, 32, 35, 37, 40));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(3, ItemOptionDefinitionNumbers.HarmonyWizardry, Stats.RequiredAgilityReduction, AggregateType.AddRaw, 0, 6, 8, 10, 12, 14, 16, 20, 23, 26, 29, 32, 35, 37, 40));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(4, ItemOptionDefinitionNumbers.HarmonyWizardry, Stats.SkillDamageBonus, AggregateType.AddRaw, 6, 7, 10, 13, 16, 19, 22, 25, 30));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(5, ItemOptionDefinitionNumbers.HarmonyWizardry, Stats.CriticalDamageBonus, AggregateType.AddRaw, 6, 10, 12, 14, 16, 18, 20, 22, 28));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(6, ItemOptionDefinitionNumbers.HarmonyWizardry, Stats.ShieldDecreaseRateIncrease, AggregateType.AddRaw, 9, 3, 5, 7, 9, 10));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(7, ItemOptionDefinitionNumbers.HarmonyWizardry, Stats.AttackRatePvp, AggregateType.AddRaw, 11, 5, 7, 9, 11, 14));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(8, ItemOptionDefinitionNumbers.HarmonyWizardry, Stats.ShieldBypassChance, AggregateType.AddRaw, 13, 0.15f));
    }   

    private void CreatePhysicalAttackOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = PhysicalAttackOptionsName;
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateHarmonyOptions(1, ItemOptionDefinitionNumbers.HarmonyPhysical, Stats.MinimumPhysBaseDmg, AggregateType.AddRaw, 0, 2, 3, 4, 5, 6, 7, 9, 11, 12, 14, 15, 16, 17, 20));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(2, ItemOptionDefinitionNumbers.HarmonyPhysical, Stats.MaximumPhysBaseDmg, AggregateType.AddRaw, 0, 3, 4, 5, 6, 7, 8, 10, 12, 14, 17, 20, 23, 26, 29));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(3, ItemOptionDefinitionNumbers.HarmonyPhysical, Stats.RequiredStrengthReduction, AggregateType.AddRaw, 0, 6, 8, 10, 12, 14, 16, 20, 23, 26, 29, 32, 35, 37, 40));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(4, ItemOptionDefinitionNumbers.HarmonyPhysical, Stats.RequiredAgilityReduction, AggregateType.AddRaw, 0, 6, 8, 10, 12, 14, 16, 20, 23, 26, 29, 32, 35, 37, 40));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(5, ItemOptionDefinitionNumbers.HarmonyPhysical, Stats.BaseDamageBonus, AggregateType.AddRaw, 6, 7, 8, 9, 11, 12, 14, 16, 19));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(6, ItemOptionDefinitionNumbers.HarmonyPhysical, Stats.CriticalDamageBonus, AggregateType.AddRaw, 6, 12, 14, 16, 18, 20, 22, 24, 30));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(7, ItemOptionDefinitionNumbers.HarmonyPhysical, Stats.SkillDamageBonus, AggregateType.AddRaw, 9, 12, 14, 16, 18, 22));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(8, ItemOptionDefinitionNumbers.HarmonyPhysical, Stats.AttackRatePvp, AggregateType.AddRaw, 9, 5, 7, 9, 11, 14));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(9, ItemOptionDefinitionNumbers.HarmonyPhysical, Stats.ShieldDecreaseRateIncrease, AggregateType.AddRaw, 9, 3, 5, 7, 9, 10));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(10, ItemOptionDefinitionNumbers.HarmonyPhysical, Stats.ShieldBypassChance, AggregateType.AddRaw, 13, 0.10f));
    }

    private void CreateDefenseOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = DefenseOptionsName;
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateHarmonyOptions(1, ItemOptionDefinitionNumbers.HarmonyDefense, Stats.DefenseBase, AggregateType.AddRaw, 0, 3, 4, 5, 6, 7, 8, 10, 12, 14, 16, 18, 20, 22, 25));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(2, ItemOptionDefinitionNumbers.HarmonyDefense, Stats.MaximumAbility, AggregateType.AddRaw, 3, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 25));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(3, ItemOptionDefinitionNumbers.HarmonyDefense, Stats.MaximumHealth, AggregateType.AddRaw, 3, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 30));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(4, ItemOptionDefinitionNumbers.HarmonyDefense, Stats.HealthRecoveryAbsolute, AggregateType.AddRaw, 6, 1, 2, 3, 4, 5, 6, 7, 8));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(5, ItemOptionDefinitionNumbers.HarmonyDefense, Stats.ManaRecoveryAbsolute, AggregateType.AddRaw, 9, 1, 2, 3, 4, 5));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(6, ItemOptionDefinitionNumbers.HarmonyDefense, Stats.DefenseRatePvp, AggregateType.AddRaw, 9, 3, 4, 5, 6, 8));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(7, ItemOptionDefinitionNumbers.HarmonyDefense, Stats.DamageReceiveDecrement, AggregateType.Multiplicate, 9, 0.97f, 0.96f, 0.95f, 0.94f, 0.93f));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(8, ItemOptionDefinitionNumbers.HarmonyDefense, Stats.ShieldRateIncrease, AggregateType.AddRaw, 13, 0.05f));
    }

    private IncreasableItemOption CreateHarmonyOptions(short number, short parentNumber, AttributeDefinition attributeDefinition, AggregateType aggregateType, byte minimumItemLevel, params float[] values)
    {
        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(parentNumber, number);
        itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.HarmonyOption);
        itemOption.Number = number;
        foreach (var value in values)
        {
            var optionOfLevel = this.Context.CreateNew<ItemOptionOfLevel>();
            optionOfLevel.RequiredItemLevel = minimumItemLevel++;
            optionOfLevel.Level = optionOfLevel.RequiredItemLevel;
            var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
            powerUpDefinition.TargetAttribute = this.GameConfiguration.Attributes.First(a => a == attributeDefinition);
            powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            powerUpDefinition.Boost.ConstantValue.Value = value;
            powerUpDefinition.Boost.ConstantValue.AggregateType = aggregateType;
            optionOfLevel.PowerUpDefinition = powerUpDefinition;
            itemOption.LevelDependentOptions.Add(optionOfLevel);
        }

        return itemOption;
    }
}