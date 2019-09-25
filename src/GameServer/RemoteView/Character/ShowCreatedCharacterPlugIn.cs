// <copyright file="ShowCreatedCharacterPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowCreatedCharacterPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowCreatedCharacterPlugIn", "The default implementation of the IShowCreatedCharacterPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("f5494592-ff7b-4f7f-b0c1-bd242a69fb8f")]
    public class ShowCreatedCharacterPlugIn : IShowCreatedCharacterPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowCreatedCharacterPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowCreatedCharacterPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowCreatedCharacter(Character character)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x2A))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x01;
                packet[4] = 0x01; // success
                packet.Slice(5).WriteString(character.Name, Encoding.UTF8);
                packet[15] = character.CharacterSlot;
                packet[16] = 0x01;
                packet[17] = 0x00;
                packet[18] = (byte)(character.CharacterClass.Number << 3);
                packet[19] = (byte)character.CharacterStatus;
                packet.Slice(20, 22).Fill(0xFF); // preview data?
                writer.Commit();
            }
        }
    }
}