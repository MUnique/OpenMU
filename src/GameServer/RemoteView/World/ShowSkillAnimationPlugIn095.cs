// <copyright file="ShowSkillAnimationPlugIn095.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowSkillAnimationPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowSkillAnimationPlugIn095), "The default implementation of the IShowSkillAnimationPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("105E727A-A8B5-4050-B6FE-1CC5F5DDC9E4")]
[MinimumClient(0, 95, ClientLanguage.Invariant)]
public class ShowSkillAnimationPlugIn095 : IShowSkillAnimationPlugIn
{
    private const byte NovaStartSkillId = 58;
    private const byte ComboSkillId = 59;
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowSkillAnimationPlugIn095"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowSkillAnimationPlugIn095(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public ValueTask ShowSkillAnimationAsync(IAttacker attacker, IAttackable? target, Skill skill, bool effectApplied)
    {
        return this.ShowSkillAnimationAsync(attacker, target, skill.Number, effectApplied);
    }

    /// <inheritdoc/>
    public async ValueTask ShowSkillAnimationAsync(IAttacker attacker, IAttackable? target, short skillNumber, bool effectApplied)
    {
        var playerId = attacker.GetId(this._player);
        var targetId = target.GetId(this._player);
        await this._player.Connection.SendSkillAnimation095Async((byte)skillNumber, playerId, targetId, effectApplied).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask ShowComboAnimationAsync(IAttacker attacker, IAttackable? target)
    {
        var playerId = attacker.GetId(this._player);
        var targetId = ((IIdentifiable?)target ?? attacker).GetId(this._player);
        await this._player.Connection.SendSkillAnimation095Async(ComboSkillId, playerId, targetId, false).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowNovaStartAsync(IAttacker attacker)
    {
        var playerId = attacker.GetId(this._player);
        await this._player.Connection.SendSkillAnimation095Async(NovaStartSkillId, playerId, 0, false).ConfigureAwait(false);
    }
}