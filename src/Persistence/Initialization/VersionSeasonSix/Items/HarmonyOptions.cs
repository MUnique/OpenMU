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

        // this.CreateCurseAttackOptions();
    }

    [Obsolete(@"There are no curse attack harmony options until Season 16 (JoH system renewal). Until then, curse spell books had wizardry attack options.
        Reference: https://muonline.webzen.com/en/gameinfo/guide/detail/117")]
    private void CreateCurseAttackOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = CurseAttackOptionsName;
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateHarmonyOptions(1, ItemOptionDefinitionNumbers.HarmonyCurse, 40, Stats.WizardryBaseDmg, AggregateType.AddRaw, 0, 6, 8, 10, 12, 14, 16, 17, 18, 19, 21, 23, 25, 27, 31));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(2, ItemOptionDefinitionNumbers.HarmonyCurse, 40, Stats.RequiredStrengthReduction, AggregateType.AddRaw, 0, 6, 8, 10, 12, 14, 16, 20, 23, 26, 29, 32, 35, 37, 40));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(3, ItemOptionDefinitionNumbers.HarmonyCurse, 40, Stats.RequiredAgilityReduction, AggregateType.AddRaw, 0, 6, 8, 10, 12, 14, 16, 20, 23, 26, 29, 32, 35, 37, 40));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(4, ItemOptionDefinitionNumbers.HarmonyCurse, 30, Stats.SkillDamageBonus, AggregateType.AddRaw, 6, 7, 10, 13, 16, 19, 22, 25, 30));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(5, ItemOptionDefinitionNumbers.HarmonyCurse, 30, Stats.CriticalDamageBonus, AggregateType.AddRaw, 6, 10, 12, 14, 16, 18, 20, 22, 28));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(6, ItemOptionDefinitionNumbers.HarmonyCurse, 20, Stats.ShieldDecreaseRateIncrease, AggregateType.AddRaw, 9, 4, 6, 8, 10, 13));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(7, ItemOptionDefinitionNumbers.HarmonyCurse, 20, Stats.AttackRatePvp, AggregateType.AddRaw, 9, 5, 7, 9, 11, 14));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(8, ItemOptionDefinitionNumbers.HarmonyCurse, 10, Stats.ShieldBypassChance, AggregateType.AddRaw, 13, 0.15f));
    }

    private void CreateWizardryAttackOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = WizardryAttackOptionsName;
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateHarmonyOptions(1, ItemOptionDefinitionNumbers.HarmonyWizardry, 40, Stats.WizardryBaseDmg, AggregateType.AddRaw, 0, 6, 8, 10, 12, 14, 16, 17, 18, 19, 21, 23, 25, 27, 31));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(2, ItemOptionDefinitionNumbers.HarmonyWizardry, 40, Stats.RequiredStrengthReduction, AggregateType.AddRaw, 0, 6, 8, 10, 12, 14, 16, 20, 23, 26, 29, 32, 35, 37, 40));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(3, ItemOptionDefinitionNumbers.HarmonyWizardry, 40, Stats.RequiredAgilityReduction, AggregateType.AddRaw, 0, 6, 8, 10, 12, 14, 16, 20, 23, 26, 29, 32, 35, 37, 40));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(4, ItemOptionDefinitionNumbers.HarmonyWizardry, 30, Stats.SkillDamageBonus, AggregateType.AddRaw, 6, 7, 10, 13, 16, 19, 22, 25, 30));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(5, ItemOptionDefinitionNumbers.HarmonyWizardry, 30, Stats.CriticalDamageBonus, AggregateType.AddRaw, 6, 10, 12, 14, 16, 18, 20, 22, 28));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(6, ItemOptionDefinitionNumbers.HarmonyWizardry, 20, Stats.ShieldDecreaseRateIncrease, AggregateType.AddRaw, 9, 4, 6, 8, 10, 13));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(7, ItemOptionDefinitionNumbers.HarmonyWizardry, 20, Stats.AttackRatePvp, AggregateType.AddRaw, 9, 5, 7, 9, 11, 14));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(8, ItemOptionDefinitionNumbers.HarmonyWizardry, 10, Stats.ShieldBypassChance, AggregateType.AddRaw, 13, 0.15f));
    }

    private void CreatePhysicalAttackOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = PhysicalAttackOptionsName;
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateHarmonyOptions(1, ItemOptionDefinitionNumbers.HarmonyPhysical, 40, Stats.MinimumPhysBaseDmg, AggregateType.AddRaw, 0, 2, 3, 4, 5, 6, 7, 9, 11, 12, 14, 15, 16, 17, 20));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(2, ItemOptionDefinitionNumbers.HarmonyPhysical, 40, Stats.MaximumPhysBaseDmg, AggregateType.AddRaw, 0, 3, 4, 5, 6, 7, 8, 10, 12, 14, 17, 20, 23, 26, 29));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(3, ItemOptionDefinitionNumbers.HarmonyPhysical, 40, Stats.RequiredStrengthReduction, AggregateType.AddRaw, 0, 6, 8, 10, 12, 14, 16, 20, 23, 26, 29, 32, 35, 37, 40));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(4, ItemOptionDefinitionNumbers.HarmonyPhysical, 40, Stats.RequiredAgilityReduction, AggregateType.AddRaw, 0, 6, 8, 10, 12, 14, 16, 20, 23, 26, 29, 32, 35, 37, 40));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(5, ItemOptionDefinitionNumbers.HarmonyPhysical, 30, Stats.PhysicalBaseDmg, AggregateType.AddRaw, 6, 7, 8, 9, 11, 12, 14, 16, 19));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(6, ItemOptionDefinitionNumbers.HarmonyPhysical, 30, Stats.CriticalDamageBonus, AggregateType.AddRaw, 6, 12, 14, 16, 18, 20, 22, 24, 30));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(7, ItemOptionDefinitionNumbers.HarmonyPhysical, 20, Stats.SkillDamageBonus, AggregateType.AddRaw, 9, 12, 14, 16, 18, 22));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(8, ItemOptionDefinitionNumbers.HarmonyPhysical, 20, Stats.AttackRatePvp, AggregateType.AddRaw, 9, 5, 7, 9, 11, 14));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(9, ItemOptionDefinitionNumbers.HarmonyPhysical, 20, Stats.ShieldDecreaseRateIncrease, AggregateType.AddRaw, 9, 3, 5, 7, 9, 10));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(10, ItemOptionDefinitionNumbers.HarmonyPhysical, 10, Stats.ShieldBypassChance, AggregateType.AddRaw, 13, 0.10f));
    }

    private void CreateDefenseOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = DefenseOptionsName;
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateHarmonyOptions(1, ItemOptionDefinitionNumbers.HarmonyDefense, 50, Stats.DefenseBase, AggregateType.AddRaw, 0, 3, 4, 5, 6, 7, 8, 10, 12, 14, 16, 18, 20, 22, 25));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(2, ItemOptionDefinitionNumbers.HarmonyDefense, 40, Stats.MaximumAbility, AggregateType.AddRaw, 3, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 25));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(3, ItemOptionDefinitionNumbers.HarmonyDefense, 40, Stats.MaximumHealth, AggregateType.AddRaw, 3, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 30));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(4, ItemOptionDefinitionNumbers.HarmonyDefense, 30, Stats.HealthRecoveryAbsolute, AggregateType.AddRaw, 6, 1, 2, 3, 4, 5, 6, 7, 8));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(5, ItemOptionDefinitionNumbers.HarmonyDefense, 20, Stats.ManaRecoveryAbsolute, AggregateType.AddRaw, 9, 1, 2, 3, 4, 5));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(6, ItemOptionDefinitionNumbers.HarmonyDefense, 20, Stats.DefenseRatePvp, AggregateType.AddRaw, 9, 3, 4, 5, 6, 8));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(7, ItemOptionDefinitionNumbers.HarmonyDefense, 20, Stats.DamageReceiveDecrement, AggregateType.Multiplicate, 9, 0.97f, 0.96f, 0.95f, 0.94f, 0.93f));
        definition.PossibleOptions.Add(this.CreateHarmonyOptions(8, ItemOptionDefinitionNumbers.HarmonyDefense, 10, Stats.ShieldRateIncrease, AggregateType.AddRaw, 13, 0.05f));
    }

    private IncreasableItemOption CreateHarmonyOptions(short number, short parentNumber, byte weight, AttributeDefinition attributeDefinition, AggregateType aggregateType, byte minimumItemLevel, params float[] values)
    {
        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(parentNumber, number);
        itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.HarmonyOption);
        itemOption.Number = number;
        itemOption.Weight = weight;
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