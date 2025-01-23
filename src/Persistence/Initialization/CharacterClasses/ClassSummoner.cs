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
        var berserkerFinalHealthMultiplier = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "BerserkerHealthMultiplier minus 0.4", string.Empty);
        this.GameConfiguration.Attributes.Add(berserkerFinalHealthMultiplier);
        var tempMaxMana = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Temp Max Mana", string.Empty);
        this.GameConfiguration.Attributes.Add(tempMaxMana);
        var tempMaxHealth = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Temp Max Health", string.Empty);
        this.GameConfiguration.Attributes.Add(tempMaxHealth);
        var tempMinCurseBaseDmg = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Temp Minimum Curse Base Damage", string.Empty);
        this.GameConfiguration.Attributes.Add(tempMinCurseBaseDmg);
        var tempMaxCurseBaseDmg = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Temp Maximum Curse Base Damage", string.Empty);
        this.GameConfiguration.Attributes.Add(tempMaxCurseBaseDmg);
        var tempMinWizBaseDmg = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Temp Minimum Wizardry Base Damage", string.Empty);
        this.GameConfiguration.Attributes.Add(tempMinWizBaseDmg);
        var tempMaxWizBaseDmg = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Temp Maximum Wizardry Base Damage", string.Empty);
        this.GameConfiguration.Attributes.Add(tempMaxWizBaseDmg);

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
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentMana, 20, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentAbility, 1, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentShield, 1, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.IsInSafezone, 1, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.Resets, 0, false));

        this.AddCommonAttributeRelationships(result.AttributeCombinations);

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalStrenghtAndAgility, Stats.TotalAgility, Stats.TotalStrength, InputOperator.Add));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseBase, 1.0f / 10, Stats.TotalAgility));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvm, 0.25f, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 0.1f, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 2, Stats.Level));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 5, Stats.TotalLevel));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 1.5f, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 0.25f, Stats.TotalStrength));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 3.5f, Stats.TotalAgility));
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

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(tempMaxMana, 1.7f, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(tempMaxMana, 1.5f, Stats.Level));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 1, tempMaxMana));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(tempMaxHealth, 1, Stats.Level));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(tempMaxHealth, 2, Stats.TotalVitality));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 1, tempMaxHealth));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1.0f / 7, Stats.TotalStrenghtAndAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1.0f / 4, Stats.TotalStrenghtAndAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1, Stats.MinimumPhysBaseDmgByWeapon));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1, Stats.MaximumPhysBaseDmgByWeapon));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1, Stats.PhysicalBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1, Stats.PhysicalBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(tempMinWizBaseDmg, 1.0f / 9, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(tempMaxWizBaseDmg, 1.0f / 4, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1, tempMinWizBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1, tempMaxWizBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1, Stats.WizardryBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1, Stats.WizardryBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1, Stats.WizardryAndCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1, Stats.WizardryAndCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1, Stats.MinWizardryAndCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.WizardryAttackDamageIncrease, 1.0f / 100, Stats.StaffRise));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(tempMinCurseBaseDmg, 1.0f / 9, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(tempMaxCurseBaseDmg, 1.0f / 4, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumCurseBaseDmg, 1, tempMinCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumCurseBaseDmg, 1, tempMaxCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumCurseBaseDmg, 1, Stats.CurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumCurseBaseDmg, 1, Stats.CurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumCurseBaseDmg, 1, Stats.WizardryAndCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumCurseBaseDmg, 1, Stats.WizardryAndCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumCurseBaseDmg, 1, Stats.MinWizardryAndCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.CurseAttackDamageIncrease, 1.0f / 100, Stats.BookRise));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.BerserkerMinPhysBaseDmg, 1.0f / 50, Stats.TotalStrenghtAndAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.BerserkerMaxPhysBaseDmg, 1.0f / 30, Stats.TotalStrenghtAndAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.BerserkerWizardryMultiplier, 1, Stats.BerserkerManaMultiplier));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.BerserkerCurseMultiplier, 1, Stats.BerserkerManaMultiplier));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, Stats.BerserkerManaMultiplier, tempMaxMana));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(berserkerFinalHealthMultiplier, -0.4f, Stats.BerserkerHealthMultiplier, InputOperator.Add));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, berserkerFinalHealthMultiplier, tempMaxHealth));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.BerserkerMinPhysDmgBonus, Stats.BerserkerManaMultiplier, Stats.BerserkerMinPhysBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.BerserkerMaxPhysDmgBonus, Stats.BerserkerManaMultiplier, Stats.BerserkerMaxPhysBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, Stats.BerserkerWizardryMultiplier, tempMinWizBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, Stats.BerserkerWizardryMultiplier, tempMaxWizBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumCurseBaseDmg, Stats.BerserkerCurseMultiplier, tempMinCurseBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumCurseBaseDmg, Stats.BerserkerCurseMultiplier, tempMaxCurseBaseDmg));

        result.AttributeCombinations.Add(this.CreateConditionalRelationship(Stats.WizardryBaseDmg, Stats.IsStickEquipped, Stats.StickBonusBaseDamage));
        result.AttributeCombinations.Add(this.CreateConditionalRelationship(Stats.CurseBaseDmg, Stats.IsBookEquipped, Stats.BookBonusBaseDamage));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 5, Stats.BaseStrength));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 5, Stats.BaseAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 7, Stats.BaseVitality));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 3, Stats.BaseEnergy));

        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(39, Stats.MaximumHealth));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(6, Stats.MaximumMana));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.SkillMultiplier));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f, Stats.WizardryAttackDamageIncrease));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f, Stats.CurseAttackDamageIncrease));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 33f, Stats.AbilityRecoveryMultiplier));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(140f, Stats.BerserkerMinPhysBaseDmg));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(160f, Stats.BerserkerMaxPhysBaseDmg));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(0.015f, Stats.MaximumCurseBaseDmg));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(0.015f, Stats.MaximumWizBaseDmg));
        /* result.BaseAttributeValues.Add(this.CreateConstValueAttribute(0.015f, tempMaxCurseBaseDmg)); => Exists in S6 source, but on later versions was removed */
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.ExcOptWizTwoPercentInc));

        this.AddCommonBaseAttributeValues(result.BaseAttributeValues, isMaster);

        return result;
    }
}