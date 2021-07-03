// <copyright file="ShowAnimationPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowAnimationPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn(nameof(ShowAnimationPlugIn075), "The default implementation of the IShowAnimationPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("AF89AC54-B902-490E-987F-3ED87145A884")]
    [MaximumClient(0, 89, ClientLanguage.Invariant)]
    public class ShowAnimationPlugIn075 : ShowAnimationPlugIn, IShowAnimationPlugIn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowAnimationPlugIn075"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowAnimationPlugIn075(RemotePlayer player)
            : base(player)
        {
        }

        /// <summary>
        /// Gets the monster attack animation id.
        /// </summary>
        protected override byte MonsterAttackAnimation => 100;
    }
}