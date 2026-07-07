// <copyright file="BotSkillProgressionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Teaches a server-side bot the class attack skills it becomes eligible for as it levels up during play,
/// so its spell repertoire grows over its lifetime just like a real player's. Skills are only ever learned
/// for the character's own class (<see cref="Skill.QualifiedCharacters"/>), gated by a per-skill learn level
/// derived from the skill's attack damage - the same rule the <see cref="BotGenerator"/> applies at creation.
/// </summary>
[PlugIn]
[Display(Name = "Bot skill progression", Description = "Teaches server-side bots new class- and level-appropriate attack skills as they level up.")]
[Guid("D1F4A7C2-6B3E-4A59-8E71-9C0D2F5B6A84")]
public class BotSkillProgressionPlugIn : ICharacterLevelUpPlugIn
{
    /// <summary>
    /// Skill-tier scaling: kept in sync with <see cref="BotGenerator"/> so runtime learning follows the same
    /// progression a freshly generated bot of the new level would already have.
    /// </summary>
    private const double SkillLearnDamageFactor = 0.9;

    /// <inheritdoc />
    public void CharacterLeveledUp(Player player)
    {
        if (player.Account?.IsBot != true
            || player.SelectedCharacter?.CharacterClass is not { } characterClass
            || player.SkillList is not { } skillList)
        {
            return;
        }

        try
        {
            var level = player.Level;
            foreach (var skill in player.GameContext.Configuration.Skills)
            {
                if (skill.AttackDamage <= 0
                    || skill.SkillType is not (SkillType.DirectHit
                        or SkillType.AreaSkillAutomaticHits
                        or SkillType.AreaSkillExplicitHits
                        or SkillType.AreaSkillExplicitTarget)
                    || !skill.QualifiedCharacters.Contains(characterClass)
                    || skillList.ContainsSkill((ushort)skill.Number))
                {
                    continue;
                }

                var learnLevel = Math.Max(1, (int)Math.Ceiling(skill.AttackDamage * SkillLearnDamageFactor));
                if (level < learnLevel)
                {
                    continue;
                }

                // For an offline bot the view invocation is a no-op and the in-memory skill maps are updated
                // synchronously, so the skill is immediately available to the combat AI; the new SkillEntry is
                // persisted by the periodic save (no own SaveChanges here, to avoid extra concurrency pressure).
                _ = skillList.AddLearnedSkillAsync(skill);
                player.Logger.LogInformation("Bot '{Name}' learned '{Skill}' at level {Level}.", player.Name, skill.Name, level);
            }
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Failed to progress bot skills for '{Name}'.", player.Name);
        }
    }
}
