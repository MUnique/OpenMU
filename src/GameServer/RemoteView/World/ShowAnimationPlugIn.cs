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
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowAnimationPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn(nameof(ShowAnimationPlugIn), "The default implementation of the IShowAnimationPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("d89cbf82-5ac1-423b-a478-f792136fce3c")]
    [MinimumClient(0, 90, ClientLanguage.Invariant)]
    public class ShowAnimationPlugIn : IShowAnimationPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowAnimationPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowAnimationPlugIn(RemotePlayer player) => this.player = player;

        /// <summary>
        /// Gets the monster attack animation id.
        /// </summary>
        protected virtual byte MonsterAttackAnimation => 120;

        /// <inheritdoc/>
        /// <remarks>
        /// This Packet is sent to the Server when an Object does an animation, including attacking other players.
        /// It will create the animation at the client side.
        /// </remarks>
        public void ShowAnimation(IIdentifiable animatingObj, byte animation, IIdentifiable? targetObj, Direction direction)
        {
            var animatingId = animatingObj.GetId(this.player);
            var targetId = targetObj?.GetId(this.player) ?? 0;
            this.player.Connection?.SendObjectAnimation(animatingId, direction.ToPacketByte(), animation, targetId);
        }

        /// <inheritdoc/>
        public void ShowMonsterAttackAnimation(IIdentifiable animatingObj, IIdentifiable? targetObj, Direction direction)
        {
            this.ShowAnimation(animatingObj, this.MonsterAttackAnimation, targetObj, direction);
        }
    }
}