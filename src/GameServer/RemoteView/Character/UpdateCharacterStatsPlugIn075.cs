// <copyright file="UpdateCharacterStatsPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateCharacterStatsPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateCharacterStatsPlugIn 0.75", "The default implementation of the IUpdateCharacterStatsPlugIn which is forwarding everything to the game client with specific data packets for version 0.75.")]
    [Guid("C180561D-E055-41CA-817C-44E7A985937C")]
    [Client(0, 75, ClientLanguage.Invariant)]
    public class UpdateCharacterStatsPlugIn075 : IUpdateCharacterStatsPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCharacterStatsPlugIn075"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateCharacterStatsPlugIn075(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateCharacterStats()
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC3, 42))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x03;
                packet[4] = this.player.Position.X;
                packet[5] = this.player.Position.Y;
                packet[6] = (byte)this.player.SelectedCharacter.CurrentMap.Number;
                packet.Slice(8).SetIntegerBigEndian((uint)this.player.SelectedCharacter.Experience);
                packet.Slice(12).SetIntegerBigEndian((uint)this.player.GameServerContext.Configuration.ExperienceTable[(int)this.player.Attributes[Stats.Level] + 1]);
                packet.Slice(16).SetShortBigEndian((ushort)this.player.SelectedCharacter.LevelUpPoints);
                packet.Slice(18).SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseStrength]);
                packet.Slice(20).SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseAgility]);
                packet.Slice(22).SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseVitality]);
                packet.Slice(24).SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseEnergy]);
                packet.Slice(26).SetShortBigEndian((ushort)this.player.Attributes[Stats.CurrentHealth]);
                packet.Slice(28).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumHealth]);
                packet.Slice(30).SetShortBigEndian((ushort)this.player.Attributes[Stats.CurrentMana]);
                packet.Slice(32).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumMana]);

                //// 2 missing bytes here are padding
                packet.Slice(36).SetIntegerBigEndian((uint)this.player.Money);
                packet[40] = (byte)this.player.SelectedCharacter.State;
                packet[41] = (byte)this.player.SelectedCharacter.CharacterStatus;
                writer.Commit();
            }

            this.player.ViewPlugIns.GetPlugIn<IApplyKeyConfigurationPlugIn>()?.ApplyKeyConfiguration();
        }
    }
}