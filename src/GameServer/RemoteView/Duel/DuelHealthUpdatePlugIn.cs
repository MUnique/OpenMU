namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IDuelHealthUpdatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(DuelHealthUpdatePlugIn), "The default implementation of the IDuelHealthUpdatePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("40CE7F73-F9DF-4F4E-BBCE-04938604A72C")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
public class DuelHealthUpdatePlugIn : IDuelHealthUpdatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelHealthUpdatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public DuelHealthUpdatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateHealthAsync(DuelRoom duelRoom)
    {
        var player1 = this._player == duelRoom.Opponent ? duelRoom.Opponent : duelRoom.Requester;
        var player2 = this._player == duelRoom.Opponent ? duelRoom.Requester : duelRoom.Opponent;

        var player1Health = player1.Attributes![Stats.CurrentHealth] / (player1.Attributes[Stats.MaximumHealth] / 100f);
        var player1Shield = player1.Attributes[Stats.CurrentShield] / (player1.Attributes[Stats.MaximumShield] / 100f);
        var player2Health = player2.Attributes![Stats.CurrentHealth] / (player1.Attributes[Stats.MaximumHealth] / 100f);
        var player2Shield = player2.Attributes[Stats.CurrentShield] / (player1.Attributes[Stats.MaximumShield] / 100f);

        await this._player.Connection.SendDuelHealthUpdateAsync(
                player1.GetId(this._player),
                player2.GetId(this._player),
                (byte)player1Health,
                (byte)player2Health,
                (byte)player1Shield,
                (byte)player2Shield)
            .ConfigureAwait(false);
    }
}