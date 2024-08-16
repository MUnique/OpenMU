// <copyright file="Jewelery.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for jewelery (rings and pendants).
/// </summary>
internal class Jewelery : InitializerBase
{
    private static readonly float[] ResistanceIncreaseByLevel = { 0, 0.1f, 0.2f, 0.3f, 0.4f };
    private ItemOptionDefinition? _healthRecoverOptionDefinition;
    private ItemLevelBonusTable? _resistancesBonusTable;

    /// <summary>
    /// Initializes a new instance of the <see cref="Jewelery"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Jewelery(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public sealed override void Initialize()
    {
        this._healthRecoverOptionDefinition = this.CreateOption("Health recover for jewelery", Stats.HealthRecoveryMultiplier, 0.01f, ItemOptionDefinitionNumbers.JeweleryHealth);
        this._resistancesBonusTable = this.CreateItemBonusTable(ResistanceIncreaseByLevel, "Elemental resistances (Jewelery)", "Defines the elemental resistances for jewelery. It's 10 % per item level.");
        this.CreateItems();
    }

    /// <summary>
    /// Creates the items.
    /// </summary>
    protected virtual void CreateItems()
    {
        this.CreateRing(8, "Ring of Ice", 20, 50, Stats.IceResistance);
        this.CreateRing(9, "Ring of Poison", 17, 50, Stats.PoisonResistance);

        this.CreatePendant(12, "Pendant of Lighting", 21, 50, Stats.LightningResistance);
        this.CreatePendant(13, "Pendant of Fire", 13, 50, Stats.FireResistance);

        this.CreateTransformationRing(
            10,
            "Transformation Ring",
            0,
            200,
            CharacterTransformationSkin.BudgeDragon,
            CharacterTransformationSkin.Giant,
            CharacterTransformationSkin.SkeletonWarrior,
            CharacterTransformationSkin.PoisonBullFighter,
            CharacterTransformationSkin.ThunderLich,
            CharacterTransformationSkin.DeathCow);
    }

    /// <summary>
    /// Creates a transformation ring.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="name">The name.</param>
    /// <param name="dropLevel">The level.</param>
    /// <param name="durability">The durability.</param>
    /// <param name="transformationSkins">The transformation skins, per item level.</param>
    /// <returns>The definition of the created ring.</returns>
    protected ItemDefinition CreateTransformationRing(byte number, string name, byte dropLevel, byte durability, params CharacterTransformationSkin[] transformationSkins)
    {
        var ring = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(ring);
        ring.Group = 13;
        ring.Number = number;
        ring.ItemSlot = this.GameConfiguration.ItemSlotTypes.FirstOrDefault(slotType => slotType.ItemSlots.Contains(10));
        ring.Name = name;
        ring.DropLevel = dropLevel;
        ring.Durability = durability;
        ring.MaximumItemLevel = (byte)(transformationSkins.Length - 1);
        ring.Width = 1;
        ring.Height = 1;

        var powerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
        ring.BasePowerUpAttributes.Add(powerUp);
        powerUp.BaseValue = 0;
        powerUp.TargetAttribute = Stats.TransformationSkin.GetPersistent(this.GameConfiguration);
        powerUp.BonusPerLevelTable = this.CreateItemBonusTable(transformationSkins.Select(v => (float)v).ToArray(), $"Transformation Ring Skins ({name})", $"Contains the skin numbers for the different levels of the transformation ring ({name}).");

        foreach (var characterClass in this.GameConfiguration.CharacterClasses)
        {
            ring.QualifiedCharacters.Add(characterClass);
        }

        ring.SetGuid(ring.Group, ring.Number);
        return ring;
    }

    /// <summary>
    /// Creates the option.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="targetAttribute">The target attribute.</param>
    /// <param name="boostPerLevel">The boost per level.</param>
    /// <param name="optionNumber">The option number.</param>
    /// <param name="aggregateType">Type of the aggregate.</param>
    /// <returns>
    /// The created option.
    /// </returns>
    protected ItemOptionDefinition CreateOption(string name, AttributeDefinition targetAttribute, float boostPerLevel, short optionNumber, AggregateType aggregateType = AggregateType.AddRaw)
    {
        var optionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
        optionDefinition.SetGuid(optionNumber, targetAttribute.Id.ExtractFirstTwoBytes());
        optionDefinition.AddsRandomly = true;
        optionDefinition.Name = name;
        optionDefinition.AddChance = 0.25f;
        optionDefinition.MaximumOptionsPerItem = 1;

        var option = this.CreateItemOption(0, targetAttribute, 0, aggregateType, boostPerLevel, optionNumber);
        optionDefinition.PossibleOptions.Add(option);
        this.GameConfiguration.ItemOptions.Add(optionDefinition);
        return optionDefinition;
    }

    /// <summary>
    /// Creates the jewelery.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="slot">The slot.</param>
    /// <param name="dropsFromMonsters">if set to <c>true</c> [drops from monsters].</param>
    /// <param name="name">The name.</param>
    /// <param name="level">The level.</param>
    /// <param name="durability">The durability.</param>
    /// <param name="resistanceAttribute">The resistance attribute.</param>
    /// <param name="withHealthOption">if set to <c>true</c> [with health option].</param>
    /// <returns>The created jewelery.</returns>
    protected ItemDefinition CreateJewelery(byte number, int slot, bool dropsFromMonsters, string name, byte level, byte durability, AttributeDefinition? resistanceAttribute, bool withHealthOption = true)
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(item);
        item.Group = 13;
        item.Number = number;
        item.ItemSlot = this.GameConfiguration.ItemSlotTypes.FirstOrDefault(slotType => slotType.ItemSlots.Contains(slot));
        item.DropsFromMonsters = dropsFromMonsters;
        item.Name = name;
        item.DropLevel = level;
        item.MaximumItemLevel = 4;
        item.Width = 1;
        item.Height = 1;
        item.Durability = durability;

        //// TODO: Requirement increases with item level
        this.CreateItemRequirementIfNeeded(item, Stats.Level, level);

        if (withHealthOption)
        {
            item.PossibleItemOptions.Add(this._healthRecoverOptionDefinition!);
        }

        if (resistanceAttribute != null)
        {
            var powerUp = this.CreateItemBasePowerUpDefinition(resistanceAttribute, 0.1f, AggregateType.AddRaw);
            powerUp.BonusPerLevelTable = this._resistancesBonusTable;
        }

        foreach (var characterClass in this.GameConfiguration.CharacterClasses)
        {
            item.QualifiedCharacters.Add(characterClass);
        }

        item.SetGuid(item.Group, item.Number);
        return item;
    }

    /// <summary>
    /// Creates a ring.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="name">The name.</param>
    /// <param name="level">The level.</param>
    /// <param name="durability">The durability.</param>
    /// <param name="resistanceAttribute">The resistance attribute.</param>
    /// <remarks>
    /// Rings always have defensive excellent options.
    /// </remarks>
    private void CreateRing(byte number, string name, byte level, byte durability, AttributeDefinition? resistanceAttribute)
    {
        this.CreateJewelery(number, 10, true, name, level, durability, resistanceAttribute);
    }

    /// <summary>
    /// Creates a pendant.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="name">The name.</param>
    /// <param name="level">The level.</param>
    /// <param name="durability">The durability.</param>
    /// <param name="resistanceAttribute">The resistance attribute.</param>
    /// <remarks>
    /// Pendants always have offensive excellent options. If it's wizardry or physical depends on the specific item. I didn't find a pattern yet.
    /// </remarks>
    private void CreatePendant(byte number, string name, byte level, byte durability, AttributeDefinition? resistanceAttribute)
    {
        this.CreateJewelery(number, 9, true, name, level, durability, resistanceAttribute);
    }
}