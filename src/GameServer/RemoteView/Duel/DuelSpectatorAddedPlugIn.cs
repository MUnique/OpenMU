namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IDuelSpectatorAddedPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(DuelSpectatorAddedPlugIn), "The default implementation of the IDuelSpectatorAddedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("BB91E97F-E1F9-4152-9AA1-573D917ACD35")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
public class DuelSpectatorAddedPlugIn : IDuelSpectatorAddedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelSpectatorAddedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public DuelSpectatorAddedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask SpectatorAddedAsync(Player spectator)
    {
        await this._player.Connection.SendDuelSpectatorAddedAsync(spectator.Name).ConfigureAwait(false);
    }
}