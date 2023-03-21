// <copyright file="ShowSkillAnimationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowSkillAnimationPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowSkillAnimationPlugIn), "The default implementation of the IShowSkillAnimationPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("a25cc420-c848-4a87-81e5-b86c4241af35")]
[MinimumClient(3, 0, ClientLanguage.Invariant)]
public class ShowSkillAnimationPlugIn : IShowSkillAnimationPlugIn
{
    /// <summary>
    /// The combo skill identifier.
    /// </summary>
    internal const ushort ComboSkillId = 59;

    private const ushort NovaStartSkillId = 58;
    private const short ForceSkillId = 60;
    private const short ForceWaveSkillId = 66;

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowSkillAnimationPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowSkillAnimationPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public ValueTask ShowSkillAnimationAsync(IAttacker attacker, IAttackable? target, Skill skill, bool effectApplied)
    {
        return this.ShowSkillAnimationAsync(attacker, target, skill.Number, effectApplied);
    }

    /// <inheritdoc/>
    public async ValueTask ShowSkillAnimationAsync(IAttacker attacker, IAttackable? target, short skillNumber, bool effectApplied)
    {
        if (skillNumber == ForceWaveSkillId)
        {
            skillNumber = ForceSkillId;
        }

        var playerId = attacker.GetId(this._player);
        var targetId = target.GetId(this._player);
        var skillId = NumberConversionExtensions.ToUnsigned(skillNumber);
        await this._player.Connection.SendSkillAnimationAsync(skillId, playerId, targetId).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask ShowComboAnimationAsync(IAttacker attacker, IAttackable? target)
    {
        var playerId = attacker.GetId(this._player);
        var targetId = ((IIdentifiable?)target ?? attacker).GetId(this._player);
        await this._player.Connection.SendSkillAnimationAsync(ComboSkillId, playerId, targetId).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowNovaStartAsync(IAttacker attacker)
    {
        var playerId = attacker.GetId(this._player);
        await this._player.Connection.SendSkillAnimationAsync(NovaStartSkillId, playerId, 0).ConfigureAwait(false);
    }
}