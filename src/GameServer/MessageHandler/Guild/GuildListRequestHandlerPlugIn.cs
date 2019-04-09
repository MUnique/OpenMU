﻿// <copyright file="GuildListRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for guild list request packets.
    /// </summary>
    [PlugIn("GuildListRequestHandlerPlugIn", "Handler for guild list request packets.")]
    [Guid("cf021a55-8ac7-45e4-a1c2-b61eadea9099")]
    internal class GuildListRequestHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly GuildListRequestAction requestAction = new GuildListRequestAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.RequestGuildList;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            this.requestAction.RequestGuildList(player);
        }
    }
}
