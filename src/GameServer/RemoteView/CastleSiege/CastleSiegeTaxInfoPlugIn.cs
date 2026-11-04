// <copyright file="CastleSiegeTaxInfoPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeTaxInfoPlugIn"/>
/// which forwards the Castle Siege economy state to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeTaxInfoPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeTaxInfoPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("5B79A56E-1D22-4AC6-96CC-BAB269315490")]
public class CastleSiegeTaxInfoPlugIn : ICastleSiegeTaxInfoPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeTaxInfoPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeTaxInfoPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public ValueTask ShowTaxInfoAsync(
        CastleSiegeRequestResult result,
        byte chaosTax,
        byte storeTax,
        long tributeMoney)
        => this._player.Connection?.SendCastleSiegeTaxInfoResponseAsync(
            (byte)result,
            chaosTax,
            storeTax,
            checked((ulong)tributeMoney)) ?? default;
}
