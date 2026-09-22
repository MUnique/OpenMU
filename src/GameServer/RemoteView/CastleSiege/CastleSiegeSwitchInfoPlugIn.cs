// <copyright file="CastleSiegeSwitchInfoPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;
using SwitchInfo = MUnique.OpenMU.GameLogic.Views.CastleSiege.CastleSiegeSwitchInfo;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeSwitchInfoPlugIn"/>
/// which forwards Crown-switch occupancy to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeSwitchInfoPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeSwitchInfoPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("28A5C92F-D3CF-42D6-B0D6-77C1CDB0913F")]
public class CastleSiegeSwitchInfoPlugIn : ICastleSiegeSwitchInfoPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeSwitchInfoPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeSwitchInfoPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public ValueTask ShowSwitchInfoAsync(SwitchInfo switchInfo)
        => this._player.Connection?.SendCastleSiegeSwitchInfoAsync(
            switchInfo.ObjectId,
            switchInfo.IsOccupied,
            (CastleSiegeJoinSide)switchInfo.JoinSide,
            switchInfo.GuildName,
            switchInfo.CharacterName) ?? default;
}
