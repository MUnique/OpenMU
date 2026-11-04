// <copyright file="MagicEffectPowerUpExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Extensions which create the power-ups of magic effects for a <see cref="Player"/>,
/// based on its current attribute values.
/// </summary>
public static class MagicEffectPowerUpExtensions
{
    /// <summary>
    /// Creates the magic effect power up for the given skill entry.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="skillEntry">The skill entry.</param>
    public static void CreateMagicEffectPowerUp(this Player player, SkillEntry skillEntry)
    {
        skillEntry.ThrowNotInitializedProperty(skillEntry.Skill is null, nameof(skillEntry.Skill));

        var skill = skillEntry.Skill;

        if (skill.MagicEffectDef?.PowerUpDefinitions.Any(d => d.Boost is null) ?? true)
        {
            throw new InvalidOperationException($"Skill {skill.Name} ({skill.Number}) has no magic effect definition or is without a PowerUpDefinition.");
        }

        if (skill.MagicEffectDef.Duration is null)
        {
            throw new InvalidOperationException($"Skill {skill.Name} ({skill.Number}) has no duration in MagicEffectDef.");
        }

        var result = new (AttributeDefinition Target, IElement BuffPowerUp)[skill.MagicEffectDef.PowerUpDefinitions.Count];
        var resultPvp = new (AttributeDefinition Target, IElement BuffPowerUp)[skill.MagicEffectDef.PowerUpDefinitionsPvp.Count];
        var durationElement = player.Attributes!.CreateDurationElement(skill.MagicEffectDef.Duration);
        var durationElementPvp = skill.MagicEffectDef.DurationPvp is { } durationPvp ? player.Attributes!.CreateDurationElement(durationPvp) : durationElement;
        var chanceElement = skill.MagicEffectDef.Chance is { } chance ? player.Attributes!.CreateChanceElement(chance) : new ConstantElement(1.0f);
        var chanceElementPvp = skill.MagicEffectDef.ChancePvp is { } chancePvp ? player.Attributes!.CreateChanceElement(chancePvp) : chanceElement;
        AddSkillPowersToResult(skill.MagicEffectDef.PowerUpDefinitions, ref result);
        AddSkillPowersToResult(skill.MagicEffectDef.PowerUpDefinitionsPvp, ref resultPvp);
        skillEntry.PowerUpDuration = durationElement;
        skillEntry.PowerUpDurationPvp = durationElementPvp;
        skillEntry.PowerUpChance = chanceElement;
        skillEntry.PowerUpChancePvp = chanceElementPvp;
        skillEntry.PowerUps = result;
        skillEntry.PowerUpsPvp = resultPvp.Count() > 0 ? resultPvp : result;

        if (skillEntry.EnsureSkillAttributes(player.Attributes!) is { } skillAttributes
            && skillAttributes[Stats.SleepStrBonusChance] is float bonusChance
            && bonusChance > 0)
        {
            skillEntry.PowerUpChance = new CombinedElement(skillEntry.PowerUpChance, new ConstantElement(bonusChance));
            skillEntry.PowerUpChancePvp = new CombinedElement(skillEntry.PowerUpChancePvp, new ConstantElement(bonusChance));
        }

        void AddSkillPowersToResult(ICollection<PowerUpDefinition> powerUps, ref (AttributeDefinition Target, IElement BuffPowerUp)[] result)
        {
            if (powerUps.Count() == 0)
            {
                return;
            }

            int i = 0;
            var durationExtended = false;
            foreach (var powerUpDef in powerUps)
            {
                IElement powerUp = player.Attributes!.CreateElement(powerUpDef);
                if (skillEntry.Level > 0)
                {
                    foreach (var masterSkillEntry in GetMasterSkillEntries(skillEntry))
                    {
                        var extendsDuration = masterSkillEntry.Skill?.MasterDefinition?.ExtendsDuration ?? false;
                        if (extendsDuration && !durationExtended)
                        {
                            var value = masterSkillEntry.CalculateValue();
                            if (value < 1)
                            {
                                value *= 100;
                            }

                            durationElement = new CombinedElement(durationElement, new ConstantElement(value));
                            durationElementPvp = new CombinedElement(durationElementPvp, new ConstantElement(value));
                        }

                        if (masterSkillEntry.Skill?.MasterDefinition?.TargetAttribute is not null)
                        {
                            powerUp = AppedMasterSkillPowerUp(masterSkillEntry, powerUpDef, powerUp);
                        }
                    }

                    // After the first iteration all possible duration extensions have been applied
                    durationExtended = true;
                }

                result[i] = (powerUpDef.TargetAttribute!, powerUp);
                i++;
            }
        }

        IEnumerable<SkillEntry> GetMasterSkillEntries(SkillEntry masterSkillEntry)
        {
            yield return masterSkillEntry;

            foreach (var masterSkill in skillEntry.Skill.GetBaseSkills(true))
            {
                yield return player.SkillList!.GetSkill((ushort)masterSkill.Number)!;
            }
        }

        IElement AppedMasterSkillPowerUp(SkillEntry masterSkillEntry, PowerUpDefinition powerUpDef, IElement powerUp)
        {
            var masterSkillDefinition = masterSkillEntry.Skill!.MasterDefinition!;

            if (masterSkillDefinition.TargetAttribute == powerUpDef.TargetAttribute
                && masterSkillDefinition.Aggregation == powerUp.AggregateType)
            {
                var additionalValue = new SimpleElement(masterSkillEntry.CalculateValue(), masterSkillDefinition.Aggregation);
                powerUp = new CombinedElement(powerUp, additionalValue);
            }

            return powerUp;
        }
    }

    /// <summary>
    /// Creates the magic effect power up for the given definition.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="magicEffectDefinition">The definition for a magic effect.</param>
    /// <returns>A tuple containing the duration element and the power-up elements.</returns>
    public static (IElement DurationInSeconds, (AttributeDefinition Target, IElement BuffPowerUp)[] PowerUps) CreateMagicEffectPowerUp(this Player player, MagicEffectDefinition magicEffectDefinition)
    {
        ArgumentNullException.ThrowIfNull(magicEffectDefinition);

        if (magicEffectDefinition.PowerUpDefinitions.Any(d => d.Boost is null))
        {
            throw new InvalidOperationException($"Magic effect definition {magicEffectDefinition.Name} ({magicEffectDefinition.Number}) is without a PowerUpDefinition.");
        }

        if (magicEffectDefinition.Duration is null)
        {
            throw new InvalidOperationException($"Magic effect definition {magicEffectDefinition.Name} ({magicEffectDefinition.Number}) has no duration.");
        }

        int i = 0;
        var result = new (AttributeDefinition Target, IElement BuffPowerUp)[magicEffectDefinition.PowerUpDefinitions.Count];
        foreach (var powerUpDef in magicEffectDefinition.PowerUpDefinitions)
        {
            IElement powerUp = player.Attributes!.CreateElement(powerUpDef);

            result[i] = (powerUpDef.TargetAttribute!, powerUp);
            i++;
        }

        return (player.Attributes!.CreateDurationElement(magicEffectDefinition.Duration), result);
    }
}
