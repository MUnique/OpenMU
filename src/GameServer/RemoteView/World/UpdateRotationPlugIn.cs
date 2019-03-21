// <copyright file="UpdateRotationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateRotationPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateRotationPlugIn", "The default implementation of the IUpdateRotationPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("2ce3ba17-fd67-4674-88c5-f29c83608310")]
    public class UpdateRotationPlugIn : IUpdateRotationPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRotationPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateRotationPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateRotation()
        {
            //// TODO: Implement Rotation, packet: { 0xc1, 0x04, 0x0F, 0x12 }
        }
    }
}