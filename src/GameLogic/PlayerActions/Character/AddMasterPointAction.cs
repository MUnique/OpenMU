// <copyright file="AddMasterPointAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character;

using MUnique.OpenMU.GameLogic.Views.Character;

/// <summary>
/// Action to add a master skill point to learn or increase the level of a master skill.
/// </summary>
public class AddMasterPointAction
{
    private const int MinimumSkillLevelOfRequiredSkill = 10;

    /// <summary>
    /// Adds the master point.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="skillId">The skill identifier.</param>
    public async ValueTask AddMasterPointAsync(Player player, ushort skillId)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        if (player.SelectedCharacter is null)
        {
            player.Logger.LogWarning("No character selected, player {0}", player);
            return;
        }

        if (player.SelectedCharacter.MasterLevelUpPoints < 1)
        {
            player.Logger.LogWarning("No free master level up point, player {0}", player);
            return;
        }

        var skill = player.GameContext.Configuration.Skills.FirstOrDefault(s => s.Number == skillId);
        if (skill is null)
        {
            player.Logger.LogWarning("Skill {0} does not exist, player {1}", skillId, player);
            return;
        }

        if (skill.MasterDefinition is null)
        {
            player.Logger.LogWarning("Not a master skill, skillId: {0}, player {1}", skill.Number, player);
            return;
        }

        var learnedSkill = player.SelectedCharacter.LearnedSkills.FirstOrDefault(ls => ls.Skill?.Number == skillId);
        if (learnedSkill is null)
        {
            player.Logger.LogDebug("Trying to add master skill, skillId: {0}, player {1}", skill.Number, player);
            if (this.CheckRequisitions(player, skill))
            {
                player.Logger.LogDebug("Adding master skill, skillId: {0}, player {1}", skill.Number, player);
                await player.SkillList!.AddLearnedSkillAsync(skill).ConfigureAwait(false);
                learnedSkill = player.SkillList?.GetSkill(skillId);
                if (learnedSkill is { })
                {
                    await this.AddMasterPointToLearnedSkillAsync(player, learnedSkill).ConfigureAwait(false);
                }
                else
                {
                    player.Logger.LogDebug($"Learned Skill {skillId} not found.");
                }
            }
        }
        else
        {
            await this.AddMasterPointToLearnedSkillAsync(player, learnedSkill).ConfigureAwait(false);
        }
    }

    private async ValueTask AddMasterPointToLearnedSkillAsync(Player player, SkillEntry learnedSkill)
    {
        learnedSkill.ThrowNotInitializedProperty(learnedSkill.Skill is null, nameof(learnedSkill.Skill));
        var requiredPoints = learnedSkill.Level == 0 ? learnedSkill.Skill.MasterDefinition!.MinimumLevel : 1;
        if (player.SelectedCharacter!.MasterLevelUpPoints >= requiredPoints && learnedSkill.Level < learnedSkill.Skill.MasterDefinition!.MaximumLevel)
        {
            player.Logger.LogDebug("Adding {0} points to skill, skillId: {1}, player {2}", requiredPoints, learnedSkill.Skill.Number, player);
            learnedSkill.Level += requiredPoints;
            learnedSkill.PowerUpDuration = null;
            learnedSkill.PowerUps = null;
            player.SelectedCharacter.MasterLevelUpPoints -= requiredPoints;
            await player.InvokeViewPlugInAsync<IMasterSkillLevelChangedPlugIn>(p => p.MasterSkillLevelChangedAsync(learnedSkill)).ConfigureAwait(false);
        }
        else
        {
            player.Logger.LogDebug("Not enough master level up points to add master points, player {0}, available {1}, required {2}", player, player.SelectedCharacter.MasterLevelUpPoints, requiredPoints);
        }
    }

    private bool CheckRequisitions(Player player, Skill skill)
    {
        if (player.SelectedCharacter!.MasterLevelUpPoints < skill.MasterDefinition!.MinimumLevel)
        {
            player.Logger.LogWarning("Not enough master level up points, player {0}, available {1}, required {2}", player, player.SelectedCharacter.MasterLevelUpPoints, skill.MasterDefinition.MinimumLevel);
            return false;
        }

        if (!skill.QualifiedCharacters.Contains(player.SelectedCharacter.CharacterClass!))
        {
            player.Logger.LogWarning("Character not in a qualified class to learn the skill, account {0}, character {1}", player.Account!.LoginName, player.SelectedCharacter.Name);
            return false;
        }

        if (!this.CheckRank(skill.MasterDefinition, player.SelectedCharacter))
        {
            player.Logger.LogWarning("No skill of the previous rank at the required minimum level of {0}, player {1}, skill {2} {3}", MinimumSkillLevelOfRequiredSkill, player, skill.Number, skill.Name);
            return false;
        }

        if (!this.CheckRequiredSkill(skill.MasterDefinition, player))
        {
            player.Logger.LogWarning("Required skill not available of not at the required minimum level of {0}, player {1}, skill {2} {3}", MinimumSkillLevelOfRequiredSkill, player, skill.Number, skill.Name);
            return false;
        }

        return true;
    }

    private bool CheckRank(MasterSkillDefinition definition, DataModel.Entities.Character character)
    {
        if (definition.Rank <= 1)
        {
            return true;
        }

        var learnedRequiredSkill = character.LearnedSkills
            .Where(l => l.Skill?.MasterDefinition?.Root != null)
            .FirstOrDefault(l => l.Skill!.MasterDefinition!.Root!.Id == definition.Root?.Id
                                 && l.Skill.MasterDefinition.Rank == definition.Rank - 1);
        return learnedRequiredSkill?.Level >= MinimumSkillLevelOfRequiredSkill;
    }

    private bool CheckRequiredSkill(MasterSkillDefinition definition, Player player)
    {
        var result = true;
        if (definition.RequiredMasterSkills is not null && definition.RequiredMasterSkills.Any())
        {
            result = definition.RequiredMasterSkills.All(s =>
                player.SelectedCharacter!.LearnedSkills.Any(learned => learned.Skill == s && learned.Level >= MinimumSkillLevelOfRequiredSkill)
                || (s.MasterDefinition is null && player.SkillList!.ContainsSkill((ushort)s.Number)));
        }

        return result;
    }
}