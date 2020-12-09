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
    /// The default implementation of the <see cref="ITeleportPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn(nameof(TeleportPlugIn), "The default implementation of the ITeleportPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("9506F77B-CA72-4150-87E3-57C889C91F02")]
    public class TeleportPlugIn : ITeleportPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeleportPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public TeleportPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowTeleported()
        {
            var mapNumber = this.player.SelectedCharacter.CurrentMap.Number.ToUnsigned();
            var position = this.player.Position;
            this.player.Connection.SendMapChanged(mapNumber, position.X, position.Y, this.player.Rotation.ToPacketByte(), false);
        }
    }
}