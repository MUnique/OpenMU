// <copyright file="ClassSummoner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initialization of character classes data.
/// </summary>
internal partial class CharacterClassInitialization
{
    private CharacterClass CreateBloodySummoner(CharacterClass dimensionMaster)
    {
        return this.CreateSummoner(CharacterClassNumber.BloodySummoner, "Bloody Summoner", false, dimensionMaster, false);
    }

    private CharacterClass CreateDimensionMaster()
    {
        var result = this.CreateSummoner(CharacterClassNumber.DimensionMaster, "Dimension Master", true, null, false);
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.MasterLevel, 0, false));
        return result;
    }

    private CharacterClass CreateSummoner(CharacterClassNumber number, string name, bool isMaster, CharacterClass? nextGenerationClass, bool canGetCreated)
    {
        var statsMinWizAndCurseBaseDmg = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Stats min wiz and curse base dmg", string.Empty);
        this.GameConfiguration.Attributes.Add(statsMinWizAndCurseBaseDmg);
        var statsMaxWizAndCurseBaseDmg = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Stats max wiz and curse base dmg", string.Empty);
        this.GameConfiguration.Attributes.Add(statsMaxWizAndCurseBaseDmg);
        var berserkerHealthMultiplierDecrement = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Berserker health multiplier decrement", string.Empty);
        this.GameConfiguration.Attributes.Add(berserkerHealthMultiplierDecrement);
        var isBerserkerBuffed = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Is berserker buffed", string.Empty);
        this.GameConfiguration.Attributes.Add(isBerserkerBuffed);
        var berserkerHealthMultiplier = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Berserker health multiplier", string.Empty);
        this.GameConfiguration.Attributes.Add(berserkerHealthMultiplier);
        var finalBerserkerManaMultiplier = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Final berserker mana multiplier", string.Empty);
        this.GameConfiguration.Attributes.Add(finalBerserkerManaMultiplier);

        var result = this.Context.CreateNew<CharacterClass>();
        result.SetGuid((byte)number);
        this.GameConfiguration.CharacterClasses.Add(result);
        result.CanGetCreated = canGetCreated;
        result.HomeMap = this.GameConfiguration.Maps.FirstOrDefault(map => map.Number == ElvenlandMapId);
        result.Number = (byte)number;
        result.Name = name;
        result.CreationAllowedFlag = 1;
        result.IsMasterClass = isMaster;
        result.NextGenerationClass = nextGenerationClass;
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.Level, 1, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.PointsPerLevelUp, 5, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseStrength, 21, true));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseAgility, 21, true));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseVitality, 18, true));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseEnergy, 23, true));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentHealth, 70, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentMana, 40, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentAbility, 1, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentShield, 1, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.IsInSafezone, 1, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.Resets, 0, false));

        this.AddCommonAttributeRelationships(result.AttributeCombinations);

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalStrengthAndAgility, Stats.TotalAgility, Stats.TotalStrength, InputOperator.Add));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseBase, 1.0f / 3, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvm, 0.25f, Stats.TotalAgility));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 0.5f, Stats.BaseAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 2, Stats.Level));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 5, Stats.TotalLevel));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 1.5f, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 0.25f, Stats.TotalStrength));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 3.5f, Stats.BaseAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 3, Stats.TotalLevel));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackSpeed, 1.0f / 20, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MagicSpeed, 1.0f / 20, Stats.TotalAgility));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.15f, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.3f, Stats.TotalVitality));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.25f, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.2f, Stats.TotalStrength));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalVitality));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalStrength));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 0.5f, Stats.DefenseBase));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShieldTemp, 2f, Stats.Level, InputOperator.Exponentiate));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1f / 30f, Stats.MaximumShieldTemp));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 1.7f, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 1.5f, Stats.Level));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 1, Stats.Level));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 2, Stats.TotalVitality));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1.0f / 7, Stats.TotalStrengthAndAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1.0f / 4, Stats.TotalStrengthAndAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(statsMinWizAndCurseBaseDmg, 1.0f / 9, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(statsMaxWizAndCurseBaseDmg, 1.0f / 4, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1, statsMinWizAndCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1, statsMaxWizAndCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1, Stats.WizardryBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1, Stats.WizardryBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1, Stats.HarmonyWizBaseDmg, stage: 2));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1, Stats.HarmonyWizBaseDmg, stage: 2));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1, Stats.SocketBaseMinDmgBonus, stage: 2));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1, Stats.SocketBaseMaxDmgBonus, stage: 2));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1, Stats.ExcellentWizBaseDmg, stage: 2));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1, Stats.ExcellentWizBaseDmg, stage: 2));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1, Stats.ExcellentWizTwoPercentInc, aggregateType: AggregateType.Multiplicate, stage: 2));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1, Stats.ExcellentWizTwoPercentInc, aggregateType: AggregateType.Multiplicate, stage: 2));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1, Stats.AncientWizDmgIncrease, aggregateType: AggregateType.Multiplicate, stage: 3));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1, Stats.AncientWizDmgIncrease, aggregateType: AggregateType.Multiplicate, stage: 3));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1, Stats.BaseDamageBonus, stage: 4));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1, Stats.BaseDamageBonus, stage: 4));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.WizardryAttackDamageIncrease, 1.0f / 100, Stats.StaffRise));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumCurseBaseDmg, 1, statsMinWizAndCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumCurseBaseDmg, 1, statsMaxWizAndCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumCurseBaseDmg, 1, Stats.CurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumCurseBaseDmg, 1, Stats.CurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.CurseAttackDamageIncrease, 1.0f / 100, Stats.BookRise));

        result.AttributeCombinations.Add(this.CreateConditionalRelationship(Stats.MinimumWizBaseDmg, Stats.IsStickEquipped, Stats.StickBonusBaseDamage, stage: 4));
        result.AttributeCombinations.Add(this.CreateConditionalRelationship(Stats.MaximumWizBaseDmg, Stats.IsStickEquipped, Stats.StickBonusBaseDamage, stage: 4));
        result.AttributeCombinations.Add(this.CreateConditionalRelationship(Stats.AttackSpeedAny, Stats.IsBookEquipped, Stats.WeaponMasteryAttackSpeed));
        result.AttributeCombinations.Add(this.CreateConditionalRelationship(Stats.MinimumCurseBaseDmg, Stats.IsBookEquipped, Stats.BookBonusBaseDamage, stage: 2));
        result.AttributeCombinations.Add(this.CreateConditionalRelationship(Stats.MaximumCurseBaseDmg, Stats.IsBookEquipped, Stats.BookBonusBaseDamage, stage: 2));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(finalBerserkerManaMultiplier, 1, Stats.BerserkerManaMultiplier));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 1, finalBerserkerManaMultiplier, aggregateType: AggregateType.Multiplicate));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(berserkerHealthMultiplierDecrement, -0.1f, Stats.BerserkerHealthMultiplierFactor, InputOperator.Minimum)); // At least -10% HP
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(isBerserkerBuffed, 1, Stats.BerserkerMinPhysDmgBonus, InputOperator.Minimum));
        result.AttributeCombinations.Add(this.CreateConditionalRelationship(berserkerHealthMultiplier, isBerserkerBuffed, berserkerHealthMultiplierDecrement));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 1, berserkerHealthMultiplier, aggregateType: AggregateType.Multiplicate));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.BerserkerMinPhysDmgBonus, 1, Stats.BerserkerManaMultiplier, aggregateType: AggregateType.Multiplicate));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.BerserkerMaxPhysDmgBonus, 1, Stats.BerserkerManaMultiplier, aggregateType: AggregateType.Multiplicate));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.BerserkerWizardryMultiplier, 1, Stats.BerserkerManaMultiplier));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.BerserkerCurseMultiplier, 1, Stats.BerserkerManaMultiplier));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.BerserkerWizardryMultiplier, 1, Stats.BerserkerProficiencyMultiplier));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.BerserkerMinWizDmgBonus, Stats.BerserkerWizardryMultiplier, statsMinWizAndCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.BerserkerMaxWizDmgBonus, Stats.BerserkerWizardryMultiplier, statsMaxWizAndCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.BerserkerMinCurseDmgBonus, Stats.BerserkerCurseMultiplier, statsMinWizAndCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.BerserkerMaxCurseDmgBonus, Stats.BerserkerCurseMultiplier, statsMaxWizAndCurseBaseDmg));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 5, Stats.BaseStrength));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 5, Stats.BaseAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 7, Stats.BaseVitality));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 3, Stats.BaseEnergy));

        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(39, Stats.MaximumHealth));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(6, Stats.MaximumMana));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f, Stats.WizardryAttackDamageIncrease));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f, Stats.CurseAttackDamageIncrease));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 33f, Stats.AbilityRecoveryMultiplier));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(0.015f, statsMaxWizAndCurseBaseDmg));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, finalBerserkerManaMultiplier));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, berserkerHealthMultiplier));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(0, Stats.BerserkerManaMultiplier));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(0, Stats.BerserkerHealthMultiplierFactor));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.ExcellentWizTwoPercentInc));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.AncientWizDmgIncrease));

        this.AddCommonBaseAttributeValues(result.BaseAttributeValues, isMaster);

        return result;
    }
}