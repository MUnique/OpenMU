// <copyright file="ShowRotationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowRotationPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowRotationPlugIn), "The default implementation of the IShowRotationPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("A8F3C1E7-2B94-4D6F-8A3C-9E5F1D2B4C8A")]
public class ShowRotationPlugIn : IShowRotationPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowRotationPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowRotationPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowRotationAsync(IRotatable rotatedObject)
    {
        // Don't send rotation update if the rotated object is the player themselves
        // (they already know their own rotation from their client input)
        if (rotatedObject == this._player)
        {
            return;
        }

        await this._player.Connection.SendUpdateRotationAsync(rotatedObject.Rotation.ToPacketByte()).ConfigureAwait(false);
    }
}
