// <copyright file="GuildView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using Network.Interfaces;

    /// <summary>
    /// The default implementation of the guild view which is forwarding everything to the game client which specific data packets.
    /// </summary>
    public class GuildView : IGuildView
    {
        private const int PlayerEntryLength = 13;

        private readonly Player player;
        private readonly IConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildView" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="player">The player.</param>
        public GuildView(IConnection connection, Player player)
        {
            this.player = player;
            this.connection = connection;
        }

        /// <inheritdoc/>
        public void PlayerLeftGuild(Player player)
        {
            var playerId = player.GetId(this.player);
            if (player.GuildStatus?.Position == GuildPosition.GuildMaster)
            {
                playerId |= 0x80;
            }

            using (var writer = this.connection.StartSafeWrite(0xC1, 0x05))
            {
                var packet = writer.Span;
                packet[2] = 0x5D;
                packet[3] = playerId.GetHighByte();
                packet[4] = playerId.GetLowByte();
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void GuildJoinResponse(GuildRequestAnswerResult result)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 4))
            {
                var packet = writer.Span;
                packet[2] = 0x51;
                packet[3] = (byte)result;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowGuildCreateResult(GuildCreateErrorDetail errorDetail)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 5))
            {
                var packet = writer.Span;
                packet[2] = 0x56;
                packet[3] = (byte)(errorDetail == GuildCreateErrorDetail.None ? 1 : 0);
                packet[4] = (byte)errorDetail;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Shows the guild info, serialized by OpenMU.GameServer.RemoteView.GuildInfoSerializer.
        /// Maybe TODO: Move usage of the serializer to this place, since this is the place where it is needed.
        /// </remarks>
        public void ShowGuildInfo(byte[] guildInfo)
        {
            // guildInfo is the cached, serialized result of the GuildInformation-Class.
            using (var writer = this.connection.StartSafeWrite(guildInfo[0], guildInfo.Length))
            {
                guildInfo.CopyTo(writer.Span);
                writer.Commit();
            }
        }

        /// <inheritdoc />
        public void AssignPlayersToGuild(ICollection<Player> guildPlayers, bool appearsNew)
        {
            // C2 00 11
            // 65
            // 01
            // 34 4B 00 00 80 00 00
            // A4 F2 00 00 00
            const int sizePerPlayer = 12;
            using (var writer = this.connection.StartSafeWrite(0xC2, (guildPlayers.Count * sizePerPlayer) + 5))
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
            using (var writer = this.connection.StartSafeWrite(0xC2, sizePerPlayer + 5))
            {
                var packet = writer.Span;
                packet[3] = 0x65;
                packet[4] = 1; // One player
                this.SetGuildPlayerBlock(packet.Slice(5), guildPlayer, appearsNew);

                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void GuildKickResult(GuildKickSuccess successCode)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 4))
            {
                var packet = writer.Span;

                packet[1] = 0x04;
                packet[2] = 0x53;
                packet[3] = (byte)successCode;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowGuildList(IEnumerable<OpenMU.Interfaces.GuildListEntry> players)
        {
            var playerCount = players.Count();
            using (var writer = this.connection.StartSafeWrite(0xC2, 6 + 18 + (playerCount * PlayerEntryLength)))
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
                    var playerBlock = packet.Slice(24 + (i * PlayerEntryLength), PlayerEntryLength);
                    playerBlock.WriteString(player.PlayerName, Encoding.UTF8);

                    playerBlock[10] = player.ServerId;
                    playerBlock[11] = (byte)(player.ServerId == 0xFF ? 0x7F : 0x80 + player.ServerId);
                    playerBlock[12] = this.PlayerPositionValue(player.PlayerPosition);

                    i++;
                }

                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowGuildJoinRequest(Player requester)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 5))
            {
                var packet = writer.Span;
                packet[2] = 0x50;
                packet[3] = requester.Id.GetHighByte();
                packet[4] = requester.Id.GetLowByte();
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowGuildCreationDialog()
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 3))
            {
                var packet = writer.Span;
                packet[2] = 0x55;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowGuildMasterDialog()
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 3))
            {
                var packet = writer.Span;
                packet[2] = 0x54;
                writer.Commit();
            }
        }

        private void SetGuildPlayerBlock(Span<byte> playerBlock, Player guildPlayer, bool appearsNew)
        {
            playerBlock.SetIntegerBigEndian(guildPlayer.GuildStatus.GuildId);
            playerBlock[4] = this.PlayerPositionValue(guildPlayer.GuildStatus.Position);

            var playerId = guildPlayer.GetId(this.player);
            playerBlock[7] = (byte)(playerId.GetHighByte() | (appearsNew ? 0x80 : 0));
            playerBlock[8] = playerId.GetLowByte();
            ////todo: for alliances there is an extra packet, code 0x67
        }

        private byte PlayerPositionValue(GuildPosition playerPosition)
        {
            switch (playerPosition)
            {
                case GuildPosition.GuildMaster:
                    return 0x80;
                case GuildPosition.NormalMember:
                    return 0x00;
                case GuildPosition.BattleMaster:
                    return 0x20;
                default:
                    throw new ArgumentException(nameof(playerPosition));
            }
        }
    }
}
