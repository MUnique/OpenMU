// <copyright file="CastleSiegeHuntZoneGuardInfoPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeHuntZoneGuardInfoPlugIn"/>
/// which forwards Land of Trials guardsman information to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeHuntZoneGuardInfoPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeHuntZoneGuardInfoPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("66A19818-AE03-4812-B870-1D80AF8C4F1A")]
public class CastleSiegeHuntZoneGuardInfoPlugIn : ICastleSiegeHuntZoneGuardInfoPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeHuntZoneGuardInfoPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeHuntZoneGuardInfoPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public ValueTask ShowHuntZoneGuardInfoAsync(
        CastleSiegeHuntZoneAccessType accessType,
        bool isPublic,
        int currentPrice,
        int maximumPrice,
        int priceStep)
        => this._player.Connection?.SendCastleSiegeHuntingZoneGuardInfoAsync(
            (byte)accessType,
            isPublic,
            checked((uint)currentPrice),
            checked((uint)maximumPrice),
            checked((uint)priceStep)) ?? default;
}
