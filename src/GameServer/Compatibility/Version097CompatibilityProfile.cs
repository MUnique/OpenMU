// <copyright file="Version097CompatibilityProfile.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.Compatibility;

using System.Buffers.Binary;
using System.Text;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.RemoteView.Character;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Centralized 0.97 compatibility logic which keeps all packet formats and encodings in one place.
/// </summary>
internal static class Version097CompatibilityProfile
{
    private static readonly Encoding MessageEncoding = Encoding.Latin1;
    private const int MaxChatMessageLength = 241;
    private const int MaxMessageLength = 250;

    public static async ValueTask SendChatMessageAsync(RemotePlayer player, string message, string sender, ChatMessageType type)
    {
        var connection = player.Connection;
        if (connection is null)
        {
            return;
        }

        var messageLength = MessageEncoding.GetByteCount(message);
        if (messageLength > MaxChatMessageLength)
        {
            var trimmedLength = MessageEncoding.GetCharacterCountOfMaxByteCount(message, MaxChatMessageLength);
            message = message.Substring(0, trimmedLength);
            messageLength = MessageEncoding.GetByteCount(message);
        }

        var senderBytes = MessageEncoding.GetBytes(sender);
        var senderLength = Math.Min(senderBytes.Length, 10);

        int WritePacket()
        {
            var packetLength = messageLength + 1 + 13;
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = (byte)(type == ChatMessageType.Whisper ? 0x02 : 0x00);
            span.Slice(3, 10).Clear();
            senderBytes.AsSpan(0, senderLength).CopyTo(span.Slice(3, senderLength));
            span.Slice(13).Clear();
            MessageEncoding.GetBytes(message, span.Slice(13, messageLength));
            return packetLength;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }

    public static async ValueTask SendServerMessageAsync(RemotePlayer player, string message, MessageType messageType)
    {
        if (MessageEncoding.GetByteCount(message) > MaxMessageLength)
        {
            var rest = message;
            while (rest.Length > 0)
            {
                var partSize = MessageEncoding.GetCharacterCountOfMaxByteCount(rest, MaxMessageLength);
                await SendServerMessageAsync(player, rest.Substring(0, partSize), messageType).ConfigureAwait(false);
                rest = rest.Length > partSize ? rest.Substring(partSize) : string.Empty;
            }

            return;
        }

        var content = player.ClientVersion.Season > 0 ? $"000000000{message}" : message;
        await SendMessagePacketAsync(player, ConvertMessageType(messageType), content).ConfigureAwait(false);
    }

    public static ValueTask SendMoneyDropAsync(RemotePlayer player, ushort itemId, bool isFreshDrop, uint amount, Point point)
    {
        return player.Connection.SendMoneyDropped075Async(itemId, isFreshDrop, point.X, point.Y, amount);
    }

    public static async ValueTask SendExperienceAsync(RemotePlayer player, int exp, IAttackable? obj, ExperienceType experienceType)
    {
        var connection = player.Connection;
        var attributes = player.Attributes;
        var selectedCharacter = player.SelectedCharacter;
        if (connection is null || attributes is null || selectedCharacter is null)
        {
            return;
        }

        var remainingExperience = exp;
        ushort damage = 0;
        if (obj is not null && obj.Id != obj.LastDeath?.KillerId)
        {
            damage = (ushort)Math.Min(obj.LastDeath?.FinalHit.HealthDamage ?? 0, ushort.MaxValue);
        }

        var id = (ushort)(obj?.GetId(player) ?? 0);
        if (id != 0)
        {
            id |= 0x8000;
        }

        while (remainingExperience > 0)
        {
            ushort sendExp = remainingExperience > ushort.MaxValue ? ushort.MaxValue : (ushort)remainingExperience;
            var viewExperience = ClampToUInt32(selectedCharacter.Experience);
            var viewNextExperience = ClampToUInt32(player.GameServerContext.ExperienceTable[(int)attributes[Stats.Level] + 1]);
            var viewDamage = (uint)damage;

            int WritePacket()
            {
                const int packetLength = 23;
                var span = connection.Output.GetSpan(packetLength)[..packetLength];
                span[0] = 0xC3;
                span[1] = (byte)packetLength;
                span[2] = 0x9C;
                BinaryPrimitives.WriteUInt16BigEndian(span.Slice(3, 2), id);
                BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(5, 2), 0);
                BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(7, 2), sendExp);
                BinaryPrimitives.WriteUInt16BigEndian(span.Slice(9, 2), damage);
                BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(11, 4), viewDamage);
                BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(15, 4), viewExperience);
                BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(19, 4), viewNextExperience);
                return packetLength;
            }

            await connection.SendAsync(WritePacket).ConfigureAwait(false);
            damage = 0;
            remainingExperience -= sendExp;
        }
    }

    public static ValueTask SendLifePacketAsync(RemotePlayer player, byte type, ushort life, uint viewHp)
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

    public static ValueTask SendManaPacketAsync(RemotePlayer player, byte type, ushort mana, ushort bp, uint viewMp, uint viewBp)
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

    public static async ValueTask SendHitAsync(RemotePlayer player, IAttackable target, HitInfo hitInfo)
    {
        var connection = player.Connection;
        if (connection is null)
        {
            return;
        }

        const int packetLength = 15;
        var targetId = target.GetId(player);
        var damage = (ushort)Math.Min(ushort.MaxValue, hitInfo.HealthDamage);
        var viewCurHp = ClampToUInt32(target.Attributes[Stats.CurrentHealth]);
        var viewDamageHp = ClampToUInt32(hitInfo.HealthDamage);

        int WritePacket()
        {
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0x15;
            BinaryPrimitives.WriteUInt16BigEndian(span.Slice(3, 2), targetId);
            BinaryPrimitives.WriteUInt16BigEndian(span.Slice(5, 2), damage);
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(7, 4), viewCurHp);
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(11, 4), viewDamageHp);
            return packetLength;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }

    public static async ValueTask SendRespawnAsync(RemotePlayer player)
    {
        var selectedCharacter = player.SelectedCharacter;
        var attributes = player.Attributes;
        if (selectedCharacter?.CurrentMap is null || attributes is null)
        {
            return;
        }

        var connection = player.Connection;
        if (connection is null)
        {
            return;
        }

        const int packetLength = 34;
        var mapNumber = (byte)selectedCharacter.CurrentMap.Number;
        var position = player.IsWalking ? player.WalkTarget : player.Position;

        var currentHealth = GetUShort(attributes[Stats.CurrentHealth]);
        var currentMana = GetUShort(attributes[Stats.CurrentMana]);
        var currentAbility = GetUShort(attributes[Stats.CurrentAbility]);

        int WritePacket()
        {
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC3;
            span[1] = (byte)packetLength;
            span[2] = 0xF3;
            span[3] = 0x04;
            span[4] = position.X;
            span[5] = position.Y;
            span[6] = mapNumber;
            span[7] = player.Rotation.ToPacketByte();
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(8, 2), currentHealth);
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(10, 2), currentMana);
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(12, 2), currentAbility);
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(14, 4), (uint)selectedCharacter.Experience);
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(18, 4), (uint)player.Money);
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(22, 4), ClampToUInt32(attributes[Stats.CurrentHealth]));
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(26, 4), ClampToUInt32(attributes[Stats.CurrentMana]));
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(30, 4), ClampToUInt32(attributes[Stats.CurrentAbility]));
            return packetLength;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }

    public static async ValueTask SendLevelUpdateAsync(RemotePlayer player)
    {
        var selectedCharacter = player.SelectedCharacter;
        var charStats = player.Attributes;
        var connection = player.Connection;
        if (selectedCharacter is null || charStats is null || connection is null)
        {
            return;
        }

        const int packetLength = 46;
        var level = GetUShort(charStats[Stats.Level]);
        var nextExperience = (uint)player.GameServerContext.ExperienceTable[(int)charStats[Stats.Level] + 1];

        int WritePacket()
        {
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0xF3;
            span[3] = 0x05;

            var offset = 4;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), level);
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), (ushort)Math.Max(selectedCharacter.LevelUpPoints, 0));
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), GetUShort(charStats[Stats.MaximumHealth]));
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), GetUShort(charStats[Stats.MaximumMana]));
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), GetUShort(charStats[Stats.MaximumAbility]));
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), (ushort)selectedCharacter.UsedFruitPoints);
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), selectedCharacter.GetMaximumFruitPoints());
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), (ushort)selectedCharacter.UsedNegFruitPoints);
            offset += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), selectedCharacter.GetMaximumFruitPoints());
            offset += 2;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), (uint)Math.Max(selectedCharacter.LevelUpPoints, 0));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(charStats[Stats.MaximumHealth]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(charStats[Stats.MaximumMana]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(charStats[Stats.MaximumAbility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), (uint)selectedCharacter.Experience);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), nextExperience);

            return packetLength;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);

        var message = player.GameServerContext.Localization.GetString("Server_Message_LevelUp", (int)charStats[Stats.Level]);
        await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal)).ConfigureAwait(false);
    }

    public static async ValueTask SendStatIncreaseResultAsync(RemotePlayer player, AttributeDefinition attribute, ushort addedPoints)
    {
        var connection = player.Connection;
        var selectedCharacter = player.SelectedCharacter;
        var attributes = player.Attributes;
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

    public static async ValueTask SendFruitConsumptionResultAsync(RemotePlayer player, FruitConsumptionResult result, byte statPoints, AttributeDefinition statAttribute)
    {
        var connection = player.Connection;
        var selectedCharacter = player.SelectedCharacter;
        var attributes = player.Attributes;
        if (connection is null || attributes is null || selectedCharacter is null)
        {
            return;
        }

        const int packetLength = 28;
        var resultByte = GetFruitResultByte(result);

        int WritePacket()
        {
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0x2C;
            span[3] = resultByte;

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
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }

    public static async ValueTask SendCharacterStatsAsync(RemotePlayer player)
    {
        var connection = player.Connection;
        if (connection is null || player.Account is null)
        {
            return;
        }

        await connection.SendCharacterInformation097Async(
            player.Position.X,
            player.Position.Y,
            (byte)player.SelectedCharacter!.CurrentMap!.Number,
            player.Rotation.ToPacketByte(),
            (uint)player.SelectedCharacter.Experience,
            (uint)player.GameServerContext.ExperienceTable[(int)player.Attributes![Stats.Level] + 1],
            (ushort)Math.Max(player.SelectedCharacter.LevelUpPoints, 0),
            (ushort)player.Attributes[Stats.BaseStrength],
            (ushort)player.Attributes[Stats.BaseAgility],
            (ushort)player.Attributes[Stats.BaseVitality],
            (ushort)player.Attributes[Stats.BaseEnergy],
            (ushort)player.Attributes[Stats.CurrentHealth],
            (ushort)player.Attributes[Stats.MaximumHealth],
            (ushort)player.Attributes[Stats.CurrentMana],
            (ushort)player.Attributes[Stats.MaximumMana],
            (ushort)player.Attributes[Stats.CurrentAbility],
            (ushort)player.Attributes[Stats.MaximumAbility],
            (uint)player.Money,
            player.SelectedCharacter.State.Convert(),
            player.SelectedCharacter.CharacterStatus.Convert(),
            (ushort)player.SelectedCharacter.UsedFruitPoints,
            player.SelectedCharacter.GetMaximumFruitPoints(),
            (ushort)player.Attributes[Stats.BaseLeadership])
            .ConfigureAwait(false);

        await SendNewCharacterInfoAsync(player, connection).ConfigureAwait(false);
        await SendNewCharacterCalcAsync(player, connection).ConfigureAwait(false);

        await player.InvokeViewPlugInAsync<IApplyKeyConfigurationPlugIn>(p => p.ApplyKeyConfigurationAsync()).ConfigureAwait(false);
    }

    private static ValueTask SendMessagePacketAsync(RemotePlayer player, ServerMessage.MessageType type, string message)
    {
        var connection = player.Connection;
        if (connection is null)
        {
            return ValueTask.CompletedTask;
        }

        int WritePacket()
        {
            var messageLength = MessageEncoding.GetByteCount(message);
            var packetLength = messageLength + 1 + 4;
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0x0D;
            span[3] = (byte)type;

            span.Slice(4).Clear();
            MessageEncoding.GetBytes(message, span.Slice(4, messageLength));
            return packetLength;
        }

        return connection.SendAsync(WritePacket);
    }

    private static ServerMessage.MessageType ConvertMessageType(MessageType messageType)
    {
        return messageType switch
        {
            MessageType.BlueNormal => ServerMessage.MessageType.BlueNormal,
            MessageType.GoldenCenter => ServerMessage.MessageType.GoldenCenter,
            MessageType.GuildNotice => ServerMessage.MessageType.GuildNotice,
            _ => throw new NotImplementedException($"Case for {messageType} is not implemented."),
        };
    }

    private static byte GetFruitResultByte(FruitConsumptionResult result)
    {
        return result switch
        {
            FruitConsumptionResult.PlusSuccess => 0x00,
            FruitConsumptionResult.MinusSuccess => 0x00,
            _ => 0xC0,
        };
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

    private static async ValueTask SendNewCharacterInfoAsync(RemotePlayer player, IConnection connection)
    {
        const int packetLength = 76;
        var level = ClampToUInt32(player.Attributes![Stats.Level]);
        var nextExperience = (uint)player.GameServerContext.ExperienceTable[(int)player.Attributes[Stats.Level] + 1];

        int WritePacket()
        {
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0xF3;
            span[3] = 0xE0;

            var offset = 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), level);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), (uint)Math.Max(0, player.SelectedCharacter!.LevelUpPoints));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), (uint)player.SelectedCharacter.Experience);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), nextExperience);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.BaseStrength]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.BaseAgility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.BaseVitality]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.BaseEnergy]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.CurrentHealth]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.MaximumHealth]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.CurrentMana]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.MaximumMana]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.CurrentAbility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.MaximumAbility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), (uint)player.SelectedCharacter.UsedFruitPoints);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), player.SelectedCharacter.GetMaximumFruitPoints());
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.Resets]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), 0);

            return packetLength;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }

    private static async ValueTask SendNewCharacterCalcAsync(RemotePlayer player, IConnection connection)
    {
        const int packetLength = 72;
        var wizardryIncrease = player.Attributes![Stats.WizardryAttackDamageIncrease];
        var magicDamageRate = wizardryIncrease > 1f ? ClampToUInt32((wizardryIncrease - 1f) * 100f) : 0;

        int WritePacket()
        {
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0xF3;
            span[3] = 0xE1;

            var offset = 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.CurrentHealth]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.MaximumHealth]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.CurrentMana]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.MaximumMana]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.CurrentAbility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.MaximumAbility]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.AttackSpeed]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.MagicSpeed]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.MinimumPhysBaseDmg]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.MaximumPhysBaseDmg]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.MinimumWizBaseDmg]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.MaximumWizBaseDmg]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), magicDamageRate);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.AttackRatePvm]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), 0);
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.DefenseFinal]));
            offset += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), ClampToUInt32(player.Attributes[Stats.DefenseRatePvm]));

            return packetLength;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }
}
