// <copyright file="KanturuEventViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Buffers.Binary;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.GameServer.RemoteView;

/// <summary>
/// Sends Kanturu-specific state/result/HUD packets (0xD1 group) to the client.
/// </summary>
/// <remarks>
/// Packet map (all C1 headers, subcode-bearing):
/// <list type="table">
///   <item><term>0xD1 0x00</term><description>State info — opens the gateway NPC dialog (INTERFACE_KANTURU2ND_ENTERNPC).</description></item>
///   <item><term>0xD1 0x01</term><description>Enter result — stops NPC animation, shows error popup on failure.</description></item>
///   <item><term>0xD1 0x03</term><description>State change — controls HUD, music, and terrain reload.</description></item>
///   <item><term>0xD1 0x04</term><description>Battle result — displays Success_kantru.tga or Failure_kantru.tga.</description></item>
///   <item><term>0xD1 0x05</term><description>Time limit — countdown timer for the in-map HUD (milliseconds).</description></item>
///   <item><term>0xD1 0x06</term><description>Maya wide-area attack — triggers the visual MayaAction sequence on the Maya body object.</description></item>
///   <item><term>0xD1 0x07</term><description>Monster/user count — numbers displayed in the HUD.</description></item>
/// </list>
/// </remarks>
[PlugIn]
[Display(Name = "Kanturu Event View Plugin", Description = "Sends Kanturu event state/result/HUD packets (0xD1 group) to the client.")]
[Guid("A3F1C8D2-5E94-4B7A-8C31-D6F2E0A49B15")]
public sealed class KanturuEventViewPlugIn : IKanturuEventViewPlugIn
{
    // Packet byte-lengths (C1 + Length + Code + SubCode + data):
    private const int StateInfoLength = 12;          // + btState + btDetailState + btEnter + btUserCount + int iRemainTime (4 bytes LE)
    private const int EnterResultLength = 5;         // + btResult
    private const int StateChangeLength = 6;         // + btState + btDetailState
    private const int BattleResultLength = 5;        // + btResult
    private const int TimeLimitLength = 8;           // + int btTimeLimit (4 bytes LE)
    private const int MonsterUserCountLength = 6;    // + bMonsterCount + btUserCount
    private const int WideAreaAttackLength = 7;      // + btObjClassH + btObjClassL + btType

    private const byte C1 = 0xC1;
    private const byte GroupCode = 0xD1;

    private const byte SubCodeStateInfo = 0x00;
    private const byte SubCodeEnterResult = 0x01;
    private const byte SubCodeStateChange = 0x03;
    private const byte SubCodeResult = 0x04;
    private const byte SubCodeTimeLimit = 0x05;
    private const byte SubCodeWideAreaAttack = 0x06;
    private const byte SubCodeMonsterUserCount = 0x07;

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuEventViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The remote player this plugin sends packets to.</param>
    public KanturuEventViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask SendStateInfoAsync(byte state, byte detailState, byte enter, byte userCount, int remainTimeSec)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int Write()
        {
            var span = connection.Output.GetSpan(StateInfoLength)[..StateInfoLength];
            span[0] = C1;
            span[1] = StateInfoLength;
            span[2] = GroupCode;
            span[3] = SubCodeStateInfo;
            span[4] = state;
            span[5] = detailState;
            span[6] = enter;
            span[7] = userCount;
            // The client struct field iRemainTime is a plain C int — write as little-endian.
            BinaryPrimitives.WriteInt32LittleEndian(span[8..], remainTimeSec);
            return StateInfoLength;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SendEnterResultAsync(byte result)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int Write()
        {
            var span = connection.Output.GetSpan(EnterResultLength)[..EnterResultLength];
            span[0] = C1;
            span[1] = EnterResultLength;
            span[2] = GroupCode;
            span[3] = SubCodeEnterResult;
            span[4] = result;
            return EnterResultLength;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SendStateChangeAsync(byte state, byte detailState)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int Write()
        {
            var span = connection.Output.GetSpan(StateChangeLength)[..StateChangeLength];
            span[0] = C1;
            span[1] = StateChangeLength;
            span[2] = GroupCode;
            span[3] = SubCodeStateChange;
            span[4] = state;
            span[5] = detailState;
            return StateChangeLength;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SendBattleResultAsync(byte result)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int Write()
        {
            var span = connection.Output.GetSpan(BattleResultLength)[..BattleResultLength];
            span[0] = C1;
            span[1] = BattleResultLength;
            span[2] = GroupCode;
            span[3] = SubCodeResult;
            span[4] = result;
            return BattleResultLength;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SendTimeLimitAsync(int timeLimitMs)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int Write()
        {
            var span = connection.Output.GetSpan(TimeLimitLength)[..TimeLimitLength];
            span[0] = C1;
            span[1] = TimeLimitLength;
            span[2] = GroupCode;
            span[3] = SubCodeTimeLimit;
            // The client struct uses a plain C int — write as little-endian.
            BinaryPrimitives.WriteInt32LittleEndian(span[4..], timeLimitMs);
            return TimeLimitLength;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SendMonsterUserCountAsync(byte monsterCount, byte userCount)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int Write()
        {
            var span = connection.Output.GetSpan(MonsterUserCountLength)[..MonsterUserCountLength];
            span[0] = C1;
            span[1] = MonsterUserCountLength;
            span[2] = GroupCode;
            span[3] = SubCodeMonsterUserCount;
            span[4] = monsterCount;
            span[5] = userCount;
            return MonsterUserCountLength;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SendMayaWideAreaAttackAsync(byte attackType)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int Write()
        {
            var span = connection.Output.GetSpan(WideAreaAttackLength)[..WideAreaAttackLength];
            span[0] = C1;
            span[1] = WideAreaAttackLength;
            span[2] = GroupCode;
            span[3] = SubCodeWideAreaAttack;
            // btObjClassH / btObjClassL — the client ignores these bytes and only reads btType,
            // so we send zeros. (Confirmed in WSclient.cpp: RecevieKanturu3rdMayaSKill reads
            // only pData->btType and passes it to MayaSceneMayaAction.)
            span[4] = 0x00; // btObjClassH
            span[5] = 0x00; // btObjClassL
            span[6] = attackType;
            return WideAreaAttackLength;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
