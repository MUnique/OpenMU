// <copyright file="AnimationHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// Handler for animation packets.
    /// </summary>
    internal class AnimationHandler : IPacketHandler
    {
        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            if (packet.Length < 5)
            {
                return;
            }

            var rotation = packet[3];
            var animation = packet[4];
            if (packet[4] == 0x7A)
            {
                player.Rotation = (Direction)rotation;
            }

            player.ObserverLock.EnterReadLock();
            try
            {
                player.Observers.ForEach(o =>
                {
                    if (o != player)
                    {
                        o.WorldView.ShowAnimation(player, animation, null, rotation);
                    }
                });
            }
            finally
            {
                player.ObserverLock.ExitReadLock();
            }
        }
    }
}
