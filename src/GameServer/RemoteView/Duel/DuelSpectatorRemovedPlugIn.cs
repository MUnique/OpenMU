namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IDuelSpectatorRemovedPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(DuelSpectatorRemovedPlugIn), "The default implementation of the IDuelSpectatorRemovedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("89DFA45D-FED2-4FAA-BDB0-683EB337629B")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
public class DuelSpectatorRemovedPlugIn : IDuelSpectatorRemovedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelSpectatorRemovedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public DuelSpectatorRemovedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask SpectatorRemovedAsync(Player spectator)
    {
        await this._player.Connection.SendDuelSpectatorRemovedAsync(spectator.Name).ConfigureAwait(false);
    }
}