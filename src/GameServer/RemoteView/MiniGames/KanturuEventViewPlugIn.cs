// <copyright file="KanturuEventViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network.Packets.ServerToClient;
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
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuEventViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The remote player this plugin sends packets to.</param>
    public KanturuEventViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowStateInfoAsync(KanturuState state, byte detailState, bool canEnter, int userCount, TimeSpan remainTime)
    {
        await this._player.Connection.SendKanturuStateInfoAsync(
            Convert(state),
            detailState,
            canEnter,
            (byte)Math.Min(255, userCount),
            (uint)remainTime.TotalSeconds)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowEnterResultAsync(bool success)
    {
        await this._player.Connection.SendKanturuEnterResultAsync(
            success ? KanturuEnterResult.EnterResult.Success : KanturuEnterResult.EnterResult.Failed)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowStateChangeAsync(KanturuState state, byte detailState)
    {
        await this._player.Connection.SendKanturuStateChangeAsync(
            ConvertChange(state),
            detailState)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowBattleResultAsync(bool victory)
    {
        await this._player.Connection.SendKanturuBattleResultAsync(
            victory ? KanturuBattleResult.BattleResult.Victory : KanturuBattleResult.BattleResult.Failure)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowTimeLimitAsync(TimeSpan timeLimit)
    {
        await this._player.Connection.SendKanturuTimeLimitAsync(
            (uint)timeLimit.TotalMilliseconds)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowMonsterUserCountAsync(int monsterCount, int userCount)
    {
        await this._player.Connection.SendKanturuMonsterUserCountAsync(
            (byte)Math.Min(255, monsterCount),
            (byte)Math.Min(255, userCount))
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowMayaWideAreaAttackAsync(bool isStorm)
    {
        await this._player.Connection.SendKanturuMayaWideAreaAttackAsync(
            isStorm ? KanturuMayaWideAreaAttack.AttackType.Storm : KanturuMayaWideAreaAttack.AttackType.Rain)
            .ConfigureAwait(false);
    }

    private static KanturuStateInfo.StateType Convert(KanturuState state) => (KanturuStateInfo.StateType)(byte)state;

    private static KanturuStateChange.StateType ConvertChange(KanturuState state) => (KanturuStateChange.StateType)(byte)state;
}
