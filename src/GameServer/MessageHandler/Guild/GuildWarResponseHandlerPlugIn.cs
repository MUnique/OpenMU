// <copyright file="GuildWarResponseHandlerPlugIn.cs" company="MUnique">
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
    /// Handler for guild master answer packets.
    /// </summary>
    [PlugIn(nameof(GuildWarResponseHandlerPlugIn), "Handler for guild war response packets.")]
    [Guid("50A96257-CD60-420F-A051-9022804241C0")]
    internal class GuildWarResponseHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly GuildWarAnswerAction answerAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => GuildWarResponse.Code;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            GuildWarResponse response = packet;
            this.answerAction.ProcessAnswer(player, response.Accepted);
        }
    }
}