// <copyright file="ShowCastleSiegeStatusPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCastleSiegeStatusPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowCastleSiegeStatusPlugIn), "The default implementation of the IShowCastleSiegeStatusPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("F6F78901-2345-45AB-CDEF-678901234ABC")]
public class ShowCastleSiegeStatusPlugIn : IShowCastleSiegeStatusPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCastleSiegeStatusPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCastleSiegeStatusPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowStatusAsync(string ownerGuildName, string siegeStatus)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        // Parse the siege status string to enum value
        var stateValue = Enum.TryParse<CastleSiegeState>(siegeStatus, out var state) ? (byte)state : (byte)0;

        await connection.SendCastleSiegeStatusAsync(ownerGuildName, stateValue).ConfigureAwait(false);
    }
}
