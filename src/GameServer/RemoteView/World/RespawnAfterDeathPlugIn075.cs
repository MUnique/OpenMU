// <copyright file="RespawnAfterDeathPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IRespawnAfterDeathPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn(nameof(RespawnAfterDeathPlugIn075), "The default implementation of the IRespawnAfterDeathPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("FE2D99D4-CA19-4E94-BDF1-B51B463AD28A")]
    [MaximumClient(0, 89, ClientLanguage.Invariant)]
    public class RespawnAfterDeathPlugIn075 : IRespawnAfterDeathPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="RespawnAfterDeathPlugIn075"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public RespawnAfterDeathPlugIn075(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void Respawn()
        {
            if (this.player.SelectedCharacter?.CurrentMap is null || this.player.Attributes is null)
            {
                return;
            }

            var mapNumber = (byte)this.player.SelectedCharacter.CurrentMap.Number;
            var position = this.player.IsWalking ? this.player.WalkTarget : this.player.Position;

            this.player.Connection?.SendRespawnAfterDeath075(
                position.X,
                position.Y,
                mapNumber,
                this.player.Rotation.ToPacketByte(),
                (ushort)this.player.Attributes[Stats.CurrentHealth],
                (ushort)this.player.Attributes[Stats.CurrentMana],
                (uint)this.player.SelectedCharacter.Experience,
                (uint)this.player.Money);
        }
    }
}