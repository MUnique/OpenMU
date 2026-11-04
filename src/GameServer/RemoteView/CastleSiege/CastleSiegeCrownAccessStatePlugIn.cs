// <copyright file="CastleSiegeCrownAccessStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;
using CrownAccessState = MUnique.OpenMU.GameLogic.Views.CastleSiege.CastleSiegeCrownAccessState;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeCrownAccessStatePlugIn"/>
/// which forwards Crown capture progress to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeCrownAccessStatePlugIn_Name), Description = nameof(PlugInResources.CastleSiegeCrownAccessStatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("9F13B1D3-70F6-47A1-AE2A-A3F1BCC35A6A")]
public class CastleSiegeCrownAccessStatePlugIn : ICastleSiegeCrownAccessStatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeCrownAccessStatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeCrownAccessStatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public ValueTask ShowCrownAccessStateAsync(
        CrownAccessState state,
        TimeSpan accumulatedTime)
        => this._player.Connection?.SendCastleSiegeCrownAccessStateAsync(
            (CastleSiegeCrownAccessStateType)state,
            checked((uint)accumulatedTime.TotalMilliseconds)) ?? default;
}
