// <copyright file="MuBotUseRequestHandlerPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MuBot
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.MuBot;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for Mu Bot Use Request Handler.
    /// </summary>
    [PlugIn("MuBotUseRequestHandlerPlugin", "Handler for mu bot use request.")]
    [Guid("26d0fef9-8171-4098-87ea-030054163511")]
    [BelongsToGroup(MuBotGroupHandler.GroupKey)]
    public class MuBotUseRequestHandlerPlugin : ISubPacketHandlerPlugIn
    {
        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => MuBotUseRequest.SubCode;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            MuBotUseRequest message = packet;
            new MuBotUseAction(player).UseMuBot(message.Status);
        }
    }
}
