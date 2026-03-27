// <copyright file="KanturuEventViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Sends Kanturu-specific state, result, and HUD packets (0xD1 group) to the client.
/// Packets are defined in <c>ServerToClientPackets.xml</c> and this plugin uses the
/// auto-generated extension methods from <c>ConnectionExtensions</c>.
/// </summary>
[PlugIn]
[Display(Name = "Kanturu Event View Plugin", Description = "Sends Kanturu event packets (0xD1 group) to the client.")]
[Guid("A3F1C8D2-5E94-4B7A-8C31-D6F2E0A49B15")]
public sealed class KanturuEventViewPlugIn : IKanturuEventViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuEventViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The remote player this plugin sends packets to.</param>
    public KanturuEventViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowStateInfoAsync(KanturuState state, byte detailState, bool canEnter, int userCount, TimeSpan remainingTime)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        await connection.SendKanturuStateInfoAsync(
            ConvertState(state),
            detailState,
            canEnter,
            (byte)Math.Min(userCount, byte.MaxValue),
            (int)remainingTime.TotalSeconds).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowEnterResultAsync(KanturuEnterResult result)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        await connection.SendKanturuEnterResultAsync(ConvertEnterResult(result)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowMayaBattleStateAsync(KanturuMayaDetailState detailState)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        await connection.SendKanturuStateChangeAsync(
            KanturuStateChange.StateType.MayaBattle,
            ConvertMayaDetail(detailState)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowNightmareStateAsync(KanturuNightmareDetailState detailState)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        await connection.SendKanturuStateChangeAsync(
            KanturuStateChange.StateType.NightmareBattle,
            ConvertNightmareDetail(detailState)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowTowerStateAsync(KanturuTowerDetailState detailState)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        await connection.SendKanturuStateChangeAsync(
            KanturuStateChange.StateType.Tower,
            ConvertTowerDetail(detailState)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowBattleResultAsync(KanturuBattleResult result)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        await connection.SendKanturuBattleResultAsync(ConvertBattleResult(result)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowTimeLimitAsync(TimeSpan timeLimit)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        await connection.SendKanturuTimeLimitAsync((int)timeLimit.TotalMilliseconds).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowMonsterUserCountAsync(int monsterCount, int userCount)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        await connection.SendKanturuMonsterUserCountAsync(
            (byte)Math.Min(monsterCount, byte.MaxValue),
            (byte)Math.Min(userCount, byte.MaxValue)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowMayaWideAreaAttackAsync(KanturuMayaAttackType attackType)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        // ObjClassH and ObjClassL are ignored by the client (confirmed in WSclient.cpp:
        // RecevieKanturu3rdMayaSKill reads only btType and passes it to MayaSceneMayaAction).
        await connection.SendKanturuMayaWideAreaAttackAsync(0x00, 0x00, ConvertMayaAttackType(attackType)).ConfigureAwait(false);
    }

    private static KanturuStateInfo.StateType ConvertState(KanturuState state) => state switch
    {
        KanturuState.None => KanturuStateInfo.StateType.None,
        KanturuState.Standby => KanturuStateInfo.StateType.Standby,
        KanturuState.MayaBattle => KanturuStateInfo.StateType.MayaBattle,
        KanturuState.NightmareBattle => KanturuStateInfo.StateType.NightmareBattle,
        KanturuState.Tower => KanturuStateInfo.StateType.Tower,
        KanturuState.End => KanturuStateInfo.StateType.End,
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
    };

    private static KanturuEnterResult.EnterResult ConvertEnterResult(KanturuEnterResult result) => result switch
    {
        KanturuEnterResult.Failed => KanturuEnterResult.EnterResult.Failed,
        KanturuEnterResult.Success => KanturuEnterResult.EnterResult.Success,
        _ => throw new ArgumentOutOfRangeException(nameof(result), result, null),
    };

    private static KanturuBattleResult.BattleResult ConvertBattleResult(KanturuBattleResult result) => result switch
    {
        KanturuBattleResult.Failure => KanturuBattleResult.BattleResult.Failure,
        KanturuBattleResult.Victory => KanturuBattleResult.BattleResult.Victory,
        _ => throw new ArgumentOutOfRangeException(nameof(result), result, null),
    };

    private static byte ConvertMayaDetail(KanturuMayaDetailState detail) => detail switch
    {
        KanturuMayaDetailState.None => 0,
        KanturuMayaDetailState.Notify => 2,
        KanturuMayaDetailState.Monster1 => 3,
        KanturuMayaDetailState.MayaLeft => 4,
        KanturuMayaDetailState.Monster2 => 8,
        KanturuMayaDetailState.MayaRight => 9,
        KanturuMayaDetailState.Monster3 => 13,
        KanturuMayaDetailState.BothHands => 14,
        KanturuMayaDetailState.EndCycleMaya3 => 16,
        _ => throw new ArgumentOutOfRangeException(nameof(detail), detail, null),
    };

    private static byte ConvertNightmareDetail(KanturuNightmareDetailState detail) => detail switch
    {
        KanturuNightmareDetailState.None => 0,
        KanturuNightmareDetailState.Idle => 1,
        KanturuNightmareDetailState.Intro => 2,
        KanturuNightmareDetailState.Battle => 3,
        KanturuNightmareDetailState.End => 4,
        _ => throw new ArgumentOutOfRangeException(nameof(detail), detail, null),
    };

    private static byte ConvertTowerDetail(KanturuTowerDetailState detail) => detail switch
    {
        KanturuTowerDetailState.None => 0,
        KanturuTowerDetailState.Revitalization => 1,
        KanturuTowerDetailState.Notify => 2,
        KanturuTowerDetailState.Close => 3,
        _ => throw new ArgumentOutOfRangeException(nameof(detail), detail, null),
    };

    private static KanturuMayaWideAreaAttack.AttackType ConvertMayaAttackType(KanturuMayaAttackType attackType) => attackType switch
    {
        KanturuMayaAttackType.Storm => KanturuMayaWideAreaAttack.AttackType.Storm,
        KanturuMayaAttackType.Rain => KanturuMayaWideAreaAttack.AttackType.Rain,
        _ => throw new ArgumentOutOfRangeException(nameof(attackType), attackType, null),
    };
}
