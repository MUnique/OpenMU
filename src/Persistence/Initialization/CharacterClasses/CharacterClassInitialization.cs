﻿// <copyright file="CharacterClassInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initialization of character classes data.
/// </summary>
internal partial class CharacterClassInitialization : InitializerBase
{
    private const int LorenciaMapId = 0;
    private const int NoriaMapId = 3;
    private const int ElvenlandMapId = 51;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterClassInitialization" /> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CharacterClassInitialization(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets a value indicating whether to use classic PVP, which uses no shield stats and the same attack/defense rate as PvM.
    /// </summary>
    protected virtual bool UseClassicPvp => false;

    /// <summary>
    /// Creates the character classes.
    /// </summary>
    public override void Initialize()
    {
        var bladeMaster = this.CreateBladeMaster();
        var bladeKnight = this.CreateBladeKnight(bladeMaster);
        this.CreateDarkKnight(CharacterClassNumber.DarkKnight, "Dark Knight", false, bladeKnight, true);

        var grandMaster = this.CreateGrandMaster();
        var soulMaster = this.CreateSoulMaster(grandMaster);
        this.CreateDarkWizard(CharacterClassNumber.DarkWizard, "Dark Wizard", false, soulMaster, true);

        var highElf = this.CreateHighElf();
        var museElf = this.CreateMuseElf(highElf);
        this.CreateFairyElf(CharacterClassNumber.FairyElf, "Fairy Elf", false, museElf, true);

        var dimensionMaster = this.CreateDimensionMaster();
        var bloodySummoner = this.CreateBloodySummoner(dimensionMaster);
        this.CreateSummoner(CharacterClassNumber.Summoner, "Summoner", false, bloodySummoner, true);

        var duelMaster = this.CreateDuelMaster();
        this.CreateMagicGladiator(CharacterClassNumber.MagicGladiator, "Magic Gladiator", false, duelMaster, true);

        var lordEmperor = this.CreateLordEmperor();
        this.CreateDarkLord(CharacterClassNumber.DarkLord, "Dark Lord", false, lordEmperor, true);

        var fistMaster = this.CreateFistMaster();
        this.CreateRageFighter(CharacterClassNumber.RageFighter, "Rage Fighter", false, fistMaster, true);
    }

    private StatAttributeDefinition CreateStatAttributeDefinition(AttributeDefinition attribute, int value, bool increasableByPlayer)
    {
        var definition = this.Context.CreateNew<StatAttributeDefinition>(attribute.GetPersistent(this.GameConfiguration), value, increasableByPlayer);
        return definition;
    }

    private AttributeRelationship CreateAttributeRelationship(AttributeDefinition targetAttribute, float multiplier, AttributeDefinition sourceAttribute, InputOperator inputOperator = InputOperator.Multiply)
    {
        return this.Context.CreateNew<AttributeRelationship>(targetAttribute.GetPersistent(this.GameConfiguration) ?? targetAttribute, multiplier, sourceAttribute.GetPersistent(this.GameConfiguration) ?? sourceAttribute, inputOperator, default(AttributeDefinition?));
    }

    private AttributeRelationship CreateAttributeRelationship(AttributeDefinition targetAttribute, AttributeDefinition multiplierAttribute, AttributeDefinition sourceAttribute, InputOperator inputOperator = InputOperator.Multiply)
    {
        return this.Context.CreateNew<AttributeRelationship>(
            targetAttribute.GetPersistent(this.GameConfiguration) ?? targetAttribute,
            0f,
            sourceAttribute.GetPersistent(this.GameConfiguration) ?? sourceAttribute,
            inputOperator,
            multiplierAttribute.GetPersistent(this.GameConfiguration) ?? multiplierAttribute);
    }

    private AttributeRelationship CreateConditionalRelationship(AttributeDefinition targetAttribute, AttributeDefinition conditionalAttribute, AttributeDefinition sourceAttribute)
    {
        return this.Context.CreateNew<AttributeRelationship>(
            targetAttribute.GetPersistent(this.GameConfiguration) ?? targetAttribute,
            conditionalAttribute.GetPersistent(this.GameConfiguration) ?? conditionalAttribute,
            sourceAttribute.GetPersistent(this.GameConfiguration) ?? sourceAttribute);
    }

    private ConstValueAttribute CreateConstValueAttribute(float value, AttributeDefinition attribute)
    {
        return this.Context.CreateNew<ConstValueAttribute>(value, attribute.GetPersistent(this.GameConfiguration));
    }

    private void AddCommonAttributeRelationships(ICollection<AttributeRelationship> attributeRelationships)
    {
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.TotalStrength, 1, Stats.BaseStrength));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.TotalAgility, 1, Stats.BaseAgility));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.TotalVitality, 1, Stats.BaseVitality));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.TotalEnergy, 1, Stats.BaseEnergy));

        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.DefensePvm, 1, Stats.DefenseBase));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.DefensePvp, 1, Stats.DefenseBase));

        attributeRelationships.Add(this.CreateConditionalRelationship(Stats.DefenseBase, Stats.IsShieldEquipped, Stats.BonusDefenseWithShield));

        var tempDefense = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Temp Defense Bonus multiplier with Shield", string.Empty);
        this.GameConfiguration.Attributes.Add(tempDefense);
        attributeRelationships.Add(this.CreateAttributeRelationship(tempDefense, Stats.IsShieldEquipped, Stats.DefenseIncreaseWithEquippedShield));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.DefensePvm, tempDefense, Stats.DefenseBase));
        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.DefensePvp, tempDefense, Stats.DefenseBase));

        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.HealthRecoveryMultiplier, 0.01f, Stats.IsInSafezone));
        if (this.UseClassicPvp)
        {
            attributeRelationships.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 1, Stats.DefenseRatePvm));
            attributeRelationships.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 1, Stats.AttackRatePvm));
        }
        else
        {
            attributeRelationships.Add(this.CreateAttributeRelationship(Stats.ShieldRecoveryMultiplier, 0.01f, Stats.IsInSafezone));
        }

        attributeRelationships.Add(this.CreateAttributeRelationship(Stats.MaximumGuildSize, 0.1f, Stats.Level));
    }

    private void AddCommonBaseAttributeValues(ICollection<ConstValueAttribute> baseAttributeValues, bool isMaster)
    {
        baseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 27.5f, Stats.ManaRecoveryMultiplier));
        baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.DamageReceiveDecrement));
        baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.AttackDamageIncrease));
        baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.TwoHandedWeaponDamageIncrease));
        baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.MoneyAmountRate));
        baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.ExperienceRate));
        baseAttributeValues.Add(this.CreateConstValueAttribute(0.03f, Stats.PoisonDamageMultiplier));
        baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.ItemDurationIncrease));
        baseAttributeValues.Add(this.CreateConstValueAttribute(2, Stats.AbilityRecoveryAbsolute));

        if (isMaster)
        {
            baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.MasterPointsPerLevelUp));
            baseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.MasterExperienceRate));
        }

        if (!this.UseClassicPvp)
        {
            baseAttributeValues.Add(this.CreateConstValueAttribute(0.01f, Stats.ShieldRecoveryMultiplier));
        }
    }
}