// <copyright file="MasterSkillExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections.Concurrent;
using org.mariuszgromada.math.mxparser;

/// <summary>
/// Extension methods regarding master skills.
/// </summary>
public static class MasterSkillExtensions
{
    private static readonly ConcurrentDictionary<string, float[]> ValueResultCache = new();

    /// <summary>
    /// Calculates the effective value of the specified skill, depending on its formula and level.
    /// </summary>
    /// <param name="skillEntry">The skill entry of the master skill.</param>
    /// <returns>The value of the specified skill, depending on its formula and level.</returns>
    public static float CalculateValue(this SkillEntry skillEntry) => skillEntry.Skill?.MasterDefinition?.CalculateValue(skillEntry.Level) ?? 0;

    /// <summary>
    /// Calculates the display value of the specified skill, depending on its formula and level.
    /// </summary>
    /// <param name="skillEntry">The skill entry of the master skill.</param>
    /// <returns>The value of the specified skill, depending on its formula and level.</returns>
    public static float CalculateDisplayValue(this SkillEntry skillEntry) => skillEntry.Skill?.MasterDefinition?.CalculateDisplayValue(skillEntry.Level) ?? 0;

    /// <summary>
    /// Calculates the next display value of the specified skill, depending on its formula and level.
    /// </summary>
    /// <param name="skillEntry">The skill entry of the master skill.</param>
    /// <returns>The value of the specified skill, depending on its formula and level.</returns>
    public static float CalculateNextDisplayValue(this SkillEntry skillEntry)
    {
        var level = Math.Min(skillEntry.Level + 1, skillEntry.Skill?.MasterDefinition?.MaximumLevel ?? 0);
        return skillEntry.Skill?.MasterDefinition.CalculateDisplayValue(level) ?? 0;
    }

    /// <summary>
    /// Gets the base <see cref="Skill"/> of a <see cref="SkillEntry"/>.
    /// </summary>
    /// <param name="skillEntry">The <see cref="SkillEntry"/>.</param>
    /// <returns>The base <see cref="Skill"/>.</returns>
    public static Skill GetBaseSkill(this SkillEntry skillEntry)
    {
        return skillEntry.Skill!.GetBaseSkill();
    }

    /// <summary>
    /// Gets the base <see cref="Skill"/> of a <see cref="Skill"/>.
    /// </summary>
    /// <param name="skill">The <see cref="Skill"/>.</param>
    /// <returns>The base <see cref="Skill"/>.</returns>
    public static Skill GetBaseSkill(this Skill skill)
    {
        while (skill.MasterDefinition?.ReplacedSkill is { } replacedSkill)
        {
            skill = replacedSkill;
        }

        return skill;
    }

    /// <summary>
    /// Gets the base <see cref="Skill"/>s of a <see cref="Skill"/>.
    /// </summary>
    /// <param name="skill">The <see cref="Skill"/>.</param>
    /// <param name="onlyMasterSkills">If set to <c>true</c>, only master skills are returned.</param>
    /// <returns>The base <see cref="Skill"/>.</returns>
    public static IEnumerable<Skill> GetBaseSkills(this Skill skill, bool onlyMasterSkills = false)
    {
        while (skill.MasterDefinition?.ReplacedSkill is { } replacedSkill)
        {
            if (onlyMasterSkills && replacedSkill.MasterDefinition is null)
            {
                yield break;
            }
            else
            {
                skill = replacedSkill;
                yield return skill;
            }
        }
    }

    private static float CalculateValue(this MasterSkillDefinition? skillDefinition, int level) => skillDefinition?.ValueFormula.GetValue(level, skillDefinition.MaximumLevel) ?? 0;

    private static float CalculateDisplayValue(this MasterSkillDefinition? skillDefinition, int level) => skillDefinition?.DisplayValueFormula.GetValue(level, skillDefinition.MaximumLevel) ?? 0;

    private static float GetValue(this string formula, int level, int maximumLevel)
    {
        if (level <= 0 || level > maximumLevel)
        {
            return 0f;
        }

        if (!ValueResultCache.TryGetValue(formula, out var results))
        {
            results = new float[maximumLevel];

            var argument = new Argument("level", 0);
            var expression = new Expression(formula);
            expression.addArguments(argument);
            for (int i = 0; i < maximumLevel; i++)
            {
                argument.setArgumentValue(i + 1);
                results[i] = (float)expression.calculate();
            }

            ValueResultCache.TryAdd(formula, results);
        }

        return results[level - 1];
    }
}