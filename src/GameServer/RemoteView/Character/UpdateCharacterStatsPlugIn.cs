// <copyright file="UpdateCharacterStatsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateCharacterStatsPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateCharacterStatsPlugIn", "The default implementation of the IUpdateCharacterStatsPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("6eb967c2-b5a2-4510-9d88-5eccc963a6ea")]
    [MinimumClient(6, 3, ClientLanguage.Invariant)]
    public class UpdateCharacterStatsPlugIn : IUpdateCharacterStatsPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCharacterStatsPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateCharacterStatsPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateCharacterStats()
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC3, 0x48))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x03;
                packet[4] = this.player.Position.X;
                packet[5] = this.player.Position.Y;
                var unsignedMapNumber = ShortExtensions.ToUnsigned(this.player.SelectedCharacter.CurrentMap.Number);
                packet[6] = unsignedMapNumber.GetLowByte();
                packet[7] = unsignedMapNumber.GetHighByte();
                packet.Slice(8).SetLongSmallEndian(this.player.SelectedCharacter.Experience);
                packet.Slice(16).SetLongSmallEndian(this.player.GameServerContext.Configuration.ExperienceTable[(int)this.player.Attributes[Stats.Level] + 1]);
                packet.Slice(24).SetShortBigEndian((ushort)this.player.SelectedCharacter.LevelUpPoints);
                packet.Slice(26).SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseStrength]);
                packet.Slice(28).SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseAgility]);
                packet.Slice(30).SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseVitality]);
                packet.Slice(32).SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseEnergy]);
                packet.Slice(34).SetShortBigEndian((ushort)this.player.Attributes[Stats.CurrentHealth]);
                packet.Slice(36).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumHealth]);
                packet.Slice(38).SetShortBigEndian((ushort)this.player.Attributes[Stats.CurrentMana]);
                packet.Slice(40).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumMana]);
                packet.Slice(42).SetShortBigEndian((ushort)this.player.Attributes[Stats.CurrentShield]);
                packet.Slice(44).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumShield]);
                packet.Slice(46).SetShortBigEndian((ushort)this.player.Attributes[Stats.CurrentAbility]);
                packet.Slice(48).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumAbility]);

                //// 2 missing bytes here are padding
                packet.Slice(52).SetIntegerBigEndian((uint)this.player.Money);
                packet[56] = (byte)this.player.SelectedCharacter.State;
                packet[57] = (byte)this.player.SelectedCharacter.CharacterStatus;
                packet.Slice(58).SetShortBigEndian((ushort)this.player.SelectedCharacter.UsedFruitPoints);
                packet.Slice(60).SetShortBigEndian(this.player.SelectedCharacter.GetMaximumFruitPoints());
                packet.Slice(62).SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseLeadership]);
                packet.Slice(64).SetShortBigEndian((ushort)this.player.SelectedCharacter.UsedNegFruitPoints);
                packet.Slice(66).SetShortBigEndian(this.player.SelectedCharacter.GetMaximumFruitPoints());
                packet[68] = this.player.Account.IsVaultExtended ? (byte)1 : (byte)0;
                //// 3 additional bytes are padding

                writer.Commit();
            }

            if (this.player.SelectedCharacter.CharacterClass.IsMasterClass)
            {
                this.player.ViewPlugIns.GetPlugIn<IUpdateMasterStatsPlugIn>()?.SendMasterStats();
            }

            this.player.ViewPlugIns.GetPlugIn<IApplyKeyConfigurationPlugIn>()?.ApplyKeyConfiguration();
        }
    }
}