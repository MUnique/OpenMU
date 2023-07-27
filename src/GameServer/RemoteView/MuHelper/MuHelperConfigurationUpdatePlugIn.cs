// <copyright file="MuHelperConfigurationUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MuHelper;

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic.Views.MuHelper;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IMuHelperConfigurationUpdatePlugIn"/>
/// which sends the new MU Helper status to the client.
/// </summary>
[PlugIn(nameof(MuHelperConfigurationUpdatePlugIn), "Sends the new MU Helper status to the client.")]
[Guid("E152E151-543C-437E-8BFD-2D92391822F5")]
public class MuHelperConfigurationUpdatePlugIn : IMuHelperConfigurationUpdatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="MuHelperConfigurationUpdatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public MuHelperConfigurationUpdatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateMuHelperConfigurationAsync(Memory<byte> data)
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        await connection.SendMuHelperConfigurationDataAsync(data).ConfigureAwait(false);
    }
}