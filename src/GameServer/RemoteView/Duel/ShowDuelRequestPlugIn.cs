namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowDuelRequestPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowDuelRequestPlugIn), "The default implementation of the IShowDuelRequestPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("708847EE-B64F-42A3-BDAC-C4FD2417B6A5")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
public class ShowDuelRequestPlugIn : IShowDuelRequestPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowDuelRequestPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowDuelRequestPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowDuelRequestAsync(Player requester)
    {
        await this._player.Connection.SendDuelStartRequestAsync(requester.GetId(this._player), requester.Name).ConfigureAwait(false);
    }
}