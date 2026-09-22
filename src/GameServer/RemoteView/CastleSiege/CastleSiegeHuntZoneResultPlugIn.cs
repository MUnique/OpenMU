// <copyright file="CastleSiegeHuntZoneResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeHuntZoneResultPlugIn"/>
/// which forwards Land of Trials request results to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeHuntZoneResultPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeHuntZoneResultPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("ACD57B49-E816-45D9-8C70-39FB4EC0F2AF")]
public class CastleSiegeHuntZoneResultPlugIn : ICastleSiegeHuntZoneResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeHuntZoneResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeHuntZoneResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public ValueTask ShowEntranceSettingResultAsync(CastleSiegeRequestResult result, bool isPublic)
        => this._player.Connection?.SendCastleSiegeHuntingZoneEntranceSettingResponseAsync(
            (byte)result,
            isPublic) ?? default;

    /// <inheritdoc />
    public ValueTask ShowEnterResultAsync(bool success)
        => this._player.Connection?.SendCastleSiegeHuntingZoneEnterResponseAsync(success ? (byte)1 : (byte)0) ?? default;
}
