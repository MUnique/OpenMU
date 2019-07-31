// <copyright file="SkillListViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
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
        /// Gets the size of the skill block.
        /// </summary>
        /// <remarks>Season 6 uses 4 bytes; first for the index, 2 for the skill id, the last one for the (master) level of the skill.</remarks>
        protected virtual int SkillBlockSize => 4;

        /// <summary>
        /// Gets the start index of the skill blocks, after the header with the skill count, where <see cref="WriteSkillBlock"/> is used.
        /// </summary>
        protected virtual int SkillsStartIndex => 6;

        private IList<Skill> SkillList => this.skillList ?? (this.skillList = new List<Skill>());

        /// <inheritdoc/>
        public void AddSkill(Skill skill)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 6 + this.SkillBlockSize))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x11;
                packet[4] = 0xFE;

                byte? skillIndex = null;
                for (byte i = 0; i < this.SkillList.Count; i++)
                {
                    if (this.SkillList[i] == null)
                    {
                        skillIndex = i;
                    }
                }

                if (skillIndex == null)
                {
                    this.SkillList.Add(skill);
                    skillIndex = (byte)(this.SkillList.Count - 1);
                }

                this.WriteSkillBlock(packet.Slice(this.SkillsStartIndex), skill, 0, skillIndex.Value);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void RemoveSkill(Skill skill)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 6 + this.SkillBlockSize))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x11;
                packet[4] = 0xFF;
                this.WriteSkillBlock(packet.Slice(this.SkillsStartIndex), skill, 0, (byte)this.SkillList.IndexOf(skill));
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void UpdateSkillList()
        {
            const int headerSize = 6;
            this.SkillList.Clear();
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, headerSize + (this.SkillBlockSize * this.player.SkillList.SkillCount)))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x11;

                var skills = this.player.SkillList.Skills.ToList();
                if (this.player.SelectedCharacter.CharacterClass.IsMasterClass)
                {
                    var replacedSkills = skills.Select(entry => entry.Skill.MasterDefinition?.ReplacedSkill).Where(skill => skill != null);
                    skills.RemoveAll(s => replacedSkills.Contains(s.Skill));
                }

                byte i = 0;
                foreach (var skillEntry in skills.Distinct(default(SkillEqualityComparer)))
                {
                    this.SkillList.Add(skillEntry.Skill);

                    int offset = this.SkillsStartIndex + (i * this.SkillBlockSize);
                    this.WriteSkillBlock(packet.Slice(offset), skillEntry.Skill, (byte)skillEntry.Level, i);
                    i++;
                }

                packet[4] = i;
                var actualSize = headerSize + (this.SkillBlockSize * i);
                packet.Slice(0, actualSize).SetPacketSize();
                writer.Commit(actualSize);
            }
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
        /// Writes a skill into the specified block.
        /// </summary>
        /// <param name="skillBlock">The skill block.</param>
        /// <param name="skill">The skill.</param>
        /// <param name="skillLevel">The skill level.</param>
        /// <param name="skillIndex">Index of the skill.</param>
        protected virtual void WriteSkillBlock(Span<byte> skillBlock, Skill skill, byte skillLevel, byte skillIndex)
        {
            skillBlock[0] = skillIndex;
            var unsignedSkillId = ShortExtensions.ToUnsigned(skill.Number);
            skillBlock[1] = unsignedSkillId.GetLowByte();
            skillBlock[2] = unsignedSkillId.GetHighByte();

            if (skillBlock.Length > 3)
            {
                skillBlock[3] = skillLevel;
            }
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