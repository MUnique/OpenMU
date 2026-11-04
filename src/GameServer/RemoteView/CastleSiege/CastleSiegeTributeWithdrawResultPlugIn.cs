// <copyright file="CastleSiegeTributeWithdrawResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeTributeWithdrawResultPlugIn"/>
/// which forwards treasury-withdrawal results to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeTributeWithdrawResultPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeTributeWithdrawResultPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("6B84EE67-9FE7-4C69-BC54-FA72BE7E3D6A")]
public class CastleSiegeTributeWithdrawResultPlugIn : ICastleSiegeTributeWithdrawResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeTributeWithdrawResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeTributeWithdrawResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public ValueTask ShowTributeWithdrawResultAsync(CastleSiegeRequestResult result, long amount)
        => this._player.Connection?.SendCastleSiegeTributeWithdrawResponseAsync(
            (byte)result,
            checked((ulong)amount)) ?? default;
}
