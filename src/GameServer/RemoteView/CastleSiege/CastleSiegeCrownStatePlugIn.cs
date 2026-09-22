// <copyright file="CastleSiegeCrownStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeCrownStatePlugIn"/>
/// which forwards the Crown lock state to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeCrownStatePlugIn_Name), Description = nameof(PlugInResources.CastleSiegeCrownStatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("50531428-FE54-458D-85D0-143DF1106D38")]
public class CastleSiegeCrownStatePlugIn : ICastleSiegeCrownStatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeCrownStatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeCrownStatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public ValueTask ShowCrownStateAsync(bool isAvailable)
        => this._player.Connection?.SendCastleSiegeCrownStateUpdateAsync(
            isAvailable ? CastleSiegeCrownState.Accessible : CastleSiegeCrownState.Protected) ?? default;
}
