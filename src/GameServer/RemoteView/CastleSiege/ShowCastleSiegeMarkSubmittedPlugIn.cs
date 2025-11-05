// <copyright file="ShowCastleSiegeMarkSubmittedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCastleSiegeMarkSubmittedPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowCastleSiegeMarkSubmittedPlugIn), "The default implementation of the IShowCastleSiegeMarkSubmittedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("E5F67890-1234-34AB-CDEF-567890123ABC")]
public class ShowCastleSiegeMarkSubmittedPlugIn : IShowCastleSiegeMarkSubmittedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCastleSiegeMarkSubmittedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCastleSiegeMarkSubmittedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowMarkSubmittedAsync(int totalMarksSubmitted)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        await connection.SendCastleSiegeMarkSubmittedAsync((uint)totalMarksSubmitted).ConfigureAwait(false);
    }
}
