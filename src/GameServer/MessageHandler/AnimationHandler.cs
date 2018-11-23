// <copyright file="AnimationHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// Handler for animation packets.
    /// </summary>
    internal class AnimationHandler : IPacketHandler
    {
        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < 5)
            {
                return;
            }

            var rotation = (Direction)(packet[3] + 1);
            var animation = packet[4];
            if (packet[4] == 0x7A)
            {
                player.Rotation = rotation;
            }

            player.ForEachObservingPlayer(o => o.WorldView.ShowAnimation(player, animation, null, rotation), false);
        }
    }
}
