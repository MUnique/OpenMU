// <copyright file="ShowGuildListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowGuildListPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowGuildListPlugIn", "The default implementation of the IShowGuildListPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("f72a9968-100b-481f-aba1-1dd597fdad47")]
    public class ShowGuildListPlugIn : IShowGuildListPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowGuildListPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowGuildListPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowGuildList(IEnumerable<OpenMU.Interfaces.GuildListEntry> players)
        {
            const int playerEntryLength = 13;
            var playerCount = players.Count();

            using (var writer = this.player.Connection.StartSafeWrite(0xC2, 6 + 18 + (playerCount * playerEntryLength)))
            {
                var packet = writer.Span;
                packet[3] = 0x52;
                packet[4] = playerCount > 0 ? (byte)1 : (byte)0;
                packet[5] = (byte)playerCount;

                uint totalScore = 0; // TODO
                byte score = 0; // TODO
                string rivalGuildName = "TODO"; // TODO
                packet.Slice(8).SetIntegerBigEndian(totalScore); // 2 bytes are padding (6+7)
                packet[12] = score;
                packet.Slice(13).WriteString(rivalGuildName, Encoding.UTF8);
                //// next 2 bytes are padding (22+23)
                int i = 0;
                foreach (var player in players)
                {
                    var playerBlock = packet.Slice(24 + (i * playerEntryLength), playerEntryLength);
                    playerBlock.WriteString(player.PlayerName, Encoding.UTF8);

                    playerBlock[10] = player.ServerId;
                    playerBlock[11] = (byte)(player.ServerId == 0xFF ? 0x7F : 0x80 + player.ServerId);
                    playerBlock[12] = player.PlayerPosition.GetViewValue();

                    i++;
                }

                writer.Commit();
            }
        }
    }
}