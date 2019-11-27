// <copyright file="SkillListViewPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="ISkillListViewPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    /// <remarks>
    /// Version 0.97d seems to be compatible to this implementation. This may be true until the end of season 2, but we're not sure.
    /// </remarks>
    [PlugIn("SkillListViewPlugIn 0.75", "The implementation of the ISkillListViewPlugIn for version 0.75 which is forwarding everything to the game client with specific data packets.")]
    [Guid("D83A0CD8-AFEB-4782-8523-AF6D093D14CB")]
    [MaximumClient(2, 255, ClientLanguage.Invariant)]
    public class SkillListViewPlugIn075 : SkillListViewPlugIn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkillListViewPlugIn075"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public SkillListViewPlugIn075(RemotePlayer player)
            : base(player)
        {
        }

        /// <inheritdoc/>
        public override void AddSkill(Skill skill)
        {
            var skillIndex = this.AddSkillToList(skill);
            using var writer = this.Player.Connection.StartSafeWrite(SkillAdded.HeaderType, SkillAdded.Length);
            _ = new SkillAdded075(writer.Span)
            {
                SkillIndex = skillIndex,
                SkillNumberAndLevel = this.GetSkillNumberAndLevel(skill),
            };
            writer.Commit();
        }

        /// <inheritdoc/>
        public override void RemoveSkill(Skill skill)
        {
            var skillIndex = this.SkillList.IndexOf(skill);
            using var writer = this.Player.Connection.StartSafeWrite(SkillRemoved.HeaderType, SkillRemoved.Length);
            _ = new SkillRemoved075(writer.Span)
            {
                SkillIndex = (byte)skillIndex,
                SkillNumberAndLevel = this.GetSkillNumberAndLevel(skill),
            };

            this.SkillList[skillIndex] = null;
            writer.Commit();
        }

        /// <inheritdoc/>
        public override void UpdateSkillList()
        {
            this.BuildSkillList();

            using var writer = this.Player.Connection.StartSafeWrite(SkillListUpdate.HeaderType, SkillListUpdate.GetRequiredSize(this.SkillList.Count));
            var packet = new SkillListUpdate075(writer.Span)
            {
                Count = (byte)this.SkillList.Count,
            };

            for (byte i = 0; i < this.SkillList.Count; i++)
            {
                var skillEntry = packet[i];
                skillEntry.SkillIndex = i;
                skillEntry.SkillNumberAndLevel = this.GetSkillNumberAndLevel(this.SkillList[i]);
            }

            writer.Commit();
        }

        private ushort GetSkillNumberAndLevel(Skill skill)
        {
            ushort result = (ushort)((skill.Number & 0xFF) << 8);

            // The next lines seems strange but is correct. The same part of the skill number is already set in the first byte.
            // Unfortunately it's unclear to us which skill got a level greater than 0 in these early versions.
            // It might be the item level of a weapon with a skill, but this should not be of interest for the client.
            // It could be the type of summoning orb skill, too. For now, we just don't send this level.
            var skillLevel = 0;
            var secondByte = (byte)((skill.Number & 7) | (skillLevel << 3));
            return (ushort)(result + secondByte);
        }
    }
}