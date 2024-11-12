// <copyright file="MuHelperStatusUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MuHelper;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.MuHelper;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="MuHelperStatusUpdatePlugIn"/> which response with new mu bot status.
/// </summary>
[PlugIn(nameof(MuHelperStatusUpdatePlugIn), "Sends the MU Helper status update to the client.")]
[Guid("6F2E1E5F-D130-496A-B2B0-5D01BD001366")]
public class MuHelperStatusUpdatePlugIn : IMuHelperStatusUpdatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="MuHelperStatusUpdatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public MuHelperStatusUpdatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask StartAsync()
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        await connection.SendMuHelperStatusUpdateAsync(false, 0, false).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask StopAsync()
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        await connection.SendMuHelperStatusUpdateAsync(false, 0, true).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ConsumeMoneyAsync(uint money)
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        // Setting the pauseStatus to false has no further effect on the client side, when consumeMoney is true.
        await connection.SendMuHelperStatusUpdateAsync(true, money, false).ConfigureAwait(false);
    }
}