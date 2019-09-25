// <copyright file="UpdateMasterStatsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateMasterStatsPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateMasterStatsPlugIn", "The default implementation of the IUpdateMasterStatsPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("41b27ec2-5bc6-4acf-b395-ddf9e81a3611")]
    public class UpdateMasterStatsPlugIn : IUpdateMasterStatsPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateMasterStatsPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateMasterStatsPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void SendMasterStats()
        {
            var character = this.player.SelectedCharacter;
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x20))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x50;
                var masterLevel = (ushort)this.player.Attributes[Stats.MasterLevel];
                packet.Slice(4).SetShortBigEndian(masterLevel);
                packet.Slice(6).SetLongSmallEndian(character.MasterExperience);
                packet.Slice(14).SetLongSmallEndian(this.player.GameServerContext.Configuration.MasterExperienceTable[masterLevel + 1]);
                packet.Slice(22).SetShortBigEndian((ushort)character.MasterLevelUpPoints);
                packet.Slice(24).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumHealth]);
                packet.Slice(26).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumMana]);
                packet.Slice(28).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumShield]);
                packet.Slice(30).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumAbility]);
                writer.Commit();
            }

            this.player.ViewPlugIns.GetPlugIn<IUpdateMasterSkillsPlugIn>()?.UpdateMasterSkills();
        }
    }
}