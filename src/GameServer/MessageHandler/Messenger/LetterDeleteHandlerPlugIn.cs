﻿// <copyright file="LetterDeleteHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Messenger
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for letter delete packets.
    /// </summary>
    [PlugIn("LetterDeleteHandlerPlugIn", "Handler for letter delete packets.")]
    [Guid("3334483b-2de2-47ff-8d74-7407d3ddf15f")]
    internal class LetterDeleteHandlerPlugIn : IPacketHandlerPlugIn
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(LetterDeleteHandlerPlugIn));

        private readonly LetterDeleteAction deleteAction = new LetterDeleteAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.FriendMemoDelete;

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
