// <copyright file="ShowAllianceResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowAllianceResponsePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowAllianceResponsePlugIn), "The default implementation of the IShowAllianceResponsePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("B2C3D4E5-F678-9012-3456-7890ABCDEF01")]
public class ShowAllianceResponsePlugIn : IShowAllianceResponsePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowAllianceResponsePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowAllianceResponsePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowResponseAsync(AllianceResponse response, string targetGuildName)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        await connection.SendAllianceJoinResponseAsync((AllianceJoinResponse.AllianceJoinResult)response).ConfigureAwait(false);
    }
}
