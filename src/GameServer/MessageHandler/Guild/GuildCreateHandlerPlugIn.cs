// <copyright file="GuildCreateHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for guild create packets.
    /// </summary>
    [PlugIn("GuildCreateHandlerPlugIn", "Handler for guild create packets.")]
    [Guid("0aae71c1-72df-47d6-af88-cddc5d5c7311")]
    internal class GuildCreateHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly GuildCreateAction createAction = new GuildCreateAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.GuildMasterInfoSave;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            var guildName = packet.ExtractString(4, 9, Encoding.UTF8);
            var guildEmblem = packet.Slice(12, 32).ToArray();
            this.createAction.CreateGuild(player, guildName, guildEmblem);
        }
    }
}
