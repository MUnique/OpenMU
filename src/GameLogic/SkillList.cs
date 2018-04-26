// <copyright file="SkillList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// The implementation of the skill list, which automatically adds the passive skill power ups to the player.
    /// </summary>
    public class SkillList : ISkillList
    {
        private readonly IDictionary<ushort, SkillEntry> availableSkills;

        private readonly ICollection<SkillEntry> learnedSkills;

        private readonly ICollection<SkillEntry> itemSkills;

        private readonly IDictionary<SkillEntry, IEnumerable<PowerUpWrapper>> passiveSkillPowerUps;

        private readonly Player player;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillList"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public SkillList(Player player)
        {
            this.player = player;
            this.learnedSkills = this.player.SelectedCharacter.LearnedSkills ?? new List<SkillEntry>();
            this.availableSkills = this.learnedSkills.ToDictionary(skillEntry => skillEntry.Skill.SkillID.ToUnsigned());
            this.itemSkills = new List<SkillEntry>();
            this.player.Inventory.EquippedItems
                .Where(item => item.HasSkill)
                .Where(item => item.Definition.Skill != null)
                .ForEach(item => this.AddItemSkill(item.Definition.Skill));
            this.player.Inventory.EquippedItemsChanged += this.Inventory_WearingItemsChanged;
            this.passiveSkillPowerUps = new Dictionary<SkillEntry, IEnumerable<PowerUpWrapper>>();
            foreach (var skill in this.learnedSkills.Where(s => s.Skill.SkillType == SkillType.PassiveBoost))
            {
                this.CreatePowerUpWrappers(skill);
                skill.PropertyChanged += (sender, eventArgs) =>
                {
                    if (eventArgs.PropertyName == "Level")
                    {
                        this.UpdateSkillPassivePowerUp((SkillEntry)sender);
                    }
                };
            }
        }

        /// <inheritdoc/>
        public IEnumerable<SkillEntry> Skills => this.learnedSkills;

        /// <inheritdoc/>
        public byte SkillCount => (byte)this.availableSkills.Count;

        /// <inheritdoc/>
        public SkillEntry GetSkill(ushort skillId)
        {
            this.availableSkills.TryGetValue(skillId, out SkillEntry result);
            return result;
        }

        /// <inheritdoc/>
        public void AddLearnedSkill(Skill skill)
        {
            var skillEntry = this.player.PersistenceContext.CreateNew<SkillEntry>();
            skillEntry.Skill = skill;
            skillEntry.Level = 0;
            this.AddLearnedSkill(skillEntry);
        }

        /// <inheritdoc/>
        public bool RemoveItemSkill(ushort skillId)
        {
            if (this.availableSkills.TryGetValue(skillId, out SkillEntry skillEntry) && this.availableSkills.Remove(skillId))
            {
                return this.itemSkills.Remove(skillEntry);
            }

            return false;
        }

        /// <inheritdoc/>
        public bool ContainsSkill(ushort skillId)
        {
            return this.availableSkills.ContainsKey(skillId);
        }

        private void AddItemSkill(Skill skill)
        {
            var skillEntry = new SkillEntry(); // Create by repository manager? Probably not, but I'm not sure...
            skillEntry.Skill = skill;
            skillEntry.Level = 0;
            this.itemSkills.Add(skillEntry);
            this.availableSkills.Add(skill.SkillID.ToUnsigned(), skillEntry);
        }

        private void AddLearnedSkill(SkillEntry skill)
        {
            this.availableSkills.Add(skill.Skill.SkillID.ToUnsigned(), skill);
            this.learnedSkills.Add(skill);
        }

        private void CreatePowerUpWrappers(SkillEntry skillEntry)
        {
            if (skillEntry.Skill.PassivePowerUps.TryGetValue(skillEntry.Level, out PowerUpDefinition powerUp))
            {
                this.passiveSkillPowerUps.Add(skillEntry, PowerUpWrapper.CreateByPowerUpDefintion(powerUp, this.player.Attributes).ToList());
            }
        }

        private void UpdateSkillPassivePowerUp(SkillEntry skillEntry)
        {
            if (this.passiveSkillPowerUps.TryGetValue(skillEntry, out IEnumerable<PowerUpWrapper> powerUps))
            {
                this.passiveSkillPowerUps.Remove(skillEntry);
                powerUps.ForEach(p => p.Dispose());
                this.CreatePowerUpWrappers(skillEntry);
            }
        }

        private void Inventory_WearingItemsChanged(object sender, ItemEventArgs eventArgs)
        {
            var item = eventArgs.Item;
            if (!item.HasSkill || item.Definition.Skill == null)
            {
                return;
            }

            var inventory = this.player.Inventory;
            if (inventory.EquippedItems.Contains(item))
            {
                this.AddItemSkill(item.Definition.Skill);
            }
            else
            {
                this.RemoveItemSkill(item.Definition.Skill.SkillID.ToUnsigned());
            }
        }
    }
}
