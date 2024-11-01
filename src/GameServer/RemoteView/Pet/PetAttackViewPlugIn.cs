// <copyright file="PetAttackViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Pet;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Pet;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the chat view which is forwarding everything to the game client which specific data packets.
/// </summary>
[PlugIn(nameof(PetAttackViewPlugIn), "View plugin to show pet attacks.")]
[Guid("1796C164-ABB8-4AD5-89E2-EA905D20036D")]
internal class PetAttackViewPlugIn : IPetAttackViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="PetAttackViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public PetAttackViewPlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    public async ValueTask ShowPetAttackAnimationAsync(IIdentifiable owner, Item pet, IAttackable target, PetAttackType attackType)
    {
        await this._player.Connection.SendPetAttackAsync(
            Convert(attackType),
            owner.GetId(this._player),
            target.GetId(this._player)).ConfigureAwait(false);
    }

    private static PetAttack.PetSkillType Convert(PetAttackType attackType)
    {
        return attackType switch
        {
            PetAttackType.SingleTarget => PetAttack.PetSkillType.SingleTarget,
            PetAttackType.RangeAttack => PetAttack.PetSkillType.Range,
            _ => throw new ArgumentException($"Unknown pet attack type: {attackType}"),
        };
    }
}