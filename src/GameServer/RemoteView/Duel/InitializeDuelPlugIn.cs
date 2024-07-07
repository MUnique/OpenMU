namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IInitializeDuelPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(InitializeDuelPlugIn), "The default implementation of the IInitializeDuelPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("41ECFA38-3EAE-4408-B7AC-82F26D8DCCD7")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
public class InitializeDuelPlugIn : IInitializeDuelPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializeDuelPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public InitializeDuelPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask InitializeDuelAsync(DuelContext duelContext)
    {
        await this._player.Connection.SendDuelInitAsync(
                0,
                duelContext.Requester.Name,
                duelContext.Opponent.Name,
                duelContext.Requester.GetId(this._player),
                duelContext.Opponent.GetId(this._player))
            .ConfigureAwait(false);
        await this._player.Connection.SendDuelHealthBarInitAsync().ConfigureAwait(false);
    }
}