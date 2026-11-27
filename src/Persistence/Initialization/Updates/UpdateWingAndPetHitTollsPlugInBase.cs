// <copyright file="UpdateWingAndPetHitTollsPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Updates the wing and pet attributes used for hit tolls and flight.
/// </summary>
public abstract class UpdateWingAndPetHitTollsPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// Gets the skill-specific toll values used by the Season Six update.
    /// </summary>
    protected static readonly (short SkillNumber, float Toll)[] SkillTolls =
    [
        ((short)SkillNumber.KillingBlow, -1),
        ((short)SkillNumber.BeastUppercut, -1),
        ((short)SkillNumber.DragonRoar, -1),
        ((short)SkillNumber.PhoenixShot, -1),
        ((short)SkillNumber.DarkSide, 1),
        ((short)SkillNumber.Charge, 1),
        ((short)SkillNumber.ChainDrive, 1),
        ((short)SkillNumber.DragonSlasher, 97),
    ];

    /// <inheritdoc />
    public override string Name => PlugInResources.UpdateWingAndPetHitTollsPlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.UpdateWingAndPetHitTollsPlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 23, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Adds or updates a relationship between two attributes.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="relationships">The collection of relationships to update.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="targetAttribute">The target attribute.</param>
    /// <param name="multiplier">The input multiplier.</param>
    /// <param name="inputAttribute">The input attribute.</param>
    protected static void SetAttributeRelationship(
        IContext context,
        ICollection<AttributeRelationship> relationships,
        GameConfiguration gameConfiguration,
        AttributeDefinition targetAttribute,
        float multiplier,
        AttributeDefinition inputAttribute)
    {
        var persistentTarget = targetAttribute.GetPersistent(gameConfiguration);
        var persistentInput = inputAttribute.GetPersistent(gameConfiguration);
        var relationship = relationships.FirstOrDefault(existing =>
            existing.TargetAttribute?.Id == persistentTarget.Id
            && existing.InputAttribute?.Id == persistentInput.Id
            && existing.OperandAttribute is null
            && existing.InputOperator == InputOperator.Multiply);

        if (relationship is null)
        {
            relationship = context.CreateNew<AttributeRelationship>();
            relationships.Add(relationship);
        }

        relationship.TargetAttribute = persistentTarget;
        relationship.InputAttribute = persistentInput;
        relationship.OperandAttribute = null;
        relationship.InputOperator = InputOperator.Multiply;
        relationship.InputOperand = multiplier;
        relationship.AggregateType = AggregateType.AddRaw;
    }

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        this.AddStatIfNotExists(context, gameConfiguration, Stats.IsWingEquipped);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.HealthLossAfterHit);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.IsImpEquipped);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.SkillHealthLossAfterDealtHit);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.HealthLossInflictedOnAttacker);

        this.UpdateCharacterClassRelationships(context, gameConfiguration);
        this.UpdateWingPowerUps(context, gameConfiguration);
        this.UpdatePetPowerUps(context, gameConfiguration);
        this.UpdateSkillTolls(context, gameConfiguration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Updates the skill-specific toll relationships, if supported by the game version.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    protected virtual void UpdateSkillTolls(IContext context, GameConfiguration gameConfiguration)
    {
    }

    private void UpdateCharacterClassRelationships(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var characterClass in gameConfiguration.CharacterClasses)
        {
            SetAttributeRelationship(context, characterClass.AttributeCombinations, gameConfiguration, Stats.CanFly, 1, Stats.IsWingEquipped);

            var classNumber = (CharacterClassNumber)characterClass.Number;
            var healthToll = classNumber switch
            {
                CharacterClassNumber.DarkKnight
                    or CharacterClassNumber.BladeKnight
                    or CharacterClassNumber.BladeMaster
                    or CharacterClassNumber.MagicGladiator
                    or CharacterClassNumber.DuelMaster
                    or CharacterClassNumber.DarkLord
                    or CharacterClassNumber.LordEmperor
                    or CharacterClassNumber.RageFighter
                    or CharacterClassNumber.FistMaster => 3,
                _ => 1,
            };
            SetAttributeRelationship(context, characterClass.AttributeCombinations, gameConfiguration, Stats.HealthLossAfterDealtHit, healthToll, Stats.IsWingEquipped);
        }
    }

    private void UpdateWingPowerUps(IContext context, GameConfiguration gameConfiguration)
    {
        var canFly = Stats.CanFly.GetPersistent(gameConfiguration);
        var isWingEquipped = Stats.IsWingEquipped.GetPersistent(gameConfiguration);
        foreach (var wing in gameConfiguration.Items.Where(item => item.Group == 12))
        {
            foreach (var powerUp in wing.BasePowerUpAttributes.Where(powerUp => powerUp.TargetAttribute?.Id == canFly.Id))
            {
                powerUp.TargetAttribute = isWingEquipped;
            }

            this.SetItemPowerUp(context, gameConfiguration, wing, Stats.IsWingEquipped, 1, AggregateType.AddRaw);
        }
    }

    private void UpdatePetPowerUps(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var imp in gameConfiguration.Items.Where(item => item.Group == 13 && item.Number == 1))
        {
            this.SetItemPowerUp(context, gameConfiguration, imp, Stats.IsImpEquipped, 1, AggregateType.AddRaw);
            this.SetItemPowerUp(context, gameConfiguration, imp, Stats.HealthLossAfterDealtHit, 3, AggregateType.AddRaw);
        }

        foreach (var dinorant in gameConfiguration.Items.Where(item => item.Group == 13 && item.Number == 3))
        {
            this.SetItemPowerUp(context, gameConfiguration, dinorant, Stats.HealthLossAfterDealtHit, 1, AggregateType.AddRaw);
            this.SetItemPowerUp(context, gameConfiguration, dinorant, Stats.HealthLossInflictedOnAttacker, 1, AggregateType.AddRaw);
        }

        foreach (var darkHorse in gameConfiguration.Items.Where(item => item.Group == 13 && item.Number == 4))
        {
            this.SetItemPowerUp(context, gameConfiguration, darkHorse, Stats.HealthLossAfterHit, 1, AggregateType.AddRaw);
        }
    }

    private void SetItemPowerUp(IContext context, GameConfiguration gameConfiguration, ItemDefinition item, AttributeDefinition attribute, float value, AggregateType aggregateType)
    {
        var persistentAttribute = attribute.GetPersistent(gameConfiguration);
        var powerUp = item.BasePowerUpAttributes.FirstOrDefault(existing => existing.TargetAttribute?.Id == persistentAttribute.Id);
        if (powerUp is null)
        {
            powerUp = context.CreateNew<ItemBasePowerUpDefinition>();
            item.BasePowerUpAttributes.Add(powerUp);
        }

        powerUp.TargetAttribute = persistentAttribute;
        powerUp.BaseValue = value;
        powerUp.AggregateType = aggregateType;
    }
}