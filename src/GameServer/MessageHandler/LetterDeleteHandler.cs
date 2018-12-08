// <copyright file="LetterDeleteHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for letter delete packets.
    /// </summary>
    internal class LetterDeleteHandler : IPacketHandler
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(LetterDeleteHandler));

        private readonly LetterDeleteAction deleteAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="LetterDeleteHandler"/> class.
        /// </summary>
        public LetterDeleteHandler()
        {
            this.deleteAction = new LetterDeleteAction();
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet[3] != 0)
            {
                Log.WarnFormat("Player {0} Unknown Letter Delete Request: {1}", player.SelectedCharacter.Name, packet.AsString());
                return;
            }

            var letterIndex = NumberConversionExtensions.MakeWord(packet[4], packet[5]);
            if (letterIndex < player.SelectedCharacter.Letters.Count)
            {
                var letter = player.SelectedCharacter.Letters[letterIndex];
                this.deleteAction.DeleteLetter(player, letter);
            }
        }
    }
}
