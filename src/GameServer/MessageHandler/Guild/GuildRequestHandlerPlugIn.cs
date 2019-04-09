// <copyright file="GuildRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for guild requests.
    /// </summary>
    [PlugIn("GuildRequestHandlerPlugIn", "Handler for guild requests.")]
    [Guid("733b8b1d-7e39-4c5a-b134-d1aac2e33216")]
    internal class GuildRequestHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly GuildRequestAction requestAction = new GuildRequestAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.GuildJoinRequest;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            var guildMasterId = NumberConversionExtensions.MakeWord(packet[4], packet[3]);
            this.requestAction.RequestGuild(player, guildMasterId);
        }
    }
}
