// <copyright file="KanturuConnectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets.ServerToClient;

using MUnique.OpenMU.Network;

/// <summary>
/// Extension methods to send Kanturu event packets (0xD1 group) on a <see cref="IConnection"/>.
/// Kept in a separate file so they survive regeneration of the auto-generated ConnectionExtensions.cs.
/// </summary>
public static class KanturuConnectionExtensions
{
    /// <summary>
    /// Sends a <see cref="KanturuStateInfo" /> to this connection.
    /// </summary>
    public static async ValueTask SendKanturuStateInfoAsync(this IConnection? connection, KanturuStateInfo.StateType @state, byte @detailState, bool @canEnter, byte @userCount, uint @remainSeconds)
    {
        if (connection is null)
        {
            return;
        }

        int WritePacket()
        {
            var length = KanturuStateInfoRef.Length;
            var packet = new KanturuStateInfoRef(connection.Output.GetSpan(length)[..length]);
            packet.State = @state;
            packet.DetailState = @detailState;
            packet.CanEnter = @canEnter;
            packet.UserCount = @userCount;
            packet.RemainSeconds = @remainSeconds;

            return packet.Header.Length;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a <see cref="KanturuEnterResult" /> to this connection.
    /// </summary>
    public static async ValueTask SendKanturuEnterResultAsync(this IConnection? connection, KanturuEnterResult.EnterResult @result)
    {
        if (connection is null)
        {
            return;
        }

        int WritePacket()
        {
            var length = KanturuEnterResultRef.Length;
            var packet = new KanturuEnterResultRef(connection.Output.GetSpan(length)[..length]);
            packet.Result = @result;

            return packet.Header.Length;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a <see cref="KanturuStateChange" /> to this connection.
    /// </summary>
    public static async ValueTask SendKanturuStateChangeAsync(this IConnection? connection, KanturuStateChange.StateType @state, byte @detailState)
    {
        if (connection is null)
        {
            return;
        }

        int WritePacket()
        {
            var length = KanturuStateChangeRef.Length;
            var packet = new KanturuStateChangeRef(connection.Output.GetSpan(length)[..length]);
            packet.State = @state;
            packet.DetailState = @detailState;

            return packet.Header.Length;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a <see cref="KanturuBattleResult" /> to this connection.
    /// </summary>
    public static async ValueTask SendKanturuBattleResultAsync(this IConnection? connection, KanturuBattleResult.BattleResult @result)
    {
        if (connection is null)
        {
            return;
        }

        int WritePacket()
        {
            var length = KanturuBattleResultRef.Length;
            var packet = new KanturuBattleResultRef(connection.Output.GetSpan(length)[..length]);
            packet.Result = @result;

            return packet.Header.Length;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a <see cref="KanturuTimeLimit" /> to this connection.
    /// </summary>
    public static async ValueTask SendKanturuTimeLimitAsync(this IConnection? connection, uint @timeLimitMilliseconds)
    {
        if (connection is null)
        {
            return;
        }

        int WritePacket()
        {
            var length = KanturuTimeLimitRef.Length;
            var packet = new KanturuTimeLimitRef(connection.Output.GetSpan(length)[..length]);
            packet.TimeLimitMilliseconds = @timeLimitMilliseconds;

            return packet.Header.Length;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a <see cref="KanturuMayaWideAreaAttack" /> to this connection.
    /// </summary>
    public static async ValueTask SendKanturuMayaWideAreaAttackAsync(this IConnection? connection, KanturuMayaWideAreaAttack.AttackType @type)
    {
        if (connection is null)
        {
            return;
        }

        int WritePacket()
        {
            var length = KanturuMayaWideAreaAttackRef.Length;
            var packet = new KanturuMayaWideAreaAttackRef(connection.Output.GetSpan(length)[..length]);
            packet.Type = @type;

            return packet.Header.Length;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a <see cref="KanturuMonsterUserCount" /> to this connection.
    /// </summary>
    public static async ValueTask SendKanturuMonsterUserCountAsync(this IConnection? connection, byte @monsterCount, byte @userCount)
    {
        if (connection is null)
        {
            return;
        }

        int WritePacket()
        {
            var length = KanturuMonsterUserCountRef.Length;
            var packet = new KanturuMonsterUserCountRef(connection.Output.GetSpan(length)[..length]);
            packet.MonsterCount = @monsterCount;
            packet.UserCount = @userCount;

            return packet.Header.Length;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }
}
