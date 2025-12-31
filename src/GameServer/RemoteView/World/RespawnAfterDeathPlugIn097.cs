// <copyright file="RespawnAfterDeathPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Respawn plugin for 0.97 clients.
/// </summary>
[PlugIn(nameof(RespawnAfterDeathPlugIn097), "Respawn plugin for 0.97 clients.")]
[Guid("780DF9D2-4E5B-4B3F-B4C2-31B36F60C2F4")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class RespawnAfterDeathPlugIn097 : IRespawnAfterDeathPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="RespawnAfterDeathPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public RespawnAfterDeathPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask RespawnAsync()
    {
        var selectedCharacter = this._player.SelectedCharacter;
        var attributes = this._player.Attributes;
        if (selectedCharacter?.CurrentMap is null || attributes is null)
        {
            return;
        }

        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        var mapNumber = (byte)selectedCharacter.CurrentMap.Number;
        var position = this._player.IsWalking ? this._player.WalkTarget : this._player.Position;

        var currentHealth = GetUShort(attributes[Stats.CurrentHealth]);
        var currentMana = GetUShort(attributes[Stats.CurrentMana]);
        var currentAbility = GetUShort(attributes[Stats.CurrentAbility]);

        await connection.SendRespawnAfterDeath095Async(
            position.X,
            position.Y,
            mapNumber,
            this._player.Rotation.ToPacketByte(),
            currentHealth,
            currentMana,
            currentAbility,
            (uint)selectedCharacter.Experience,
            (uint)this._player.Money).ConfigureAwait(false);
    }

    private static ushort GetUShort(float value)
    {
        if (value <= 0f)
        {
            return 0;
        }

        if (value >= ushort.MaxValue)
        {
            return ushort.MaxValue;
        }

        return (ushort)value;
    }

}
