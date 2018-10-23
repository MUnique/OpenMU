// <copyright file="LetterReadRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for letter read request packets.
    /// </summary>
    internal class LetterReadRequestHandler : IPacketHandler
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(LetterReadRequestHandler));

        private readonly LetterReadRequestAction readAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="LetterReadRequestHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public LetterReadRequestHandler(IGameContext gameContext)
        {
            this.readAction = new LetterReadRequestAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet[3] != 0)
            {
                Log.WarnFormat("Player {0} Unknown Letter Read Request: {1}", player.SelectedCharacter.Name, packet.AsString());
                return;
            }

            var letterIndex = NumberConversionExtensions.MakeWord(packet[4], packet[5]);
            this.readAction.ReadRequest(player, letterIndex);
        }
    }
}
