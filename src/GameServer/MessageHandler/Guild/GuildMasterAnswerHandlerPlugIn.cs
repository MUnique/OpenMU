// <copyright file="GuildMasterAnswerHandlerPlugIn.cs" company="MUnique">
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
    /// Handler for guild master answer packets.
    /// </summary>
    [PlugIn("GuildMasterAnswerHandlerPlugIn", "Handler for guild master answer packets.")]
    [Guid("3715c03e-9c77-4e43-9f6b-c1db3a2c3233")]
    internal class GuildMasterAnswerHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly GuildMasterAnswerAction answerAction = new GuildMasterAnswerAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.GuildMasterAnswer;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            var answer = (GuildMasterAnswerAction.Answer)packet[3];
            this.answerAction.ProcessAnswer(player, answer);
        }
    }
}
