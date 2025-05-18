// <copyright file="RespawnAfterDeathPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IRespawnAfterDeathPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(RespawnAfterDeathPlugIn), "The default implementation of the IRespawnAfterDeathPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("71CE460D-1025-45A2-94B7-46EC651A2664")]
[MinimumClient(2, 0, ClientLanguage.Invariant)]
public class RespawnAfterDeathPlugIn : IRespawnAfterDeathPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="RespawnAfterDeathPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public RespawnAfterDeathPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask RespawnAsync()
    {
        if (this._player.SelectedCharacter?.CurrentMap is null || this._player.Attributes is null)
        {
            return;
        }

        var mapNumber = (byte)this._player.SelectedCharacter.CurrentMap.Number;
        var position = this._player.IsWalking ? this._player.WalkTarget : this._player.Position;
        var isMaster = this._player.SelectedCharacter.CharacterClass?.IsMasterClass is true;
        await this._player.Connection.SendRespawnAfterDeathAsync(
                position.X,
                position.Y,
                mapNumber,
                this._player.Rotation.ToPacketByte(),
                (ushort)this._player.Attributes[Stats.CurrentHealth],
                (ushort)this._player.Attributes[Stats.CurrentMana],
                (ushort)this._player.Attributes[Stats.CurrentShield],
                (ushort)this._player.Attributes[Stats.CurrentAbility],
                (ulong)(isMaster ? this._player.SelectedCharacter.MasterExperience : this._player.SelectedCharacter.Experience),
                (uint)this._player.Money)
            .ConfigureAwait(false);
    }
}