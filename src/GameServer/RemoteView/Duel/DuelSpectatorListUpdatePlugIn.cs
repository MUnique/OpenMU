namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IDuelSpectatorListUpdatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(DuelSpectatorListUpdatePlugIn), "The default implementation of the IDuelSpectatorListUpdatePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("1A8EC472-6924-4150-9B4C-4352AFF03AC0")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
public class DuelSpectatorListUpdatePlugIn : IDuelSpectatorListUpdatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelSpectatorListUpdatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public DuelSpectatorListUpdatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateSpectatorListAsync(IList<Player> spectators)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int WritePacket()
        {
            var length = DuelSpectatorListRef.Length;
            var packet = new DuelSpectatorListRef(connection.Output.GetSpan(length)[..length]);
            for (int i = 0; i < spectators.Count; i++)
            {
                var spectator = spectators[i];
                var spectatorStruct = packet[i];
                spectatorStruct.Name = spectator.Name;
            }

            return length;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }
}