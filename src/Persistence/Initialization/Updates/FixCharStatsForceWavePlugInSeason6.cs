// <copyright file="FixCharStatsForceWavePlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes several character stats values and DL Force Wave Strengthener master skill.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("0C1995AB-A1CC-42A8-9EFC-E5FE8F360C53")]
public class FixCharStatsForceWavePlugInSeason6 : FixCharStatsForceWavePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal new const string PlugInName = "Fix Char Stats and DL Force Wave Str skill";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal new const string PlugInDescription = "This update fixes several character stats values and DL Force Wave Strengthener master skill.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixCharStatsForceWaveSeason6;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
        this.UpdateMagicGladiatorClassesStats(gameConfiguration);

        var totalLevel = Stats.TotalLevel.GetPersistent(gameConfiguration);

        gameConfiguration.CharacterClasses.ForEach(charClass =>
        {
            foreach (var attrCombo in charClass.AttributeCombinations)
            {
                if (attrCombo.TargetAttribute == Stats.AttackRatePvm && attrCombo.InputAttribute == Stats.Level)
                {
                    attrCombo.InputAttribute = totalLevel;
                }
                else if (attrCombo.TargetAttribute == Stats.AttackRatePvp && attrCombo.InputAttribute == Stats.Level)
                {
                    attrCombo.InputAttribute = totalLevel;
                }
                else if (attrCombo.TargetAttribute == Stats.DefenseRatePvp && attrCombo.InputAttribute == Stats.Level)
                {
                    attrCombo.InputAttribute = totalLevel;
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
            else if (charClass.Number == 24 || charClass.Number == 25) // RF classes
            {
                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.DefenseRatePvp && attrCombo.InputAttribute == Stats.TotalAgility) is { } totalAgilityToDefenseRatePvp)
                {
                    totalAgilityToDefenseRatePvp.InputOperand = 0.2f;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.DefenseRatePvp && attrCombo.InputAttribute == Stats.TotalLevel) is { } totalLevelToDefenseRatePvp)
                {
                    totalLevelToDefenseRatePvp.InputOperand = 1.5f;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.AttackRatePvm && attrCombo.InputAttribute == Stats.TotalAgility) is { } totalAgilityToAttackRatePvm)
                {
                    totalAgilityToAttackRatePvm.InputOperand = 1.25f;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.AttackRatePvm && attrCombo.InputAttribute == Stats.TotalStrength) is { } totalStrengthToAttackRatePvm)
                {
                    totalStrengthToAttackRatePvm.InputOperand = 1.0f / 6;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.AttackRatePvp && attrCombo.InputAttribute == Stats.TotalAgility) is { } totalAgilityToAttackRatePvp)
                {
                    totalAgilityToAttackRatePvp.InputOperand = 3.6f;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.AttackRatePvp && attrCombo.InputAttribute == Stats.TotalLevel) is { } totalLevelToAttackRatePvp)
                {
                    totalLevelToAttackRatePvp.InputOperand = 2.6f;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.AttackRatePvp && attrCombo.InputAttribute == Stats.TotalEnergy) is { } totalEnergyToAttackRatePvp)
                {
                    charClass.AttributeCombinations.Remove(totalEnergyToAttackRatePvp);
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.FenrirBaseDmg && attrCombo.InputAttribute == Stats.TotalStrength) is { } totalStrengthToFenrirBaseDmg)
                {
                    totalStrengthToFenrirBaseDmg.InputOperand = 1.0f / 5;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.FenrirBaseDmg && attrCombo.InputAttribute == Stats.TotalVitality) is { } totalVitalityToFenrirBaseDmg)
                {
                    totalVitalityToFenrirBaseDmg.InputOperand = 1.0f / 3;
                }
            }
            else if (charClass.Number == 20 || charClass.Number == 22 || charClass.Number == 23) // Summoner classes
            {
                charClass.StatAttributes.First(attr => attr.Attribute == Stats.CurrentMana).BaseValue = 40;

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.DefenseBase && attrCombo.InputAttribute == Stats.TotalAgility) is { } totalAgilityToDefenseBase)
                {
                    totalAgilityToDefenseBase.InputOperand = 1.0f / 3;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.DefenseRatePvp && attrCombo.InputAttribute == Stats.TotalAgility) is { } totalAgilityToDefenseRatePvp)
                {
                    totalAgilityToDefenseRatePvp.InputOperand = 0.5f;
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.AttackRatePvp && attrCombo.InputAttribute == Stats.TotalAgility) is { } totalAgilityToAttackRatePvp)
                {
                    totalAgilityToAttackRatePvp.InputOperand = 3.5f;
                }
            }
            else
            {
                // nothing to do
            }
        });

        // Update Force Wave Strengthener skill
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.ForceWave) is { } forceWave
            && gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.ForceWaveStreng) is { } forceWaveStr)
        {
            forceWaveStr.AttackDamage = forceWave.AttackDamage;
            forceWaveStr.DamageType = forceWave.DamageType;
            forceWaveStr.ElementalModifierTarget = forceWave.ElementalModifierTarget;
            forceWaveStr.ImplicitTargetRange = forceWave.ImplicitTargetRange;
            forceWaveStr.MovesTarget = forceWave.MovesTarget;
            forceWaveStr.MovesToTarget = forceWave.MovesToTarget;
            forceWaveStr.SkillType = forceWave.SkillType;
            forceWaveStr.Target = forceWave.Target;
            forceWaveStr.TargetRestriction = forceWave.TargetRestriction;
            forceWaveStr.MagicEffectDef = forceWave.MagicEffectDef;

            if (forceWave.AreaSkillSettings is { } areaSkillSettings)
            {
                forceWaveStr.AreaSkillSettings = context.CreateNew<AreaSkillSettings>();
                var id = forceWaveStr.AreaSkillSettings.GetId();
                forceWaveStr.AreaSkillSettings.AssignValuesOf(areaSkillSettings, gameConfiguration);
                forceWaveStr.AreaSkillSettings.SetGuid(id);
            }
        }
    }
}