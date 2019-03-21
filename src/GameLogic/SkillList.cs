// <copyright file="SkillList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;

    /// <summary>
    /// The implementation of the skill list, which automatically adds the passive skill power ups to the player.
    /// </summary>
    public class SkillList : ISkillList
    {
        private readonly IDictionary<ushort, SkillEntry> availableSkills;

        private readonly ICollection<SkillEntry> learnedSkills;

        private readonly ICollection<SkillEntry> itemSkills;

        private readonly Player player;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillList"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public SkillList(Player player)
        {
            this.player = player;
            this.learnedSkills = this.player.SelectedCharacter.LearnedSkills ?? new List<SkillEntry>();
            this.availableSkills = this.learnedSkills.ToDictionary(skillEntry => skillEntry.Skill.Number.ToUnsigned());
            this.itemSkills = new List<SkillEntry>();
            this.player.Inventory.EquippedItems
                .Where(item => item.HasSkill)
                .Where(item => item.Definition.Skill != null)
                .ForEach(item => this.AddItemSkill(item.Definition.Skill));
            this.player.Inventory.EquippedItemsChanged += this.Inventory_WearingItemsChanged;
            foreach (var skill in this.learnedSkills.Where(s => s.Skill.SkillType == SkillType.PassiveBoost))
            {
                this.CreatePowerUpForPassiveSkill(skill);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<SkillEntry> Skills => this.availableSkills.Values;

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
            this.availableSkills.TryGetValue(skillId, out SkillEntry skillEntry);
            if (skillEntry == null)
            {
                return false;
            }

            // We need to take into account that we there might be multiple items equipped with the same skill
            var skillRemoved = this.itemSkills.Remove(skillEntry);
            if (skillRemoved && this.itemSkills.All(s => s.Skill.Number != skillId))
            {
                this.player.ViewPlugIns.GetPlugIn<ISkillListViewPlugIn>()?.RemoveSkill(skillEntry.Skill);
                this.availableSkills.Remove(skillId);
            }

            return true;
        }

        /// <inheritdoc/>
        public bool ContainsSkill(ushort skillId)
        {
            return this.availableSkills.ContainsKey(skillId);
        }

        private void AddItemSkill(Skill skill)
        {
            var skillEntry = new SkillEntry();
            skillEntry.Skill = skill;
            skillEntry.Level = 0;
            this.itemSkills.Add(skillEntry);

            // Item skills are always level 0, so it doesn't matter which one is added to the dictionary.
            if (!this.ContainsSkill((ushort)skill.Number))
            {
                this.availableSkills.Add(skill.Number.ToUnsigned(), skillEntry);
                this.player.ViewPlugIns.GetPlugIn<ISkillListViewPlugIn>()?.AddSkill(skill);
            }
        }

        private void AddLearnedSkill(SkillEntry skill)
        {
            this.availableSkills.Add(skill.Skill.Number.ToUnsigned(), skill);
            this.learnedSkills.Add(skill);

            this.player.ViewPlugIns.GetPlugIn<ISkillListViewPlugIn>()?.AddSkill(skill.Skill);
            if (skill.Skill.SkillType == SkillType.PassiveBoost)
            {
                this.CreatePowerUpForPassiveSkill(skill);
            }
        }

        private void CreatePowerUpForPassiveSkill(SkillEntry skillEntry)
        {
            this.CreatePowerUpWrappers(skillEntry);
        }

        private void CreatePowerUpWrappers(SkillEntry skillEntry)
        {
            var masterDefinition = skillEntry.Skill.MasterDefinition;
            if (masterDefinition == null)
            {
                return;
            }

            if (masterDefinition.TargetAttribute == null)
            {
                // log?
                return;
            }

            // maybe to do: We don't need to hold it, as it's added to the player attributes.
            new PowerUpWrapper(new PassiveSkillBoostPowerUp(skillEntry), masterDefinition.TargetAttribute, this.player.Attributes);
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
                this.RemoveItemSkill(item.Definition.Skill.Number.ToUnsigned());
            }
        }

        private sealed class PassiveSkillBoostPowerUp : IElement
        {
            public PassiveSkillBoostPowerUp(SkillEntry skillEntry)
            {
                this.Value = skillEntry.CalculateValue();
                this.AggregateType = skillEntry.Skill.MasterDefinition.Aggregation;
                skillEntry.PropertyChanged += (sender, eventArgs) =>
                {
                    if (eventArgs.PropertyName == nameof(SkillEntry.Level))
                    {
                        this.Value = skillEntry.CalculateValue();
                        this.ValueChanged?.Invoke(this, EventArgs.Empty);
                    }
                };
            }

            public event EventHandler ValueChanged;

            public float Value { get; private set; }

            public AggregateType AggregateType { get; }
        }
    }
}
