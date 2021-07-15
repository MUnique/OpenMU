// <copyright file="GuildInfoRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for guild info request packets.
    /// </summary>
    [PlugIn("GuildInfoRequestHandlerPlugIn", "Handler for guild info request packets.")]
    [Guid("cfea6fcb-0cf4-4c11-8730-3d25ec08b6b0")]
    internal class GuildInfoRequestHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly GuildInfoRequestAction requestAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => GuildInfoRequest.Code;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            GuildInfoRequest request = packet;
            this.requestAction.RequestGuildInfo(player, request.GuildId);
        }
    }
}
