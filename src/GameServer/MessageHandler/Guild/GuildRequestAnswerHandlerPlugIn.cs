﻿// <copyright file="GuildRequestAnswerHandlerPlugIn.cs" company="MUnique">
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
    /// Handler for guild request answer packets.
    /// </summary>
    [PlugIn("GuildRequestAnswerHandlerPlugIn", "Handler for guild request answer packets.")]
    [Guid("30828257-ec3a-4b99-981d-ec105fc5e82e")]
    internal class GuildRequestAnswerHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly GuildRequestAnswerAction answerAction = new GuildRequestAnswerAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.GuildJoinAnswer;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            bool accept = packet[3] == 1;
            this.answerAction.AnswerRequest(player, accept);
        }
    }
}
