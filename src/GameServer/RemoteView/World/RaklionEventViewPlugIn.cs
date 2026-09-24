// <copyright file="RaklionEventViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Raklion;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;
using RaklionState = MUnique.OpenMU.GameLogic.Raklion.RaklionState;

/// <summary>
/// The default implementation of the <see cref="IRaklionEventViewPlugIn"/> which
/// sends the raklion event packets (0xD1 0x10 - 0x13, 0x69) to the client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.RaklionEventViewPlugIn_Name), Description = nameof(PlugInResources.RaklionEventViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("3E9B5C71-2A84-4D0F-B6E1-8C47F2A9D035")]
public sealed class RaklionEventViewPlugIn : IRaklionEventViewPlugIn
{
    private const byte DetailState = 11;

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaklionEventViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public RaklionEventViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowStateInfoAsync(RaklionState state, SelupanState selupanState, bool canEnter, TimeSpan remainingTime)
    {
        // The states of the packets have the same values as the state of the game logic.
        var remainingSeconds = (uint)Math.Max(0, remainingTime.TotalSeconds);
        await this._player.Connection.SendRaklionStateInfoAsync((RaklionStateInfo.RaklionState)(byte)state, (byte)selupanState, canEnter, remainingSeconds).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowCurrentStateAsync(RaklionState state)
    {
        await this._player.Connection.SendRaklionCurrentStateAsync((RaklionCurrentState.RaklionState)(byte)state, 0).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowStateChangeAsync(RaklionState state)
    {
        await this._player.Connection.SendRaklionStateChangeAsync((RaklionStateChange.RaklionState)(byte)state, 0).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowSelupanStateAsync(SelupanState selupanState)
    {
        await this._player.Connection.SendRaklionStateChangeAsync((RaklionStateChange.RaklionState)DetailState, (byte)selupanState).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowBattleResultAsync(bool success)
    {
        var result = success ? RaklionBattleResult.BattleResult.Success : RaklionBattleResult.BattleResult.Failure;
        await this._player.Connection.SendRaklionBattleResultAsync(result).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowSelupanSkillAsync(IAttacker selupan, IAttackable? target, SelupanSkill skill)
    {
        var selupanId = selupan.GetId(this._player);

        // The client searches the target by the id as it is, so the flag for a successful skill can't be set.
        var targetId = target is null ? selupanId : target.GetId(this._player);
        await this._player.Connection.SendMonsterSkillAnimationAsync((ushort)skill, selupanId, targetId).ConfigureAwait(false);
    }
}
