// <copyright file="ShowAllianceListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowAllianceListPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowAllianceListPlugIn), "The default implementation of the IShowAllianceListPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("C3D4E5F6-7890-1234-5678-90ABCDEF0123")]
public class ShowAllianceListPlugIn : IShowAllianceListPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowAllianceListPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowAllianceListPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowAllianceListAsync(IEnumerable<Guild> allianceGuilds)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        // TODO: Implement proper packet sending when server-to-client alliance packets are defined
        var guildNames = string.Join(", ", allianceGuilds.Select(g => g.Name));
        await connection.SendServerMessageAsync(
            ServerMessage.MessageType.GoldenCenter,
            $"Alliance members: {guildNames}").ConfigureAwait(false);
    }
}
