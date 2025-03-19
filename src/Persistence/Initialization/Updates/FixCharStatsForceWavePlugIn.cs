// <copyright file="FixCharStatsForceWavePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes several character stats values and DL Force (Wave) base and master skills.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("0C1995AB-A1CC-42A8-9EFC-E5FE8F360C53")]
public class FixCharStatsForceWavePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Char Stats and DL Force (Wave) skills";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes several character stats values and DL Force (Wave) base and master skills.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2025, 03, 19, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixCharStatsForceWavePlugIn;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var baseStrength = Stats.BaseStrength.GetPersistent(gameConfiguration);
        var baseAgility = Stats.BaseAgility.GetPersistent(gameConfiguration);
        var baseVitality = Stats.BaseVitality.GetPersistent(gameConfiguration);
        var baseEnergy = Stats.BaseEnergy.GetPersistent(gameConfiguration);
        var baseLeadership = Stats.BaseLeadership.GetPersistent(gameConfiguration);
        var totalLevel = Stats.TotalLevel.GetPersistent(gameConfiguration);

        gameConfiguration.CharacterClasses.ForEach(charClass =>
        {
            foreach (var attrCombo in charClass.AttributeCombinations)
            {
                if (attrCombo.TargetAttribute == Stats.FenrirBaseDmg && attrCombo.InputAttribute == Stats.TotalStrength)
                {
                    attrCombo.InputAttribute = baseStrength;
                }
                else if (attrCombo.TargetAttribute == Stats.FenrirBaseDmg && attrCombo.InputAttribute == Stats.TotalAgility)
                {
                    attrCombo.InputAttribute = baseAgility;
                }
                else if (attrCombo.TargetAttribute == Stats.FenrirBaseDmg && attrCombo.InputAttribute == Stats.TotalVitality)
                {
                    attrCombo.InputAttribute = baseVitality;
                }
                else if (attrCombo.TargetAttribute == Stats.FenrirBaseDmg && attrCombo.InputAttribute == Stats.TotalEnergy)
                {
                    attrCombo.InputAttribute = baseEnergy;
                }
                else if (attrCombo.TargetAttribute == Stats.AttackRatePvm && attrCombo.InputAttribute == Stats.Level)
                {
                    attrCombo.InputAttribute = totalLevel;
                }
                else if (attrCombo.TargetAttribute == Stats.AttackRatePvp && attrCombo.InputAttribute == Stats.Level)
                {
                    attrCombo.InputAttribute = totalLevel;
                }
                else if (attrCombo.TargetAttribute == Stats.AttackRatePvp && attrCombo.InputAttribute == Stats.TotalAgility)
                {
                    attrCombo.InputAttribute = baseAgility;
                }
                else if (attrCombo.TargetAttribute == Stats.DefenseRatePvp && attrCombo.InputAttribute == Stats.Level)
                {
                    attrCombo.InputAttribute = totalLevel;
                }
                else if (attrCombo.TargetAttribute == Stats.DefenseRatePvp && attrCombo.InputAttribute == Stats.TotalAgility)
                {
                    attrCombo.InputAttribute = baseAgility;
                }
                else if (attrCombo.TargetAttribute == Stats.MaximumMana && attrCombo.InputAttribute == Stats.Level)
                {
                    attrCombo.InputAttribute = totalLevel;
                }
                else if (attrCombo.TargetAttribute == Stats.MaximumHealth && attrCombo.InputAttribute == Stats.Level)
                {
                    attrCombo.InputAttribute = totalLevel;
                }
                else if (attrCombo.TargetAttribute == Stats.MaximumShieldTemp && attrCombo.InputAttribute == Stats.Level)
                {
                    attrCombo.InputAttribute = totalLevel;
                }
                else
                {
                    // nothing to do
                }
            }

            if (charClass.Number == 16 || charClass.Number == 17) // Lord classes
            {
                charClass.StatAttributes.First(attr => attr.Attribute == Stats.CurrentHealth).BaseValue = 90;
                charClass.StatAttributes.First(attr => attr.Attribute == Stats.CurrentMana).BaseValue = 40;

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.FenrirBaseDmg && attrCombo.InputAttribute == Stats.TotalLeadership) is { } totalLeadershipToFenrirBaseDmg)
                {
                    totalLeadershipToFenrirBaseDmg.InputAttribute = baseLeadership;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.AttackRatePvm && attrCombo.InputAttribute == Stats.TotalAgility) is { } totalAgilityToAttackRatePvm)
                {
                    totalAgilityToAttackRatePvm.InputOperand = 3;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.AttackRatePvm && attrCombo.InputAttribute == Stats.TotalStrength) is { } totalStrengthToAttackRatePvm)
                {
                    totalStrengthToAttackRatePvm.InputOperand = 0.25f;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.MaximumMana && attrCombo.InputAttribute == Stats.TotalLevel) is { } totalLevelToMaximumMana)
                {
                    totalLevelToMaximumMana.InputOperand = 1;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.MaximumHealth && attrCombo.InputAttribute == Stats.TotalLevel) is { } totalLevelToMaximumHealth)
                {
                    totalLevelToMaximumHealth.InputOperand = 1.5f;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.MaximumHealth && attrCombo.InputAttribute == Stats.TotalVitality) is { } totalVitalityoMaximumHealth)
                {
                    totalVitalityoMaximumHealth.InputOperand = 2;
                }
            }
            else if (charClass.Number == 0 || charClass.Number == 2 || charClass.Number == 3) // Wizard classes
            {
                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.DefenseBase && attrCombo.InputAttribute == Stats.TotalAgility) is { } totalAgilityToDefenseBase)
                {
                    totalAgilityToDefenseBase.InputOperand = 0.25f;
                }
            }
            else if (charClass.Number == 12 || charClass.Number == 13) // MG classes
            {
                charClass.StatAttributes.First(attr => attr.Attribute == Stats.BaseEnergy).BaseValue = 26;

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.DefenseBase && attrCombo.InputAttribute == Stats.TotalAgility) is { } totalAgilityToDefenseBase)
                {
                    totalAgilityToDefenseBase.InputOperand = 0.25f;
                }
            }
            else if (charClass.Number == 24 || charClass.Number == 25) // RF classes
            {
                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.DefenseRatePvp && attrCombo.InputAttribute == Stats.BaseAgility) is { } baseAgilityToDefenseRatePvp)
                {
                    baseAgilityToDefenseRatePvp.InputOperand = 0.2f;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.DefenseRatePvp && attrCombo.InputAttribute == Stats.TotalLevel) is { } totalLevelToDefenseRatePvp)
                {
                    totalLevelToDefenseRatePvp.InputOperand = 1.5f;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.AttackRatePvm && attrCombo.InputAttribute == Stats.TotalLevel) is { } totalLevelToAttackRatePvm)
                {
                    totalLevelToAttackRatePvm.InputOperand = 5;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.AttackRatePvm && attrCombo.InputAttribute == Stats.TotalAgility) is { } totalAgilityToAttackRatePvm)
                {
                    totalAgilityToAttackRatePvm.InputOperand = 1.5f;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.AttackRatePvm && attrCombo.InputAttribute == Stats.TotalStrength) is { } totalStrengthToAttackRatePvm)
                {
                    totalStrengthToAttackRatePvm.InputOperand = 0.25f;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.AttackRatePvp && attrCombo.InputAttribute == Stats.BaseAgility) is { } baseAgilityToAttackRatePvp)
                {
                    baseAgilityToAttackRatePvp.InputOperand = 3.6f;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.AttackRatePvp && attrCombo.InputAttribute == Stats.TotalLevel) is { } totalLevelToAttackRatePvp)
                {
                    totalLevelToAttackRatePvp.InputOperand = 2.6f;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.AttackRatePvp && attrCombo.InputAttribute == Stats.TotalEnergy) is { } totalEnergyToAttackRatePvp)
                {
                    charClass.AttributeCombinations.Remove(totalEnergyToAttackRatePvp);
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.FenrirBaseDmg && attrCombo.InputAttribute == Stats.BaseStrength) is { } baseStrengthToFenrirBaseDmg)
                {
                    baseStrengthToFenrirBaseDmg.InputOperand = 1.0f / 5;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.FenrirBaseDmg && attrCombo.InputAttribute == Stats.BaseVitality) is { } baseVitalityToFenrirBaseDmg)
                {
                    baseVitalityToFenrirBaseDmg.InputOperand = 1.0f / 7;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.FenrirBaseDmg && attrCombo.InputAttribute == Stats.BaseEnergy) is { } baseEnergyToFenrirBaseDmg)
                {
                    baseEnergyToFenrirBaseDmg.InputOperand = 1.0f / 3;
                }
            }
            else if (charClass.Number == 20 || charClass.Number == 22 || charClass.Number == 23) // Summoner classes
            {
                charClass.StatAttributes.First(attr => attr.Attribute == Stats.CurrentMana).BaseValue = 40;

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.DefenseBase && attrCombo.InputAttribute == Stats.TotalAgility) is { } totalAgilityToDefenseBase)
                {
                    totalAgilityToDefenseBase.InputOperand = 1.0f / 3;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.DefenseRatePvp && attrCombo.InputAttribute == Stats.BaseAgility) is { } baseAgilityToDefenseRatePvp)
                {
                    baseAgilityToDefenseRatePvp.InputOperand = 0.5f;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.AttackRatePvp && attrCombo.InputAttribute == Stats.BaseAgility) is { } baseAgilityToAttackRatePvp)
                {
                    baseAgilityToAttackRatePvp.InputOperand = 3.5f;
                }
            }
            else
            {
                // nothing to do
            }
        });

        // Update Force Wave skill
        var forceWave = gameConfiguration.Skills.First(me => me.Number == (short)SkillNumber.ForceWave);
        forceWave.ImplicitTargetRange = 1;

        // Create Force Wave Streng alt skill
        this.CreateForceWaveStrengAltSkill(context, gameConfiguration, forceWave);
    }

    private void CreateForceWaveStrengAltSkill(IContext context, GameConfiguration gameConfiguration, Skill regularSkill)
    {
        var skill = context.CreateNew<Skill>();
        gameConfiguration.Skills.Add(skill);
        skill.Number = (short)SkillNumber.ForceWaveStrengAlt;
        skill.Name = "Force Wave Streng (scepter with skill)";
        skill.Range = 4;

        var requirement = context.CreateNew<AttributeRequirement>();
        requirement.Attribute = Stats.CurrentMana.GetPersistent(gameConfiguration);
        requirement.MinimumValue = 15;
        skill.ConsumeRequirements.Add(requirement);

        var lordEmperorClass = gameConfiguration.CharacterClasses.First(c => c.Number == 17);
        skill.QualifiedCharacters.Add(lordEmperorClass);

        skill.SetGuid(skill.Number);

        skill.MasterDefinition = context.CreateNew<MasterSkillDefinition>();
        skill.MasterDefinition.Rank = 2;
        skill.MasterDefinition.Root = gameConfiguration.MasterSkillRoots.First(r => r.Name == "Middle Root");

        var formula = gameConfiguration.Skills.First(me => me.Number == (short)SkillNumber.ForceWaveStreng).MasterDefinition!.ValueFormula;
        skill.MasterDefinition.ValueFormula = formula;
        skill.MasterDefinition.DisplayValueFormula = formula;
        skill.MasterDefinition.MaximumLevel = 20;
        skill.MasterDefinition.TargetAttribute = null;
        skill.MasterDefinition.Aggregation = AggregateType.AddRaw;
        skill.MasterDefinition.ReplacedSkill = regularSkill;
        skill.MasterDefinition.ExtendsDuration = false;
        skill.MasterDefinition.RequiredMasterSkills.Add(gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.Force));
        skill.MasterDefinition.MinimumLevel = 1;

        var replacedSkill = skill.MasterDefinition.ReplacedSkill;
        if (replacedSkill != null)
        {
            skill.AttackDamage = replacedSkill.AttackDamage;
            skill.DamageType = replacedSkill.DamageType;
            skill.ElementalModifierTarget = replacedSkill.ElementalModifierTarget;
            skill.ImplicitTargetRange = replacedSkill.ImplicitTargetRange;
            skill.MovesTarget = replacedSkill.MovesTarget;
            skill.MovesToTarget = replacedSkill.MovesToTarget;
            skill.SkillType = replacedSkill.SkillType;
            skill.Target = replacedSkill.Target;
            skill.TargetRestriction = replacedSkill.TargetRestriction;
            skill.MagicEffectDef = replacedSkill.MagicEffectDef;
        }
    }
}