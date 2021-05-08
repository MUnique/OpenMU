﻿// <copyright file="MapChangePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IMapChangePlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("MapChangePlugIn", "The default implementation of the IMapChangePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("234b477d-6fe9-4caa-a03f-78cb25518b39")]
    [MinimumClient(0, 90, ClientLanguage.Invariant)]
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
            this.SendMessage(true);
        }

        /// <inheritdoc/>
        public void MapChangeFailed()
        {
            this.SendMessage(false);
        }

        private void SendMessage(bool success)
        {
            if (this.player.SelectedCharacter?.CurrentMap is null)
            {
                return;
            }

            var mapNumber = this.player.SelectedCharacter.CurrentMap.Number.ToUnsigned();
            var position = this.player.IsWalking ? this.player.WalkTarget : this.player.Position;

            this.player.Connection?.SendMapChanged(mapNumber, position.X, position.Y, this.player.Rotation.ToPacketByte(), success);
        }
    }
}