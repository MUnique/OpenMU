// <copyright file="FruitConsumptionResultPlugIn097.cs" company="MUnique">
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
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Fruit consumption response plugin for 0.97 clients.
/// </summary>
[PlugIn(nameof(FruitConsumptionResultPlugIn097), "Fruit consumption response plugin for 0.97 clients.")]
[Guid("6BE7F3D4-4A56-4AAB-9E5F-0B1D59A6A169")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class FruitConsumptionResultPlugIn097 : IFruitConsumptionResponsePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="FruitConsumptionResultPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public FruitConsumptionResultPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowResponseAsync(FruitConsumptionResult result, byte statPoints, AttributeDefinition statAttribute)
    {
        var connection = this._player.Connection;
        var selectedCharacter = this._player.SelectedCharacter;
        var attributes = this._player.Attributes;
        if (connection is null || selectedCharacter is null || attributes is null)
        {
            return;
        }

        await connection.SendAsync(() =>
        {
            const int packetLength = 28;
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0x2C;
            span[3] = GetResultByte(result);

            var offset = 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), statPoints);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(selectedCharacter.LevelUpPoints));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.BaseStrength]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.BaseAgility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.BaseVitality]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(attributes[Stats.BaseEnergy]));

            return packetLength;
        }).ConfigureAwait(false);
    }

    private static byte GetResultByte(FruitConsumptionResult result)
    {
        return result switch
        {
            FruitConsumptionResult.PlusSuccess => 0x00,
            FruitConsumptionResult.MinusSuccess => 0x00,
            _ => 0xC0,
        };
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
