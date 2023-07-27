// <copyright file="ClassRageFighter.cs" company="MUnique">
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
    private CharacterClass CreateFistMaster()
    {
        var result = this.CreateRageFighter(CharacterClassNumber.FistMaster, "Fist Master", true, null, false);
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.MasterLevel, 0, false));
        return result;
    }

    private CharacterClass CreateRageFighter(CharacterClassNumber number, string name, bool isMaster, CharacterClass? nextGenerationClass, bool canGetCreated)
    {
        var result = this.Context.CreateNew<CharacterClass>();
        result.SetGuid((byte)number);
        this.GameConfiguration.CharacterClasses.Add(result);
        result.CanGetCreated = canGetCreated;
        result.HomeMap = this.GameConfiguration.Maps.FirstOrDefault(map => map.Number == LorenciaMapId);
        result.Number = (byte)number;
        result.Name = name;
        result.CreationAllowedFlag = 8;
        result.IsMasterClass = isMaster;
        result.LevelRequirementByCreation = 150;
        result.NextGenerationClass = nextGenerationClass;
        result.LevelWarpRequirementReductionPercent = (int) Math.Ceiling(100.0 / 3);
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.Level, 1, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.PointsPerLevelUp, 7, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseStrength, 32, true));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseAgility, 27, true));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseVitality, 25, true));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseEnergy, 20, true));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentHealth, 100, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentMana, 40, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentAbility, 1, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentShield, 1, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.IsInSafezone, 1, false));
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.Resets, 0, false));

        this.AddCommonAttributeRelationships(result.AttributeCombinations);

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseBase, 1.0f / 8, Stats.TotalAgility));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvm, 1.0f / 10, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 0.113f, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 2, Stats.Level));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 3, Stats.Level));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 2.5f, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 6, Stats.TotalStrength));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 3.5f, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 3, Stats.Level));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 1.0f / 5, Stats.TotalEnergy));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 1.0f, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.3f, Stats.TotalVitality));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.2f, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.15f, Stats.TotalStrength));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalVitality));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalStrength));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 0.5f, Stats.DefenseBase));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShieldTemp, 2f, Stats.Level, InputOperator.Exponentiate));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1f / 30f, Stats.MaximumShieldTemp));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 1.3f, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 1, Stats.Level));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 1.3f, Stats.Level));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 2, Stats.TotalVitality));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1.0f / 7, Stats.TotalStrength));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1.0f / 5, Stats.TotalStrength));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1.0f / 15, Stats.TotalVitality));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1.0f / 12, Stats.TotalVitality));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1, Stats.MinimumPhysBaseDmgByWeapon));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1, Stats.MaximumPhysBaseDmgByWeapon));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1, Stats.PhysicalBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1, Stats.PhysicalBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1.0f / 9, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1.0f / 4, Stats.TotalEnergy));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1, Stats.WizardryBaseDmg));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1, Stats.WizardryBaseDmg));

        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 3, Stats.TotalStrength));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 5, Stats.TotalAgility));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 5, Stats.TotalVitality));
        result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.FenrirBaseDmg, 1.0f / 7, Stats.TotalEnergy));

        result.AttributeCombinations.Add(this.CreateConditionalRelationship(Stats.PhysicalBaseDmg, Stats.IsGloveWeaponEquipped, Stats.GloveWeaponBonusBaseDamage));

        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(57, Stats.MaximumHealth));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(7, Stats.MaximumMana));
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(2, Stats.SkillMultiplier));
        
        result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 33f, Stats.AbilityRecoveryMultiplier));
        this.AddCommonBaseAttributeValues(result.BaseAttributeValues, isMaster);
        return result;
    }
}