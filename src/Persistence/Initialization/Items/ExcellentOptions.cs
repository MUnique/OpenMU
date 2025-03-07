// <copyright file="ExcellentOptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for excellent options.
/// </summary>
public class ExcellentOptions : InitializerBase
{
    /// <summary>
    /// The name of the <see cref="ItemOptionDefinition"/> of excellent defense options.
    /// </summary>
    public static readonly string DefenseOptionsName = "Excellent Defense Options";

    /// <summary>
    /// The name of the <see cref="ItemOptionDefinition"/> of excellent physical attack options.
    /// </summary>
    public static readonly string PhysicalAttackOptionsName = "Excellent Physical Attack Options";

    /// <summary>
    /// The name of the <see cref="ItemOptionDefinition"/> of excellent wizardry attack options.
    /// </summary>
    public static readonly string WizardryAttackOptionsName = "Excellent Wizardry Attack Options";

    /// <summary>
    /// The name of the <see cref="ItemOptionDefinition"/> of excellent curse attack options.
    /// </summary>
    public static readonly string CurseAttackOptionsName = "Excellent Curse Attack Options";

    /// <summary>
    /// Initializes a new instance of the <see cref="ExcellentOptions"/> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public ExcellentOptions(IContext context, GameConfiguration gameConfiguration)
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

    [Obsolete(@"There are no curse attack excellent options until Season 14 (Summoner renewal). Until then, excellent curse spell books had excellent wizardry attack options.
        Reference: https://muonline.webzen.com/pt/gameinfo/guide/detail/91")]
    private void CreateCurseAttackOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.ExcellentCurse);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = CurseAttackOptionsName;
        definition.AddChance = 0.001f;
        definition.AddsRandomly = true;
        definition.MaximumOptionsPerItem = 2;

        definition.PossibleOptions.Add(this.CreateExcellentOption(1, Stats.ManaAfterMonsterKillMultiplier, 1f / 8f, AggregateType.AddRaw, ItemOptionDefinitionNumbers.ExcellentCurse));
        definition.PossibleOptions.Add(this.CreateExcellentOption(2, Stats.HealthAfterMonsterKillMultiplier, 1f / 8f, AggregateType.AddRaw, ItemOptionDefinitionNumbers.ExcellentCurse));
        definition.PossibleOptions.Add(this.CreateExcellentOption(3, Stats.AttackSpeedAny, 7, AggregateType.AddRaw, ItemOptionDefinitionNumbers.ExcellentCurse));
        definition.PossibleOptions.Add(this.CreateExcellentOption(4, Stats.MaximumCurseBaseDmg, 1.02f, AggregateType.Multiplicate, ItemOptionDefinitionNumbers.ExcellentCurse));
        definition.PossibleOptions.Add(this.CreateRelatedExcellentOption(5, Stats.WizardryBaseDmg, Stats.Level, 1f / 20f, ItemOptionDefinitionNumbers.ExcellentCurse));
        definition.PossibleOptions.Add(this.CreateExcellentOption(6, Stats.ExcellentDamageChance, 0.1f, AggregateType.AddRaw, ItemOptionDefinitionNumbers.ExcellentCurse));
    }

    private void CreateWizardryAttackOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.ExcellentWizardry);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = WizardryAttackOptionsName;
        definition.AddChance = 0.001f;
        definition.AddsRandomly = true;
        definition.MaximumOptionsPerItem = 2;

        definition.PossibleOptions.Add(this.CreateExcellentOption(1, Stats.ManaAfterMonsterKillMultiplier, 1f / 8f, AggregateType.AddRaw, ItemOptionDefinitionNumbers.ExcellentWizardry));
        definition.PossibleOptions.Add(this.CreateExcellentOption(2, Stats.HealthAfterMonsterKillMultiplier, 1f / 8f, AggregateType.AddRaw, ItemOptionDefinitionNumbers.ExcellentWizardry));
        definition.PossibleOptions.Add(this.CreateExcellentOption(3, Stats.AttackSpeedAny, 7, AggregateType.AddRaw, ItemOptionDefinitionNumbers.ExcellentWizardry));
        definition.PossibleOptions.Add(this.CreateExcellentOption(4, Stats.MaximumWizBaseDmg, 1.02f, AggregateType.Multiplicate, ItemOptionDefinitionNumbers.ExcellentWizardry));
        definition.PossibleOptions.Add(this.CreateRelatedExcellentOption(5, Stats.WizardryBaseDmg, Stats.Level, 1f / 20f, ItemOptionDefinitionNumbers.ExcellentWizardry));
        definition.PossibleOptions.Add(this.CreateExcellentOption(6, Stats.ExcellentDamageChance, 0.1f, AggregateType.AddRaw, ItemOptionDefinitionNumbers.ExcellentWizardry));
    }

    private void CreatePhysicalAttackOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.ExcellentPhysical);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = PhysicalAttackOptionsName;
        definition.AddChance = 0.001f;
        definition.AddsRandomly = true;
        definition.MaximumOptionsPerItem = 2;

        definition.PossibleOptions.Add(this.CreateExcellentOption(1, Stats.ManaAfterMonsterKillMultiplier, 1f / 8f, AggregateType.AddRaw, ItemOptionDefinitionNumbers.ExcellentPhysical));
        definition.PossibleOptions.Add(this.CreateExcellentOption(2, Stats.HealthAfterMonsterKillMultiplier, 1f / 8f, AggregateType.AddRaw, ItemOptionDefinitionNumbers.ExcellentPhysical));
        definition.PossibleOptions.Add(this.CreateExcellentOption(3, Stats.AttackSpeedAny, 7, AggregateType.AddRaw, ItemOptionDefinitionNumbers.ExcellentPhysical));
        definition.PossibleOptions.Add(this.CreateExcellentOption(4, Stats.MaximumPhysBaseDmg, 1.02f, AggregateType.Multiplicate, ItemOptionDefinitionNumbers.ExcellentPhysical));
        definition.PossibleOptions.Add(this.CreateRelatedExcellentOption(5, Stats.PhysicalBaseDmg, Stats.Level, 1f / 20f, ItemOptionDefinitionNumbers.ExcellentPhysical));
        definition.PossibleOptions.Add(this.CreateExcellentOption(6, Stats.ExcellentDamageChance, 0.1f, AggregateType.AddRaw, ItemOptionDefinitionNumbers.ExcellentPhysical));
    }

    private void CreateDefenseOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.ExcellentDefense);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = DefenseOptionsName;
        definition.AddChance = 0.001f;
        definition.AddsRandomly = true;
        definition.MaximumOptionsPerItem = 2;

        definition.PossibleOptions.Add(this.CreateExcellentOption(1, Stats.MoneyAmountRate, 1.4f, AggregateType.Multiplicate, ItemOptionDefinitionNumbers.ExcellentDefense));
        definition.PossibleOptions.Add(this.CreateExcellentOption(2, Stats.DefenseRatePvm, 1.1f, AggregateType.Multiplicate, ItemOptionDefinitionNumbers.ExcellentDefense));
        definition.PossibleOptions.Add(this.CreateExcellentOption(3, Stats.DamageReflection, 0.04f, AggregateType.AddRaw, ItemOptionDefinitionNumbers.ExcellentDefense));
        definition.PossibleOptions.Add(this.CreateExcellentOption(4, Stats.DamageReceiveDecrement, 0.96f, AggregateType.Multiplicate, ItemOptionDefinitionNumbers.ExcellentDefense));
        definition.PossibleOptions.Add(this.CreateExcellentOption(5, Stats.MaximumMana, 1.04f, AggregateType.Multiplicate, ItemOptionDefinitionNumbers.ExcellentDefense));
        definition.PossibleOptions.Add(this.CreateExcellentOption(6, Stats.MaximumHealth, 1.04f, AggregateType.Multiplicate, ItemOptionDefinitionNumbers.ExcellentDefense));
    }

    private IncreasableItemOption CreateExcellentOption(short number, AttributeDefinition attributeDefinition, float value, AggregateType aggregateType, short parentNumber)
    {
        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(parentNumber, number);
        itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Excellent);
        itemOption.Number = number;
        itemOption.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        itemOption.PowerUpDefinition.TargetAttribute = attributeDefinition.GetPersistent(this.GameConfiguration);
        itemOption.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        itemOption.PowerUpDefinition.Boost.ConstantValue.Value = value;
        itemOption.PowerUpDefinition.Boost.ConstantValue.AggregateType = aggregateType;
        return itemOption;
    }

    private IncreasableItemOption CreateRelatedExcellentOption(short number, AttributeDefinition targetAttribute, AttributeDefinition sourceAttribute, float multiplier, short parentNumber)
    {
        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(parentNumber, number);
        itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Excellent);
        itemOption.Number = number;
        itemOption.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        itemOption.PowerUpDefinition.TargetAttribute = targetAttribute.GetPersistent(this.GameConfiguration);
        itemOption.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();

        var attributeRelationship = this.Context.CreateNew<AttributeRelationship>();
        attributeRelationship.SetGuid(parentNumber, number); // because of a different type, the id will be different from the item option above.
        itemOption.PowerUpDefinition.Boost.RelatedValues.Add(attributeRelationship);
        attributeRelationship.InputOperator = InputOperator.Multiply;
        attributeRelationship.InputOperand = multiplier;
        attributeRelationship.InputAttribute = sourceAttribute.GetPersistent(this.GameConfiguration);
        attributeRelationship.TargetAttribute = targetAttribute.GetPersistent(this.GameConfiguration);
        return itemOption;
    }
}