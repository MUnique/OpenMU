// <copyright file="GuildKickPlayerHandlerPlugIn.cs" company="MUnique">
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
    /// Handler for guild player kick packets.
    /// </summary>
    [PlugIn("GuildKickPlayerHandlerPlugIn", "Handler for guild player kick packets.")]
    [Guid("ddc7e221-c3a9-47c3-881e-dc59beecc03e")]
    internal class GuildKickPlayerHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly GuildKickPlayerAction kickAction = new GuildKickPlayerAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.GuildKickPlayer;

        /// <inheritdoc/>
        public void HandlePacket(Player guildMaster, Span<byte> packet)
        {
            var nickname = packet.ExtractString(3, 10, Encoding.UTF8);
            var securityCode = packet.ExtractString(13, packet.Length - 13, Encoding.UTF8);

            this.kickAction.KickPlayer(guildMaster, nickname, securityCode);
        }
    }
}
