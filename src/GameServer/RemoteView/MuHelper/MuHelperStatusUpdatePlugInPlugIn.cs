// <copyright file="MuHelperStatusUpdatePlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MuHelper;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Views.MuHelper;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="MuHelperStatusUpdatePlugInPlugIn"/> which response with new mu bot status.
/// </summary>
[PlugIn(nameof(MuHelperStatusUpdatePlugInPlugIn), "Sends the MU Helper status update to the client.")]
[Guid("6F2E1E5F-D130-496A-B2B0-5D01BD001366")]
public class MuHelperStatusUpdatePlugInPlugIn : IMuHelperStatusUpdatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="MuHelperStatusUpdatePlugInPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public MuHelperStatusUpdatePlugInPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateStatusAsync(MuHelperStatus status, uint money = 0)
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        await connection.SendMuHelperStatusUpdateAsync(money > 0, money, status == MuHelperStatus.Disabled).ConfigureAwait(false);
    }
}