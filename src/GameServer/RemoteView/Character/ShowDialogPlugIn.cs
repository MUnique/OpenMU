// <copyright file="ShowDialogPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowDialogPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ShowDialogPlugIn", "The default implementation of the IShowDialogPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("DCAB5737-2B44-408F-A14D-C0FD3B5F6516")]
public class ShowDialogPlugIn : IShowDialogPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowDialogPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowDialogPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public ValueTask ShowDialogAsync(byte categoryNumber, byte dialogNumber)
    {
        return this._player.Connection.SendServerCommandAsync(categoryNumber, dialogNumber, 0);
    }
}