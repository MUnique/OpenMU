// <copyright file="UpdateStatsPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Buffers.Binary;
using System.Collections.Frozen;
using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Stat update plugin for 0.97 clients.
/// </summary>
[PlugIn(nameof(UpdateStatsPlugIn097), "Stat update plugin for 0.97 clients.")]
[Guid("9914B4A3-AD76-4C3A-9C6F-48FBC9F4525C")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class UpdateStatsPlugIn097 : UpdateStatsBasePlugIn
{
    private static readonly FrozenDictionary<AttributeDefinition, Func<RemotePlayer, ValueTask>> AttributeChangeActions =
        new Dictionary<AttributeDefinition, Func<RemotePlayer, ValueTask>>
        {
            { Stats.CurrentHealth, OnCurrentHealthChangedAsync },
            { Stats.MaximumHealth, OnMaximumHealthChangedAsync },

            { Stats.CurrentMana, OnCurrentManaOrAbilityChangedAsync },
            { Stats.CurrentAbility, OnCurrentManaOrAbilityChangedAsync },
            { Stats.MaximumMana, OnMaximumManaOrAbilityChangedAsync },
            { Stats.MaximumAbility, OnMaximumManaOrAbilityChangedAsync },
        }.ToFrozenDictionary();

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateStatsPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateStatsPlugIn097(RemotePlayer player)
        : base(player, AttributeChangeActions)
    {
    }

    private static ValueTask OnMaximumHealthChangedAsync(RemotePlayer player)
    {
        return SendLifePacketAsync(player, 0xFE, GetUShort(player.Attributes![Stats.MaximumHealth]), ClampToUInt32(player.Attributes[Stats.MaximumHealth]));
    }

    private static ValueTask OnCurrentHealthChangedAsync(RemotePlayer player)
    {
        return SendLifePacketAsync(player, 0xFF, GetUShort(player.Attributes![Stats.CurrentHealth]), ClampToUInt32(player.Attributes[Stats.CurrentHealth]));
    }

    private static ValueTask OnMaximumManaOrAbilityChangedAsync(RemotePlayer player)
    {
        return SendManaPacketAsync(
            player,
            0xFE,
            GetUShort(player.Attributes![Stats.MaximumMana]),
            GetUShort(player.Attributes[Stats.MaximumAbility]),
            ClampToUInt32(player.Attributes[Stats.MaximumMana]),
            ClampToUInt32(player.Attributes[Stats.MaximumAbility]));
    }

    private static ValueTask OnCurrentManaOrAbilityChangedAsync(RemotePlayer player)
    {
        return SendManaPacketAsync(
            player,
            0xFF,
            GetUShort(player.Attributes![Stats.CurrentMana]),
            GetUShort(player.Attributes[Stats.CurrentAbility]),
            ClampToUInt32(player.Attributes[Stats.CurrentMana]),
            ClampToUInt32(player.Attributes[Stats.CurrentAbility]));
    }

    private static ValueTask SendLifePacketAsync(RemotePlayer player, byte type, ushort life, uint viewHp)
    {
        var connection = player.Connection;
        if (connection is null)
        {
            return ValueTask.CompletedTask;
        }

        const int packetLength = 11;
        int WritePacket()
        {
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0x26;
            span[3] = type;
            BinaryPrimitives.WriteUInt16BigEndian(span.Slice(4, 2), life);
            span[6] = 0;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(7, 4), viewHp);
            return packetLength;
        }

        return connection.SendAsync(WritePacket);
    }

    private static ValueTask SendManaPacketAsync(RemotePlayer player, byte type, ushort mana, ushort bp, uint viewMp, uint viewBp)
    {
        var connection = player.Connection;
        if (connection is null)
        {
            return ValueTask.CompletedTask;
        }

        const int packetLength = 16;
        int WritePacket()
        {
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0x27;
            span[3] = type;
            BinaryPrimitives.WriteUInt16BigEndian(span.Slice(4, 2), mana);
            BinaryPrimitives.WriteUInt16BigEndian(span.Slice(6, 2), bp);
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(8, 4), viewMp);
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(12, 4), viewBp);
            return packetLength;
        }

        return connection.SendAsync(WritePacket);
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
}
