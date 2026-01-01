// <copyright file="RespawnAfterDeathPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Buffers.Binary;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
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
        var viewHp = ClampToUInt32(attributes[Stats.CurrentHealth]);
        var viewMp = ClampToUInt32(attributes[Stats.CurrentMana]);
        var viewBp = ClampToUInt32(attributes[Stats.CurrentAbility]);

        await connection.SendAsync(() =>
        {
            const int packetLength = 34;
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC3;
            span[1] = (byte)packetLength;
            span[2] = 0xF3;
            span[3] = 0x04;
            span[4] = position.X;
            span[5] = position.Y;
            span[6] = mapNumber;
            span[7] = this._player.Rotation.ToPacketByte();
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(8, 2), currentHealth);
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(10, 2), currentMana);
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(12, 2), currentAbility);
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(14, 4), ClampToUInt32(selectedCharacter.Experience));
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(18, 4), ClampToUInt32(this._player.Money));
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(22, 4), viewHp);
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(26, 4), viewMp);
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(30, 4), viewBp);
            return packetLength;
        }).ConfigureAwait(false);
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

    private static uint ClampToUInt32(float value)
    {
        if (value <= 0f)
        {
            return 0;
        }

        if (value >= uint.MaxValue)
        {
            return uint.MaxValue;
        }

        return (uint)value;
    }

    private static uint ClampToUInt32(long value)
    {
        if (value <= 0)
        {
            return 0;
        }

        if (value >= uint.MaxValue)
        {
            return uint.MaxValue;
        }

        return (uint)value;
    }

}
