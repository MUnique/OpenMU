// <copyright file="ShowAllianceRequestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowAllianceRequestPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowAllianceRequestPlugIn), "The default implementation of the IShowAllianceRequestPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("A1B2C3D4-E5F6-7890-1234-567890ABCDEF")]
public class ShowAllianceRequestPlugIn : IShowAllianceRequestPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowAllianceRequestPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowAllianceRequestPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowRequestAsync(string requesterGuildName)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        await connection.SendAllianceJoinRequestAsync(requesterGuildName).ConfigureAwait(false);
    }
}
