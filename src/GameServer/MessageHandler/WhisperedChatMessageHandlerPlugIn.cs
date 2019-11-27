// <copyright file="WhisperedChatMessageHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for whispered chat messages.
    /// </summary>
    [PlugIn("Whisper Message Handler", "Packet handler for whispered chat messages.")]
    [Guid("A81C652C-4DAE-477F-A7D0-328924B8A3FC")]
    internal class WhisperedChatMessageHandlerPlugIn : ChatMessageBaseHandlerPlugIn
    {
        /// <inheritdoc />
        public override byte Key => WhisperMessage.Code;

        /// <inheritdoc />
        protected override bool IsWhisper => true;
    }
}