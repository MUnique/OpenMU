// <copyright file="AddMasterPointAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Action to add a master skill point to learn or increase the level of a master skill.
    /// </summary>
    public class AddMasterPointAction
    {
        private const int MaximumSkillLevel = 20;

        private const int MinimumSkillLevelOfRequiredSkill = 10;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(AddMasterPointAction));

        private readonly IGameContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddMasterPointAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public AddMasterPointAction(IGameContext gameContext)
        {
            this.gameContext = gameContext;
        }

        /// <summary>
        /// Adds the master point.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="skillId">The skill identifier.</param>
        public void AddMasterPoint(Player player, ushort skillId)
        {
            if (player.SelectedCharacter == null)
            {
                Log.WarnFormat("No character selected, account: {0}", player.Account.LoginName);
                return;
            }

            if (player.SelectedCharacter.MasterLevelUpPoints < 1)
            {
                Log.WarnFormat("No free master level up point, account {0}, character {1}", player.Account.LoginName, player.SelectedCharacter.Name);
                return;
            }

            Skill skill = this.gameContext.Configuration.Skills.FirstOrDefault(s => s.Number == skillId);
            if (skill == null)
            {
                Log.WarnFormat("Skill {0} does not exist, account: {1}", skillId, player.Account.LoginName);
                return;
            }

            if (skill.MasterDefinitions == null || skill.MasterDefinitions.Count == 0)
            {
                Log.WarnFormat("Not a master skill, skillId: {0}, account: {1}", skill.Number, player.Account);
                return;
            }

            SkillEntry learnedSkill = player.SelectedCharacter.LearnedSkills.FirstOrDefault(ls => ls.Skill.Number == skillId);
            if (learnedSkill == null)
            {
                if (this.CheckRequisitions(player, skill))
                {
                    player.SkillList.AddLearnedSkill(skill);
                    learnedSkill = player.SkillList.GetSkill(skillId);
                    this.AddMasterPointToLearnedSkill(player, learnedSkill);
                }
            }
            else
            {
                this.AddMasterPointToLearnedSkill(player, learnedSkill);
            }
        }

        private void AddMasterPointToLearnedSkill(Player player, SkillEntry learnedSkill)
        {
            if (player.SelectedCharacter.MasterLevelUpPoints > 0 && learnedSkill.Level < MaximumSkillLevel)
            {
                learnedSkill.Level++;
                player.SelectedCharacter.MasterLevelUpPoints--;

                // TODO: Send response, see protocol documentation
            }
        }

        private bool CheckRequisitions(Player player, Skill skill)
        {
            var masterDefs = skill.MasterDefinitions
                .Where(def => def.CharacterClass == player.SelectedCharacter.CharacterClass);

            foreach (var def in masterDefs)
            {
                if (!this.CheckRank(def, player.SelectedCharacter))
                {
                    continue;
                }

                if (this.CheckRequiredSkill(def, player.SelectedCharacter))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckRank(MasterSkillDefinition definition, Character character)
        {
            var result = true;
            if (definition.Rank > 0)
            {
                var lSkill = character.LearnedSkills
                    .Where(learnedSkill => learnedSkill.Skill.MasterDefinitions != null && learnedSkill.Skill.MasterDefinitions.Any())
                    .FirstOrDefault(learnedSkill =>
                  {
                      var mdefs = learnedSkill.Skill.MasterDefinitions.Where(mdef => mdef.CharacterClass == character.CharacterClass);
                      mdefs = mdefs.Where(mdef => mdef.Root.Id == definition.Root.Id);
                      return mdefs.Any(mdef => mdef.Rank == definition.Rank - 1);
                  });
                result = (lSkill != null) && lSkill.Level >= MinimumSkillLevelOfRequiredSkill;
            }

            return result;
        }

        private bool CheckRequiredSkill(MasterSkillDefinition defintion, Character character)
        {
            var result = true;
            if (defintion.RequiredMasterSkills != null && defintion.RequiredMasterSkills.Any())
            {
                result = defintion.RequiredMasterSkills.Any(s => character.LearnedSkills.Any(l => l.Skill == s && (l.Level >= MinimumSkillLevelOfRequiredSkill || l.Skill.MasterDefinitions == null || !l.Skill.MasterDefinitions.Any())));
            }

            return result;
        }
    }
}
