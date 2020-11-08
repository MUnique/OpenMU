// <copyright file="MuBotSaveDataRequestHandlerPlugin.cs" company="MUnique">
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
    /// Packet handler for mu bot data save packets (0xAE identifier).
    /// </summary>
    [PlugIn("MuBotSaveDataRequestHandlerPlugin", "Handler for mu bot data save request.")]
    [Guid("3715c03e-9c77-4e43-9f6b-c1db3a2c3233")]
    public class MuBotSaveDataRequestHandlerPlugin : IPacketHandlerPlugIn
    {
        private readonly MuBotSaveDataAction muBotSaveDataAction = new MuBotSaveDataAction();

        /// <inheritdoc />
        public byte Key => MuBotSaveDataRequest.Code;

        /// <inheritdoc />
        public bool IsEncryptionExpected => false;

        /// <inheritdoc />
        public void HandlePacket(Player player, Span<byte> packet)
        {
            MuBotSaveDataRequest message = packet;
            this.muBotSaveDataAction.SaveData(player, message.BotData);
        }
    }
}