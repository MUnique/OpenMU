// <copyright file="GuildView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using GameLogic.Views;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.Network;

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
            if (player.SelectedCharacter.GuildMemberInfo.Status == GuildPosition.GuildMaster)
            {
                playerId |= 0x80;
            }

            this.connection.Send(new byte[] { 0xC1, 0x05, 0x5D, playerId.GetHighByte(), playerId.GetLowByte() });
        }

        /// <inheritdoc/>
        public void GuildJoinResponse(GuildRequestAnswerResult result)
        {
            this.connection.Send(new byte[] { 0xC1, 0x04, 0x51, (byte)result });
        }

        /// <inheritdoc/>
        public void ShowGuildCreateResult(GuildCreateErrorDetail errorDetail)
        {
            this.connection.Send(new byte[] { 0xC1, 0x05, 0x56, (byte)(errorDetail == GuildCreateErrorDetail.None ? 1 : 0), (byte)errorDetail });
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Shows the guild info, serialized by OpenMU.GameServer.RemoteView.GuildInfoSerializer.
        /// Maybe TODO: Move usage of the serializer to this place, since this is the place where it is needed.
        /// </remarks>
        public void ShowGuildInfo(byte[] guildInfo)
        {
            this.connection.Send(guildInfo); // guildInfo is the cached, serialized result of the GuildInformation-Class.
        }

        /// <inheritdoc />
        public void AssignPlayersToGuild(ICollection<Player> guildPlayers, bool appearsNew)
        {
            // C2 00 11
            // 65
            // 01
            // 34 4A 00 00 80 00 00
            // A4 F2 00 00 00
            var array = new byte[(guildPlayers.Count * 12) + 5];
            array[0] = 0xC2;
            array[1] = ((ushort)array.Length).GetHighByte();
            array[2] = ((ushort)array.Length).GetLowByte();
            array[3] = 0x65;
            array[4] = (byte)guildPlayers.Count;
            int i = 0;
            foreach (var guildPlayer in guildPlayers)
            {
                var offset = 5 + (i * 12);
                var playerId = guildPlayer.GetId(this.player);
                var memberInfo = guildPlayer.SelectedCharacter.GuildMemberInfo;
                array[offset] = guildPlayer.ShortGuildID.GetHighByte();
                array[offset + 1] = guildPlayer.ShortGuildID.GetLowByte();
                array[offset + 4] = (byte)memberInfo.Status;
                array[offset + 7] = (byte)(playerId.GetHighByte() | (appearsNew ? 0x80 : 0));
                array[offset + 8] = playerId.GetLowByte();
                ////todo: alliance id somewhere

                i++;
            }
        }

        /// <inheritdoc/>
        public void GuildKickResult(GuildKickSuccess successCode)
        {
            this.connection.Send(new byte[] { 0xC1, 0x04, 0x53, (byte)successCode });
        }

        /// <inheritdoc/>
        public void ShowGuildList(IEnumerable<OpenMU.Interfaces.GuildListEntry> players)
        {
            var playerCount = players.Count();
            byte[] packet = new byte[6 + 18 + (playerCount * PlayerEntryLength)];
            packet[0] = 0xC2;
            packet[1] = ((ushort)packet.Length).GetHighByte();
            packet[2] = ((ushort)packet.Length).GetLowByte();
            packet[3] = 0x52;
            packet[4] = playerCount > 0 ? (byte)1 : (byte)0;
            packet[5] = (byte)playerCount;

            ////TODO: 18 bytes missing here... guild logo?
            // next 4 bytes "TotalScore"
            // next byte "Score"
            // next 9 bytes "RivalGuild"
            int i = 0;
            foreach (var player in players)
            {
                Encoding.ASCII.GetBytes(player.PlayerName, 0, player.PlayerName.Length, packet, 23 + (i * PlayerEntryLength));
                packet[34 + (i * PlayerEntryLength)] = player.ServerId;
                packet[35 + (i * PlayerEntryLength)] = (byte)player.PlayerPosition;
                i++;
            }

            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void ShowGuildJoinRequest(Player requester)
        {
            this.connection.Send(new byte[] { 0xC1, 0x05, 0x50, requester.Id.GetHighByte(), requester.Id.GetLowByte() });
        }

        /// <inheritdoc/>
        public void ShowGuildCreationDialog()
        {
            this.connection.Send(new byte[] { 0xC1, 3, 0x55 });
        }
    }
}
