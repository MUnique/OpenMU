// <copyright file="ChatMessageHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for chat messages.
    /// </summary>
    [PlugIn("Chat Message Handler", "Packet handler for chat messages.")]
    [Guid("EDECCEC6-9DC7-499F-8658-EAF94498BDEE")]
    internal class ChatMessageHandlerPlugIn : ChatMessageBaseHandlerPlugIn
    {
        /// <inheritdoc />
        public override byte Key => PublicChatMessage.Code;

        /// <inheritdoc />
        protected override bool IsWhisper => false;
    }
}