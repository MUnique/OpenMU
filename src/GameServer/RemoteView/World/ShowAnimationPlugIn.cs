// <copyright file="ShowAnimationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowAnimationPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowAnimationPlugIn", "The default implementation of the IShowAnimationPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("d89cbf82-5ac1-423b-a478-f792136fce3c")]
    public class ShowAnimationPlugIn : IShowAnimationPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowAnimationPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowAnimationPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        /// <remarks>
        /// This Packet is sent to the Server when an Object does an animation, including attacking other players.
        /// It will create the animation at the client side.
        /// </remarks>
        public void ShowAnimation(IIdentifiable animatingObj, byte animation, IIdentifiable targetObj, Direction direction)
        {
            var animatingId = animatingObj.GetId(this.player);
            if (targetObj == null)
            {
                using (var writer = this.player.Connection.StartSafeWrite(0xC1, 7))
                {
                    var packet = writer.Span;
                    packet[2] = 0x18;
                    packet[3] = animatingId.GetHighByte();
                    packet[4] = animatingId.GetLowByte();
                    packet[5] = direction.ToPacketByte();
                    packet[6] = animation;
                    writer.Commit();
                }
            }
            else
            {
                var targetId = targetObj.GetId(this.player);
                using (var writer = this.player.Connection.StartSafeWrite(0xC1, 9))
                {
                    var packet = writer.Span;
                    packet[2] = 0x18;
                    packet[3] = animatingId.GetHighByte();
                    packet[4] = animatingId.GetLowByte();
                    packet[5] = direction.ToPacketByte();
                    packet[6] = animation;
                    packet[7] = targetId.GetHighByte();
                    packet[8] = targetId.GetLowByte();
                    writer.Commit();
                }
            }
        }
    }
}