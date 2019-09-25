// <copyright file="AssignPlayersToGuildPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IAssignPlayersToGuildPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("AssignPlayersToGuildPlugIn", "The default implementation of the IAssignPlayersToGuildPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("f42f571e-0cd1-4c22-ba53-8344848ba998")]
    public class AssignPlayersToGuildPlugIn : IAssignPlayersToGuildPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignPlayersToGuildPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public AssignPlayersToGuildPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void AssignPlayersToGuild(ICollection<Player> guildPlayers, bool appearsNew)
        {
            // C2 00 11
            // 65
            // 01
            // 34 4B 00 00 80 00 00
            // A4 F2 00 00 00
            const int sizePerPlayer = 12;
            using (var writer = this.player.Connection.StartSafeWrite(0xC2, (guildPlayers.Count * sizePerPlayer) + 5))
            {
                var packet = writer.Span;
                packet[3] = 0x65;
                packet[4] = (byte)guildPlayers.Count;
                int i = 0;
                foreach (var guildPlayer in guildPlayers)
                {
                    this.SetGuildPlayerBlock(packet.Slice(5 + (i * sizePerPlayer)), guildPlayer, appearsNew);
                    i++;
                }

                writer.Commit();
            }
        }

        /// <inheritdoc />
        public void AssignPlayerToGuild(Player guildPlayer, bool appearsNew)
        {
            // C2 00 11
            // 65
            // 01
            // 34 4B 00 00 80 00 00
            // A4 F2 00 00 00
            const int sizePerPlayer = 12;
            using (var writer = this.player.Connection.StartSafeWrite(0xC2, sizePerPlayer + 5))
            {
                var packet = writer.Span;
                packet[3] = 0x65;
                packet[4] = 1; // One player
                this.SetGuildPlayerBlock(packet.Slice(5), guildPlayer, appearsNew);

                writer.Commit();
            }
        }

        private void SetGuildPlayerBlock(Span<byte> playerBlock, Player guildPlayer, bool appearsNew)
        {
            playerBlock.SetIntegerBigEndian(guildPlayer.GuildStatus.GuildId);
            playerBlock[4] = guildPlayer.GuildStatus.Position.GetViewValue();

            var playerId = guildPlayer.GetId(this.player);
            playerBlock[7] = (byte)(playerId.GetHighByte() | (appearsNew ? 0x80 : 0));
            playerBlock[8] = playerId.GetLowByte();
            ////todo: for alliances there is an extra packet, code 0x67
        }
    }
}