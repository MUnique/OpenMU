// <copyright file="UpdateCharacterHeroStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateCharacterHeroStatePlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateCharacterHeroStatePlugIn", "The default implementation of the IUpdateCharacterHeroStatePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("d1ce36d6-cbdd-4bcb-99c7-c7495d8597d9")]
    public class UpdateCharacterHeroStatePlugIn : IUpdateCharacterHeroStatePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCharacterHeroStatePlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateCharacterHeroStatePlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateCharacterHeroState()
        {
            var playerId = this.player.GetId(this.player);
            var characterHeroState = this.player.SelectedCharacter.State;

            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x07))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x08;
                packet[4] = playerId.GetHighByte();
                packet[5] = playerId.GetLowByte();
                packet[6] = (byte)characterHeroState;
                writer.Commit();
            }
        }
    }
}