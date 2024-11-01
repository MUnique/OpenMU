// <copyright file="DuelFinishedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IDuelFinishedPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(DuelFinishedPlugIn), "The default implementation of the IDuelFinishedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("0DA9F6D1-3DC2-4A75-BA9C-BB77C2A7EB62")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
public class DuelFinishedPlugIn : IDuelFinishedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelFinishedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public DuelFinishedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask DuelFinishedAsync(Player winner, Player loser)
    {
        await this._player.Connection.SendDuelFinishedAsync(winner.Name, loser.Name).ConfigureAwait(false);
    }
}