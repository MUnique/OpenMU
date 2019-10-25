// <copyright file="SkillListViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="ISkillListViewPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("SkillListViewPlugIn", "The default implementation of the ISkillListViewPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("E67BB791-5BE7-4CC8-B2C9-38E86158A356")]
    [MinimumClient(3, 0, ClientLanguage.Invariant)]
    public class SkillListViewPlugIn : ISkillListViewPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// This contains again all available skills. However, we need this to maintain the indexes. It can happen that the list contains holes after a skill got removed!.
        /// </summary>
        private IList<Skill> skillList;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillListViewPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public SkillListViewPlugIn(RemotePlayer player)
        {
            this.player = player;
        }

        /// <summary>
        /// Gets the internal skill list.
        /// </summary>
        protected IList<Skill> SkillList => this.skillList ??= new List<Skill>();

        /// <summary>
        /// Gets the player.
        /// </summary>
        protected RemotePlayer Player => this.player;

        /// <inheritdoc/>
        public virtual void AddSkill(Skill skill)
        {
            var skillIndex = this.AddSkillToList(skill);
            using var writer = this.player.Connection.StartSafeWrite(SkillAdded.HeaderType, SkillAdded.Length);
            _ = new SkillAdded(writer.Span)
            {
                SkillIndex = skillIndex,
                SkillNumber = (ushort)skill.Number,
            };
            writer.Commit();
        }

        /// <inheritdoc/>
        public virtual void RemoveSkill(Skill skill)
        {
            var skillIndex = this.SkillList.IndexOf(skill);
            using var writer = this.player.Connection.StartSafeWrite(SkillRemoved.HeaderType, SkillRemoved.Length);
            _ = new SkillRemoved(writer.Span)
            {
                SkillIndex = (byte)skillIndex,
                SkillNumber = (ushort)skill.Number,
            };
            this.SkillList[skillIndex] = null;
            writer.Commit();
        }

        /// <inheritdoc/>
        public virtual void UpdateSkillList()
        {
            this.BuildSkillList();

            using var writer = this.player.Connection.StartSafeWrite(SkillListUpdate.HeaderType, SkillListUpdate.GetRequiredSize(this.SkillList.Count));
            var packet = new SkillListUpdate(writer.Span)
            {
                Count = (byte)this.SkillList.Count,
            };

            for (byte i = 0; i < this.SkillList.Count; i++)
            {
                var skillEntry = packet[i];
                skillEntry.SkillIndex = i;

                var skill = this.SkillList[i];
                skillEntry.SkillNumber = (ushort)this.SkillList[i].Number;
                if (skill.MasterDefinition != null)
                {
                    skillEntry.SkillLevel = (byte)this.player.SkillList.GetSkill((ushort)skill.Number).Level;
                }
            }

            writer.Commit();
        }

        /// <inheritdoc />
        public Skill GetSkillByIndex(byte skillIndex)
        {
            if (this.skillList != null && this.skillList.Count > skillIndex)
            {
                return this.skillList[skillIndex];
            }

            return null;
        }

        /// <summary>
        /// Builds the internal skill list, considering duplicates.
        /// </summary>
        protected void BuildSkillList()
        {
            this.SkillList.Clear();
            var skills = this.player.SkillList.Skills.ToList();
            if (this.player.SelectedCharacter.CharacterClass.IsMasterClass)
            {
                var replacedSkills = skills.Select(entry => entry.Skill.MasterDefinition?.ReplacedSkill).Where(skill => skill != null);
                skills.RemoveAll(s => replacedSkills.Contains(s.Skill));
            }

            foreach (var skillEntry in skills.Distinct(default(SkillEqualityComparer)))
            {
                this.SkillList.Add(skillEntry.Skill);
            }
        }

        /// <summary>
        /// Adds the skill to the internal skill list.
        /// </summary>
        /// <param name="skill">The skill to add.</param>
        /// <returns>The index of the added skill.</returns>
        protected byte AddSkillToList(Skill skill)
        {
            for (byte i = 0; i < this.SkillList.Count; i++)
            {
                if (this.SkillList[i] == null)
                {
                    this.SkillList[i] = skill;
                    return i;
                }
            }

            this.SkillList.Add(skill);
            return (byte)(this.SkillList.Count - 1);
        }

        private struct SkillEqualityComparer : IEqualityComparer<SkillEntry>
        {
            public bool Equals(SkillEntry left, SkillEntry right)
            {
                return Equals(left?.Skill, right?.Skill);
            }

            public int GetHashCode(SkillEntry obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}