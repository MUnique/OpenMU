// <copyright file="ObjectGotKilledPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IObjectGotKilledPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ObjectGotKilledPlugIn), "The default implementation of the IObjectGotKilledPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("fbe6666e-4425-4f33-b7c7-fc9b5fa36430")]
public class ObjectGotKilledPlugIn : IObjectGotKilledPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectGotKilledPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ObjectGotKilledPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ObjectGotKilledAsync(IAttackable killed, IAttacker? killer)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        var killedId = killed.GetId(this._player);
        var killerId = killer.GetId(this._player);
        var skillId = killed.LastDeath?.SkillNumber.ToUnsigned() ?? 0;
        var isCombo = killed.LastDeath?.FinalHit.Attributes.HasFlag(DamageAttributes.Combo) ?? false;
        skillId = isCombo ? ShowSkillAnimationPlugIn.ComboSkillId : skillId;
        await connection.SendObjectGotKilledAsync(killedId, skillId, killerId).ConfigureAwait(false);
        if (this._player == killed && killer is Player killerPlayer && this._player.DuelRoom is null)
        {
            await this._player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync($"You got killed by {killerPlayer.Name}", MessageType.BlueNormal)).ConfigureAwait(false);
        }
    }
}