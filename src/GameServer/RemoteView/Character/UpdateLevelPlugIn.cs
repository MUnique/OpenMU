// <copyright file="UpdateLevelPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateLevelPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateLevelPlugIn", "The default implementation of the IUpdateLevelPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("1ff3709e-d99b-4c00-b926-efce281b3997")]
    public class UpdateLevelPlugIn : IUpdateLevelPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateLevelPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateLevelPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateLevel()
        {
            var selectedCharacter = this.player.SelectedCharacter;
            if (selectedCharacter == null)
            {
                return;
            }

            var charStats = this.player.Attributes;
            var level = (ushort)charStats[Stats.Level];
            var levelUpPoints = (ushort)selectedCharacter.LevelUpPoints;
            var maximumHealth = (ushort)charStats[Stats.MaximumHealth];
            var maximumMana = (ushort)charStats[Stats.MaximumMana];
            var maximumShield = (ushort)charStats[Stats.MaximumShield];
            var maximumAbility = (ushort)charStats[Stats.MaximumAbility];
            var fruitPoints = (ushort)selectedCharacter.UsedFruitPoints;
            var maxFruitPoints = selectedCharacter.GetMaximumFruitPoints();
            var negativeFruitPoints = (ushort)selectedCharacter.UsedNegFruitPoints;

            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x18))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x05;
                packet.Slice(4).SetShortBigEndian(level);
                packet.Slice(6).SetShortBigEndian(levelUpPoints);
                packet.Slice(8).SetShortBigEndian(maximumHealth);
                packet.Slice(10).SetShortBigEndian(maximumMana);
                packet.Slice(12).SetShortBigEndian(maximumShield);
                packet.Slice(14).SetShortBigEndian(maximumAbility);
                packet.Slice(16).SetShortBigEndian(fruitPoints);
                packet.Slice(18).SetShortBigEndian(maxFruitPoints);
                packet.Slice(20).SetShortBigEndian(negativeFruitPoints);
                packet.Slice(22).SetShortBigEndian(maxFruitPoints);

                writer.Commit();
            }

            this.player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage($"Congratulations, you are Level {this.player.Attributes[Stats.Level]} now.", MessageType.BlueNormal);
        }
    }
}