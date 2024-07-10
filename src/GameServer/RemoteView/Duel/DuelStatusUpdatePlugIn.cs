namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IDuelStatusUpdatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(DuelStatusUpdatePlugIn), "The default implementation of the IDuelStatusUpdatePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("9C37DF93-7E1C-44BB-914D-DB8B3F96FEE0")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
public class DuelStatusUpdatePlugIn : IDuelStatusUpdatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelStatusUpdatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public DuelStatusUpdatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateStatusAsync(DuelRoom?[] duelRooms)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int WritePacket()
        {
            var length = DuelStatusRef.Length;
            var packet = new DuelStatusRef(connection.Output.GetSpan(length)[..length]);
            for (int i = 0; i < duelRooms.Length; i++)
            {
                var duelRoom = duelRooms[i];
                var duelStatus = packet[i];
                if (duelRoom is not null)
                {
                    duelStatus.Player1Name = duelRoom.Requester.Name;
                    duelStatus.Player2Name = duelRoom.Opponent.Name;
                    duelStatus.DuelRunning = duelRoom.State == DuelState.DuelStarted;
                    duelStatus.DuelOpen = duelRoom.IsOpen;
                }
            }

            return length;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }
}