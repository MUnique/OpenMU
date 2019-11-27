// <copyright file="MapChangePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
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
            var position = this.player.IsWalking ? this.player.WalkTarget : this.player.Position;
            using var writer = this.player.Connection.StartSafeWrite(MapChanged.HeaderType, MapChanged.Length);
            _ = new MapChanged(writer.Span)
            {
                MapNumber = mapNumber,
                PositionX = position.X,
                PositionY = position.Y,
                Rotation = this.player.Rotation.ToPacketByte(),
            };

            writer.Commit();
        }
    }
}