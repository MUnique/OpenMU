// <copyright file="MasterSkillAreaSkillSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Copies the area skill settings of base attack skills to their mastery versions,
/// which is what players actually cast.
/// </summary>
/// <remarks>
/// The generic copy from the replaced skill doesn't cover these pairs, because the
/// replaced strengthener skills have no area skill settings of their own.
/// It's used by the data initialization and by the configuration update for existing
/// databases, so both end up with the same data.
/// </remarks>
internal static class MasterSkillAreaSkillSettings
{
    /// <summary>
    /// The base attack skill and mastery skill pairs whose settings have to match.
    /// </summary>
    private static readonly (SkillNumber BaseSkill, SkillNumber MasterySkill)[] Pairs =
    [
        (SkillNumber.TripleShot, SkillNumber.TripleShotMastery),
    ];

    /// <summary>
    /// Copies the area skill settings of each base skill to its mastery version.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public static void Sync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var (baseSkillNumber, masterySkillNumber) in Pairs)
        {
            var source = gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)baseSkillNumber)?.AreaSkillSettings;
            var target = gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)masterySkillNumber);
            if (source is null || target is null)
            {
                continue;
            }

            CopySettings(context, gameConfiguration, source, target);
        }
    }

    /// <summary>
    /// Copies area skill settings onto the specified skill, preserving the target row identity.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="source">The settings to copy.</param>
    /// <param name="target">The skill receiving the copy.</param>
    public static void CopySettings(IContext context, GameConfiguration gameConfiguration, AreaSkillSettings source, Skill target)
    {
        var targetSettings = target.AreaSkillSettings ?? context.CreateNew<AreaSkillSettings>();
        var id = targetSettings.GetId();
        targetSettings.AssignValuesOf(source, gameConfiguration);
        targetSettings.SetGuid(id);
        target.AreaSkillSettings = targetSettings;
    }
}
