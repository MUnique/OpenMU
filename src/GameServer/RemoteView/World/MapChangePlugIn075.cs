// <copyright file="MapChangePlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IMapChangePlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn(nameof(MapChangePlugIn075), "The default implementation of the IMapChangePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("88195844-06C7-4EDA-8501-8B75A8B4B3F4")]
    [MaximumClient(0, 89, ClientLanguage.Invariant)]
    public class MapChangePlugIn075 : IMapChangePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapChangePlugIn075"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public MapChangePlugIn075(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void MapChange()
        {
            if (this.player.SelectedCharacter?.CurrentMap is null)
            {
                return;
            }

            var mapNumber = (byte)this.player.SelectedCharacter.CurrentMap.Number;
            var position = this.player.IsWalking ? this.player.WalkTarget : this.player.Position;

            this.player.Connection?.SendMapChanged075(mapNumber, position.X, position.Y, this.player.Rotation.ToPacketByte());
        }

        /// <inheritdoc/>
        public void MapChangeFailed()
        {
            // not implemented?
        }
    }
}