// <copyright file="ClassDarkLord.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.CharacterClasses
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Initialization of character classes data.
    /// </summary>
    internal partial class CharacterClassInitialization
    {
        private CharacterClass CreateLordEmperor()
        {
            var result = this.CreateDarkLord(CharacterClassNumber.LordEmperor, "Lord Emperor", 7, true, null, false);
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.MasterLevel, 0, false));
            return result;
        }

        private CharacterClass CreateDarkLord(CharacterClassNumber number, string name, short pointsPerLevelUp, bool isMaster, CharacterClass nextGenerationClass, bool canGetCreated)
        {
            var energyMinus15 = this.Context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "TotalEnergy minus 15", "TotalEnergy minus 15");
            this.GameConfiguration.Attributes.Add(energyMinus15);

            var result = this.Context.CreateNew<CharacterClass>();
            this.GameConfiguration.CharacterClasses.Add(result);
            result.CanGetCreated = canGetCreated;
            result.HomeMap = this.GameConfiguration.Maps.FirstOrDefault(map => map.Number == LorenciaMapId);
            result.Number = (byte)number;
            result.Name = name;
            result.PointsPerLevelUp = pointsPerLevelUp;
            result.LevelRequirementByCreation = 250;
            result.IsMasterClass = isMaster;
            result.NextGenerationClass = nextGenerationClass;
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.Level, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseStrength, 26, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseAgility, 20, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseVitality, 20, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseEnergy, 15, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.BaseLeadership, 25, true));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentHealth, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentMana, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentAbility, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.CurrentShield, 1, false));
            result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.IsInSafezone, 1, false));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalStrength, 1, Stats.BaseStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalAgility, 1, Stats.BaseAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalVitality, 1, Stats.BaseVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.TotalEnergy, 1, Stats.BaseEnergy));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseBase, 1.0f / 7, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefensePvm, 1, Stats.DefenseBase));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefensePvp, 1, Stats.DefenseBase));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvm, 1.0f / 7, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 0.5f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.DefenseRatePvp, 2, Stats.Level));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 5, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 2.5f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 1.0f / 6, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvm, 1.0f / 10, Stats.TotalLeadership));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 4.0f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.AttackRatePvp, 3, Stats.Level));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.15f, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.1f, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.2f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.3f, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumAbility, 0.3f, Stats.TotalLeadership));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalAgility));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1.2f, Stats.TotalLeadership));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 0.5f, Stats.DefenseBase));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShieldTemp, 2f, Stats.Level, InputOperator.Exponentiate));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumShield, 1f / 30f, Stats.MaximumShieldTemp));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(energyMinus15, -15, Stats.TotalEnergy, InputOperator.Add));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 1.5f, energyMinus15));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumMana, 0.5f, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 2, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumHealth, 3, Stats.TotalVitality));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1.0f / 7, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1.0f / 5, Stats.TotalStrength));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1.0f / 14, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1.0f / 10, Stats.TotalEnergy));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MinimumPhysBaseDmg, 1, Stats.MinimumPhysBaseDmgByWeapon));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumPhysBaseDmg, 1, Stats.MaximumPhysBaseDmgByWeapon));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.SkillMultiplier, 0.005f, Stats.TotalEnergy));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumGuildSize, 0.1f, Stats.Level));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.MaximumGuildSize, 0.1f, Stats.TotalLeadership));

            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.HealthRecoveryMultiplier, 0.01f, Stats.IsInSafezone));
            result.AttributeCombinations.Add(this.CreateAttributeRelationship(Stats.ShieldRecoveryMultiplier, 0.01f, Stats.IsInSafezone));

            /* TODO: Add these stats
                                        Critical dmg = cmd/25+str/30
                                        Fireburst bonus min dmg = 100+str/25+ene/50
                                        Fireburst bonus max dmg = 150+str/25+ene/50
                                        Horse bonus dmg = 100+horseLvl*10+lvl*2.5+str/10+cmd/5
                                        Raven speed = 20+(ravenLvl*4)/5+cmd/50
                                        Raven min dmg = 180+ravenLvl*15+cmd/8
                                        Raven max dmg = 200+ravenLvl*15+cmd/4
                                    */

            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(38, Stats.MaximumMana));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(48.5f, Stats.MaximumHealth));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(2, Stats.SkillMultiplier));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(2, Stats.AbilityRecoveryAbsolute));
            result.BaseAttributeValues.Add(this.CreateConstValueAttribute(1.0f / 33f, Stats.AbilityRecoveryMultiplier));
            this.AddCommonBaseAttributeValues(result.BaseAttributeValues, isMaster);

            return result;
        }
    }
}