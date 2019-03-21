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
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="ISkillListViewPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("SkillListViewPlugIn", "The default implementation of the ISkillListViewPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("E67BB791-5BE7-4CC8-B2C9-38E86158A356")]
    public class SkillListViewPlugIn : ISkillListViewPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// This contains again all available skills. However, we need this to maintain the indexes. It can happen that the list contains holes after a skill got removed!
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

        private IList<Skill> SkillList => this.skillList ?? (this.skillList = new List<Skill>());

        /// <inheritdoc/>
        public void AddSkill(Skill skill)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x0A))
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

                packet[6] = skillIndex.Value;
                var unsignedSkillId = ShortExtensions.ToUnsigned(skill.Number);
                packet[7] = unsignedSkillId.GetLowByte();
                packet[8] = unsignedSkillId.GetHighByte();
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void RemoveSkill(Skill skill)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x0A))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x11;
                packet[4] = 0xFF;
                packet[6] = (byte)this.SkillList.IndexOf(skill);
                var unsignedSkillId = ShortExtensions.ToUnsigned(skill.Number);
                packet[7] = unsignedSkillId.GetLowByte();
                packet[8] = unsignedSkillId.GetHighByte();
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void UpdateSkillList()
        {
            const int headerSize = 6;
            const int skillBlockSize = 4;
            this.SkillList.Clear();
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, headerSize + (skillBlockSize * this.player.SkillList.SkillCount)))
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
                    int offset = i * 4;
                    packet[6 + offset] = i;
                    this.SkillList.Add(skillEntry.Skill);
                    var unsignedSkillId = ShortExtensions.ToUnsigned(skillEntry.Skill.Number);
                    packet[7 + offset] = unsignedSkillId.GetLowByte();
                    packet[8 + offset] = unsignedSkillId.GetHighByte();
                    packet[9 + offset] = (byte)skillEntry.Level;
                    i++;
                }

                packet[4] = i;
                var actualSize = headerSize + (skillBlockSize * i);
                packet.Slice(0, actualSize).SetPacketSize();
                writer.Commit(actualSize);
            }
        }

        private struct SkillEqualityComparer : IEqualityComparer<SkillEntry>
        {
            public bool Equals(SkillEntry left, SkillEntry right)
            {
                return Equals(left.Skill, right.Skill);
            }

            public int GetHashCode(SkillEntry obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}