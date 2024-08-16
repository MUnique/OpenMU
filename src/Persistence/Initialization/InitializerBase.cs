// <copyright file="InitializerBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

/// <summary>
/// Base class for an <see cref="IInitializer"/>.
/// </summary>
/// <seealso cref="MUnique.OpenMU.Persistence.Initialization.IInitializer" />
public abstract class InitializerBase : IInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InitializerBase"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    protected InitializerBase(IContext context, GameConfiguration gameConfiguration)
    {
        this.Context = context;
        this.GameConfiguration = gameConfiguration;
    }

    /// <summary>
    /// Gets the persistence context.
    /// </summary>
    /// <value>
    /// The persistence context.
    /// </value>
    protected IContext Context { get; }

    /// <summary>
    /// Gets the game configuration.
    /// </summary>
    /// <value>
    /// The game configuration.
    /// </value>
    protected GameConfiguration GameConfiguration { get; }

    /// <summary>
    /// Gets the maximum option level.
    /// </summary>
    protected virtual int MaximumOptionLevel => 4;

    /// <inheritdoc />
    public abstract void Initialize();

    /// <summary>
    /// Creates an attribute requirement with the specified minimum value.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <returns>The created requirement.</returns>
    protected AttributeRequirement CreateRequirement(AttributeDefinition attribute, int minimumValue)
    {
        var requirement = this.Context.CreateNew<AttributeRequirement>();
        requirement.Attribute = attribute.GetPersistent(this.GameConfiguration);
        requirement.MinimumValue = minimumValue;
        return requirement;
    }

    /// <summary>
    /// Creates an item requirement, if the required value is greater than 0.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="attribute">The attribute.</param>
    /// <param name="requiredValue">The required value.</param>
    protected void CreateItemRequirementIfNeeded(ItemDefinition item, AttributeDefinition attribute, int requiredValue)
    {
        if (requiredValue == 0)
        {
            return;
        }

        var requirement = this.CreateRequirement(attribute, requiredValue);
        item.Requirements.Add(requirement);
    }

    /// <summary>
    /// Gets the character class of the specified number.
    /// </summary>
    /// <param name="classNumber">The class number.</param>
    /// <returns>The character class of the specified number.</returns>
    protected CharacterClass? GetCharacterClass(CharacterClassNumber classNumber)
    {
        return this.GameConfiguration.CharacterClasses.FirstOrDefault(c => c.Number == (byte)classNumber);
    }

    /// <summary>
    /// Creates the power up definition with the given values.
    /// </summary>
    /// <param name="attributeDefinition">The attribute definition.</param>
    /// <param name="value">The value.</param>
    /// <param name="aggregateType">Type of the aggregate.</param>
    /// <returns>The created power up definition.</returns>
    protected PowerUpDefinition CreatePowerUpDefinition(AttributeDefinition attributeDefinition, float value, AggregateType aggregateType)
    {
        var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        powerUpDefinition.TargetAttribute = attributeDefinition.GetPersistent(this.GameConfiguration);
        if (value != 0)
        {
            powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            powerUpDefinition.Boost.ConstantValue.Value = value;
            powerUpDefinition.Boost.ConstantValue.AggregateType = aggregateType;
        }

        return powerUpDefinition;
    }

    /// <summary>
    /// Creates the <see cref="ItemBasePowerUpDefinition" />.
    /// </summary>
    /// <param name="attributeDefinition">The attribute definition.</param>
    /// <param name="value">The value.</param>
    /// <param name="aggregateType">Type of the aggregate.</param>
    /// <returns>
    /// The created <see cref="ItemBasePowerUpDefinition" />.
    /// </returns>
    protected ItemBasePowerUpDefinition CreateItemBasePowerUpDefinition(AttributeDefinition attributeDefinition, float value, AggregateType aggregateType)
    {
        var powerUpDefinition = this.Context.CreateNew<ItemBasePowerUpDefinition>();
        powerUpDefinition.TargetAttribute = attributeDefinition.GetPersistent(this.GameConfiguration);
        powerUpDefinition.BaseValue = value;
        powerUpDefinition.AggregateType = aggregateType;
        return powerUpDefinition;
    }

    /// <summary>
    /// Creates a new item bonus table and adds it to the <see cref="GameConfiguration"/>.
    /// </summary>
    /// <param name="values">The values, index is the item level.</param>
    /// <param name="name">The name.</param>
    /// <param name="description">The description.</param>
    /// <returns>The created table.</returns>
    protected ItemLevelBonusTable CreateItemBonusTable(float[] values, string name, string description)
    {
        var table = this.Context.CreateNew<ItemLevelBonusTable>();
        this.GameConfiguration.ItemLevelBonusTables.Add(table);
        table.Name = name;
        table.Description = description;
        for (int level = 0; level < values.Length; level++)
        {
            var value = values[level];
            if (value != 0)
            {
                var levelBonus = this.Context.CreateNew<LevelBonus>();
                levelBonus.Level = level;
                levelBonus.AdditionalValue = values[level];
                table.BonusPerLevel.Add(levelBonus);
            }
        }

        return table;
    }

    /// <summary>
    /// Creates a new <see cref="IncreasableItemOption" /> with the specified parameters
    /// with options for multiple option levels until <see cref="MaximumOptionLevel" />.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="attributeDefinition">The attribute definition.</param>
    /// <param name="value">The value.</param>
    /// <param name="aggregateType">Type of the aggregate.</param>
    /// <param name="valueIncrementPerLevel">The value increment per level.</param>
    /// <param name="optionNumber">The option number.</param>
    /// <returns>
    /// The created <see cref="IncreasableItemOption" />.
    /// </returns>
    protected IncreasableItemOption CreateItemOption(int number, AttributeDefinition attributeDefinition, float value, AggregateType aggregateType, float valueIncrementPerLevel, short optionNumber)
    {
        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(optionNumber, attributeDefinition.Id.ExtractFirstTwoBytes());
        itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Option);
        itemOption.Number = number;

        itemOption.PowerUpDefinition = this.CreatePowerUpDefinition(attributeDefinition, value, aggregateType);

        for (int level = 1; level <= this.MaximumOptionLevel; level++)
        {
            var optionOfLevel = this.Context.CreateNew<ItemOptionOfLevel>();
            optionOfLevel.Level = level;
            optionOfLevel.PowerUpDefinition = this.CreatePowerUpDefinition(
                itemOption.PowerUpDefinition.TargetAttribute!,
                level * valueIncrementPerLevel,
                aggregateType);
            itemOption.LevelDependentOptions.Add(optionOfLevel);
        }

        return itemOption;
    }
}