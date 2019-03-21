// <copyright file="UpdateMasterSkillsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateMasterSkillsPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateMasterSkillsPlugIn", "The default implementation of the IUpdateMasterSkillsPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("72942fe8-925d-43b0-a908-b814b2baa1f3")]
    public class UpdateMasterSkillsPlugIn : IUpdateMasterSkillsPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateMasterSkillsPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateMasterSkillsPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void UpdateMasterSkills()
        {
            var masterSkills = this.player.SkillList.Skills.Where(s => s.Skill.MasterDefinition != null).ToList();

            const int sizePerSkill = 12;
            using (var writer = this.player.Connection.StartSafeWrite(0xC2, 12 + (masterSkills.Count * sizePerSkill)))
            {
                var packet = writer.Span;
                packet[3] = 0xF3;
                packet[4] = 0x53;
                packet.Slice(8).SetIntegerBigEndian((uint)masterSkills.Count);
                var skillsBlock = packet.Slice(12);
                foreach (var masterSkill in masterSkills)
                {
                    skillsBlock[0] = masterSkill.Skill.GetMasterSkillIndex(this.player.SelectedCharacter.CharacterClass);
                    skillsBlock[1] = (byte)masterSkill.Level;
                    //// 2 bytes padding
                    // Instead of using the BitConverter, we should use something more efficient. BitConverter creates an array.
                    BitConverter.GetBytes(masterSkill.CalculateDisplayValue()).CopyTo(skillsBlock.Slice(4, 4));
                    BitConverter.GetBytes(masterSkill.CalculateNextDisplayValue()).CopyTo(skillsBlock.Slice(8, 4));

                    skillsBlock = skillsBlock.Slice(sizePerSkill);
                }

                writer.Commit();
            }
        }
    }
}