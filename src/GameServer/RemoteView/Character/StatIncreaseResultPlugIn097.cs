// <copyright file="StatIncreaseResultPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Buffers.Binary;
using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Stat increase result plugin for 0.97 clients.
/// </summary>
[PlugIn(nameof(StatIncreaseResultPlugIn097), "Stat increase result plugin for 0.97 clients.")]
[Guid("8B019191-4C4C-4FBD-8A65-1AE0E0490CE3")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class StatIncreaseResultPlugIn097 : IStatIncreaseResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatIncreaseResultPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public StatIncreaseResultPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask StatIncreaseResultAsync(AttributeDefinition attribute, ushort addedPoints)
    {
        var connection = this._player.Connection;
        var selectedCharacter = this._player.SelectedCharacter;
        var attributes = this._player.Attributes;
        if (connection is null || attributes is null || selectedCharacter is null)
        {
            return;
        }

        const int packetLength = 41;
        var maxHealth = (uint)Math.Max(attributes[Stats.MaximumHealth], 0f);
        var maxMana = (uint)Math.Max(attributes[Stats.MaximumMana], 0f);
        var maxBp = (uint)Math.Max(attributes[Stats.MaximumAbility], 0f);
        var result = addedPoints > 0
            ? (byte)(0x10 + (byte)attribute.GetStatType())
            : (byte)0;

        int WritePacket()
        {
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0xF3;
            span[3] = 0x06;
            span[4] = result;

            var maxLifeAndMana = attribute == Stats.BaseVitality ? maxHealth : maxMana;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(5, 2), (ushort)Math.Min(maxLifeAndMana, ushort.MaxValue));
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(7, 2), (ushort)Math.Min(maxBp, ushort.MaxValue));

            var offset = 9;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), (uint)Math.Max(selectedCharacter.LevelUpPoints, 0));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), maxHealth);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), maxMana);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), maxBp);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.BaseStrength]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.BaseAgility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.BaseVitality]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.BaseEnergy]));

            return packetLength;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
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
}
