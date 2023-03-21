// <copyright file="GuardianOptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Initializer for the Level 380 Options which can be added to Level 380 Set Items
/// by using the Jewel Of Guardian at the Chaos Machine.
/// </summary>
internal class GuardianOptions : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GuardianOptions"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public GuardianOptions(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        this.CreateWeaponOption();
        this.CreateArmorOption();
        this.CreatePantsOption();
        this.CreateHelmOption();
        this.CreateBootsOption();
        this.CreateGlovesOption();
    }

    private void CreateWeaponOption()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "Guardian Option (Weapon)";
        definition.AddsRandomly = false;

        definition.PossibleOptions.Add(this.CreateOption(ItemGroups.Weapon, Stats.AttackRatePvp, 10, AggregateType.AddRaw, ItemOptionDefinitionNumbers.GuardianOption1));
        definition.PossibleOptions.Add(this.CreateOption(ItemGroups.Weapon, Stats.FinalDamageIncreasePvp, 200, AggregateType.AddRaw, ItemOptionDefinitionNumbers.GuardianOption2));
    }

    private void CreatePantsOption()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "Guardian Option (Pants)";
        definition.AddsRandomly = false;

        definition.PossibleOptions.Add(this.CreateOption(ItemGroups.Pants, Stats.DefenseRatePvp, 10, AggregateType.AddRaw, ItemOptionDefinitionNumbers.GuardianOption1));
        definition.PossibleOptions.Add(this.CreateOption(ItemGroups.Pants, Stats.DefenseBase, 200, AggregateType.AddRaw, ItemOptionDefinitionNumbers.GuardianOption2));
    }

    private void CreateArmorOption()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "Guardian Option (Armor)";
        definition.AddsRandomly = false;

        definition.PossibleOptions.Add(this.CreateOption(ItemGroups.Armor, Stats.DefenseRatePvp, 10, AggregateType.AddRaw, ItemOptionDefinitionNumbers.GuardianOption1));
        definition.PossibleOptions.Add(this.CreateOption(ItemGroups.Armor, Stats.ShieldRecoveryEverywhere, 1, AggregateType.AddRaw, ItemOptionDefinitionNumbers.GuardianOption2));
    }

    private void CreateHelmOption()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "Guardian Option (Helm)";
        definition.AddsRandomly = false;

        definition.PossibleOptions.Add(this.CreateOption(ItemGroups.Helm, Stats.DefenseRatePvp, 10, AggregateType.AddRaw, ItemOptionDefinitionNumbers.GuardianOption1));
        definition.PossibleOptions.Add(this.CreateOption(ItemGroups.Helm, Stats.ShieldRecoveryMultiplier, 20, AggregateType.AddRaw, ItemOptionDefinitionNumbers.GuardianOption2)); // 20 absolute, need test
    }

    private void CreateGlovesOption()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "Guardian Option (Gloves)";
        definition.AddsRandomly = false;

        definition.PossibleOptions.Add(this.CreateOption(ItemGroups.Gloves, Stats.DefenseRatePvp, 10, AggregateType.AddRaw, ItemOptionDefinitionNumbers.GuardianOption1));
        definition.PossibleOptions.Add(this.CreateOption(ItemGroups.Gloves, Stats.MaximumHealth, 200, AggregateType.AddFinal, ItemOptionDefinitionNumbers.GuardianOption2));
    }

    private void CreateBootsOption()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = "Guardian Option (Boots)";
        definition.AddsRandomly = false;

        definition.PossibleOptions.Add(this.CreateOption(ItemGroups.Boots, Stats.DefenseRatePvp, 10, AggregateType.AddRaw, ItemOptionDefinitionNumbers.GuardianOption1));
        definition.PossibleOptions.Add(this.CreateOption(ItemGroups.Boots, Stats.MaximumShield, 200, AggregateType.AddFinal, ItemOptionDefinitionNumbers.GuardianOption2));
    }

    private IncreasableItemOption CreateOption(ItemGroups itemGroup, AttributeDefinition attributeDefinition, float value, AggregateType aggregateType, short optionNumber)
    {
        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(optionNumber, (short)itemGroup);
        itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.GuardianOption);
        itemOption.Number = (int)itemGroup;
        itemOption.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        itemOption.PowerUpDefinition.TargetAttribute = this.GameConfiguration.Attributes.First(a => a == attributeDefinition);
        itemOption.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        itemOption.PowerUpDefinition.Boost.ConstantValue.Value = value;
        itemOption.PowerUpDefinition.Boost.ConstantValue.AggregateType = aggregateType;
        return itemOption;
    }
}