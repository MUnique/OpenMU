// <copyright file="CastleSiegeJoinSidePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;
using JoinSide = MUnique.OpenMU.DataModel.Configuration.CastleSiegeJoinSide;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeJoinSidePlugIn"/>
/// which forwards the assigned side to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeJoinSidePlugIn_Name), Description = nameof(PlugInResources.CastleSiegeJoinSidePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("175ECFB8-476A-4371-A416-A1B1C5E19402")]
public class CastleSiegeJoinSidePlugIn : ICastleSiegeJoinSidePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeJoinSidePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeJoinSidePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public ValueTask ShowJoinSideAsync(JoinSide side)
        => this._player.Connection?.SendCastleSiegeJoinSideNotificationAsync((CastleSiegeJoinSide)side) ?? default;
}
