// <copyright file="ObjectGotKilledPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IObjectGotKilledPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ObjectGotKilledPlugIn", "The default implementation of the IObjectGotKilledPlugIn which is forwarding everything to the game client with specific data packets.")]
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
    public void ObjectGotKilled(IAttackable killed, IAttacker killer, Skill? skill = null)
    {
        var killedId = killed.GetId(this._player);
        var killerId = killer.GetId(this._player);
        this._player.Connection?.SendObjectGotKilled(killedId, skill?.Number.ToUnsigned() ?? 0, killerId);
        if (this._player == killed && killer is Player killerPlayer)
        {
            this._player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage($"You got killed by {killerPlayer.Name}", MessageType.BlueNormal);
        }
    }
}