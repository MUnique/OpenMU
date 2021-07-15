// <copyright file="GuildCreateHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for guild create packets.
    /// </summary>
    [PlugIn(nameof(GuildCreateHandlerPlugIn), "Handler for guild create packets.")]
    [Guid("0aae71c1-72df-47d6-af88-cddc5d5c7311")]
    [MinimumClient(0, 90, ClientLanguage.Invariant)]
    internal class GuildCreateHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly GuildCreateAction createAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => GuildCreateRequest.Code;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            GuildCreateRequest request = packet;
            this.createAction.CreateGuild(player, request.GuildName, request.GuildEmblem.ToArray());
        }
    }
}
