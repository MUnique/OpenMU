// <copyright file="LetterSendHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Messenger
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for letter send packets.
    /// </summary>
    [PlugIn("LetterSendHandlerPlugIn", "Handler for letter send packets.")]
    [Guid("6d10d34d-bd20-4dcf-99eb-569d38ef1c1b")]
    internal class LetterSendHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly LetterSendAction sendAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => LetterSendRequest.Code;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            LetterSendRequest message = packet;
            if (packet.Length < 83)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Letter invalid.", MessageType.BlueNormal);
                player.ViewPlugIns.GetPlugIn<ILetterSendResultPlugIn>()?.LetterSendResult(LetterSendSuccess.TryAgain, message.LetterId);
                return;
            }

            this.sendAction.SendLetter(player, message.Receiver, message.Message, message.Title, message.Rotation, message.Animation, message.LetterId);
        }
    }
}
