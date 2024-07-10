using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameServer.RemoteView.World;

namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IDuelEndedPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(DuelEndedPlugIn), "The default implementation of the IDuelEndedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("4FBC822B-F35B-4CB1-AFE6-180243171074")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
public class DuelEndedPlugIn : IDuelEndedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelEndedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public DuelEndedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask DuelEndedAsync()
    {
        await this._player.Connection.SendDuelEndAsync(0, string.Empty).ConfigureAwait(false);
        await this._player.Connection.SendMagicEffectStatusAsync(false, this._player.GetId(this._player), EffectNumbers.DuelSpectatorHealthBar).ConfigureAwait(false);
    }
}