// <copyright file="AnimationHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;

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

            var rotation = packet[3].ParseAsDirection();
            var animation = packet[4];
            player.Rotation = rotation;

            switch (animation)
            {
                case 0x80:
                    player.Pose = CharacterPose.Sitting;
                    break;
                case 0x81:
                    player.Pose = CharacterPose.Leaning;
                    break;
                case 0x82:
                    player.Pose = CharacterPose.Hanging;
                    break;
                default:
                    player.Pose = default;
                    break;
            }

            player.ForEachWorldObserver(o => o.ViewPlugIns.GetPlugIn<IWorldView>().ShowAnimation(player, animation, null, rotation), false);
        }
    }
}
