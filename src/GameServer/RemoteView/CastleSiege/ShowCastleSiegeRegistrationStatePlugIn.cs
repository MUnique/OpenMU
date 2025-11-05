// <copyright file="ShowCastleSiegeRegistrationStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCastleSiegeRegistrationStatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowCastleSiegeRegistrationStatePlugIn), "The default implementation of the IShowCastleSiegeRegistrationStatePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("A7089012-3456-56BC-CDEF-789012345BCD")]
public class ShowCastleSiegeRegistrationStatePlugIn : IShowCastleSiegeRegistrationStatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCastleSiegeRegistrationStatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCastleSiegeRegistrationStatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowRegistrationStateAsync(bool isRegistered, int totalMarksSubmitted)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        await connection.SendCastleSiegeRegistrationStateAsync(isRegistered, (uint)totalMarksSubmitted).ConfigureAwait(false);
    }
}
