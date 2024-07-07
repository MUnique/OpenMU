namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowDuelScoreUpdatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowDuelScoreUpdatePlugIn), "The default implementation of the IShowDuelScoreUpdatePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("328E366D-B801-4780-B65D-B250C388E6B0")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
public class ShowDuelScoreUpdatePlugIn : IShowDuelScoreUpdatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowDuelScoreUpdatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowDuelScoreUpdatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateScoreAsync(Player player1, byte player1Score, Player player2, byte player2Score)
    {
        await this._player.Connection.SendDuelScoreAsync(player1.GetId(this._player), player2.GetId(this._player), player1Score, player2Score).ConfigureAwait(false);
    }
}