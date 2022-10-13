// <copyright file="PetBehaviourChangedViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Pet;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Pet;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the chat view which is forwarding everything to the game client which specific data packets.
/// </summary>
[PlugIn(nameof(PetBehaviourChangedViewPlugIn), "View plugin to signal a changed pet behaviour.")]
[Guid("F0B5BAD4-B97C-49F1-84E0-25EDC796B0E4")]
internal class PetBehaviourChangedViewPlugIn : IPetBehaviourChangedViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="PetBehaviourChangedViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public PetBehaviourChangedViewPlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    public async ValueTask PetBehaviourChanged(Item pet, PetBehaviour behaviour, IAttackable? target)
    {
        await this._player.Connection.SendPetModeAsync(Convert(behaviour), target?.GetId(this._player) ?? 0xFFFF).ConfigureAwait(false);
    }

    private static PetCommandMode Convert(PetBehaviour attackType)
    {
        switch (attackType)
        {
            case PetBehaviour.AttackRandom: return PetCommandMode.AttackRandom;
            case PetBehaviour.AttackWithOwner: return PetCommandMode.AttackWithOwner;
            case PetBehaviour.AttackTarget: return PetCommandMode.AttackTarget;
            case PetBehaviour.Idle: return PetCommandMode.Normal;
            default: throw new ArgumentException($"Unknown pet attack type: {attackType}");
        }
    }
}