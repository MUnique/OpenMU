﻿// <copyright file="ClassSummoner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.CharacterClasses
{
    using System.Linq;
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
            return this.CreateSummoner(CharacterClassNumber.BloodySummoner, "Bloody Summoner", 6, false, dimensionMaster, false);
        }

        private CharacterClass CreateDimensionMaster()
        {
            var result = this.CreateSummoner(CharacterClassNumber.DimensionMaster, "Dimension Master", 6, true, null, false);
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.MasterLevel, 0, false));
            return result;
        }

        private CharacterClass CreateSummoner(CharacterClassNumber number, string name, short pointsPerLevelUp, bool isMaster, CharacterClass nextGenerationClass, bool canGetCreated)
        {
            var result = this.Context.CreateNew<CharacterClass>();
            this.GameConfiguration.CharacterClasses.Add(result);
            result.CanGetCreated = canGetCreated;
            result.HomeMap = this.GameConfiguration.Maps.FirstOrDefault(map => map.Number == ElvenlandMapId);
            result.Number = (byte)number;
            result.Name = name;
            result.PointsPerLevelUp = pointsPerLevelUp;
            result.IsMasterClass = isMaster;
            result.NextGenerationClass = nextGenerationClass;
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.Level, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseStrength, 21, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseAgility, 21, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseVitality, 18, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseEnergy, 23, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentHealth, 70, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentMana, 20, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentAbility, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentShield, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.IsInSafezone, 1, false));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalStrength, 1, Stats.BaseStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalAgility, 1, Stats.BaseAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalVitality, 1, Stats.BaseVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalEnergy, 1, Stats.BaseEnergy));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseBase, 1.0f / 10, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefensePvm, 1, Stats.DefenseBase));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefensePvp, 1, Stats.DefenseBase));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvm, 0.25f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 0.1f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 2, Stats.Level));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 5, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 1.5f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 0.25f, Stats.TotalStrength));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 0.6f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 3, Stats.Level));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackSpeed, 1.0f / 20, Stats.TotalAgility));

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
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1.0f / 8, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1.0f / 4, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1, Stats.MinimumPhysBaseDmgByWeapon));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1, Stats.MaximumPhysBaseDmgByWeapon));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumWizBaseDmg, 1.0f / 9, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumWizBaseDmg, 1.0f / 4, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumGuildSize, 0.1f, Stats.Level));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.HealthRecoveryMultiplier, 0.01f, Stats.IsInSafezone));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.ShieldRecoveryMultiplier, 0.01f, Stats.IsInSafezone));

            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(39, Stats.MaximumHealth));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(6, Stats.MaximumMana));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1, Stats.SkillMultiplier));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(2, Stats.AbilityRecoveryAbsolute));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 33f, Stats.AbilityRecoveryMultiplier));

            this.AddCommonBaseAttributeValues(result.BaseAttributeValues, isMaster);
            return result;
        }
    }
}