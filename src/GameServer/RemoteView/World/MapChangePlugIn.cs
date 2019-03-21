// <copyright file="MapChangePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IMapChangePlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("MapChangePlugIn", "The default implementation of the IMapChangePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("234b477d-6fe9-4caa-a03f-78cb25518b39")]
    public class MapChangePlugIn : IMapChangePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapChangePlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public MapChangePlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void MapChange()
        {
            var mapNumber = this.player.SelectedCharacter.CurrentMap.Number.ToUnsigned();
            using (var writer = this.player.Connection.StartSafeWrite(0xC3, 0x0F))
            {
                var packet = writer.Span;
                packet[2] = 0x1C;
                packet[3] = 0x0F;
                packet[4] = 1;
                packet.Slice(5).SetShortSmallEndian(mapNumber);
                var position = this.player.IsWalking ? this.player.WalkTarget : this.player.Position;
                packet[7] = position.X;
                packet[8] = position.Y;
                packet[9] = this.player.Rotation.ToPacketByte();
                writer.Commit();
            }
        }
    }
}