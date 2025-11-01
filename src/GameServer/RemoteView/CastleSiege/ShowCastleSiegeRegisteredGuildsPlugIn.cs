// <copyright file="ShowCastleSiegeRegisteredGuildsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCastleSiegeRegisteredGuildsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowCastleSiegeRegisteredGuildsPlugIn), "The default implementation of the IShowCastleSiegeRegisteredGuildsPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("B8C9D0E1-4567-67BC-CDEF-890123456CDE")]
public class ShowCastleSiegeRegisteredGuildsPlugIn : IShowCastleSiegeRegisteredGuildsPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCastleSiegeRegisteredGuildsPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCastleSiegeRegisteredGuildsPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowRegisteredGuildsAsync(IEnumerable<(Guild Guild, int MarksSubmitted)> registeredGuilds)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        // TODO: Implement proper packet sending when server-to-client castle siege guild list packets are defined
        var guildList = string.Join(", ", registeredGuilds.Select(g => $"{g.Guild.Name} ({g.MarksSubmitted} marks)"));
        var message = registeredGuilds.Any()
            ? $"Registered guilds: {guildList}"
            : "No guilds registered for castle siege";

        await connection.SendServerMessageAsync(
            ServerMessage.MessageType.GoldenCenter,
            message).ConfigureAwait(false);
    }
}
