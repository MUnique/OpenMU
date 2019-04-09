// <copyright file="LetterReadRequestHandlerPlugIn.cs" company="MUnique">
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
    /// Handler for letter read request packets.
    /// </summary>
    [PlugIn("LetterReadRequestHandlerPlugIn", "Handler for letter read request packets.")]
    [Guid("056ffd3b-567b-4787-9d07-2c9d8a5a7175")]
    internal class LetterReadRequestHandlerPlugIn : IPacketHandlerPlugIn
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(LetterReadRequestHandlerPlugIn));

        private readonly LetterReadRequestAction readAction = new LetterReadRequestAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.FriendMemoReadRequest;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < 6)
            {
                Log.WarnFormat("Player {0}, Unknown Letter Read Request (too short, min size is 6 bytes): {1}", player.SelectedCharacter.Name, packet.AsString());
                return;
            }

            var letterIndex = NumberConversionExtensions.MakeWord(packet[4], packet[5]);
            this.readAction.ReadRequest(player, letterIndex);
        }
    }
}
