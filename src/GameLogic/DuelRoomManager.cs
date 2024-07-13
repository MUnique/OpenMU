using MUnique.OpenMU.GameLogic.Views.Duel;

namespace MUnique.OpenMU.GameLogic;

using System.Threading;
using Nito.AsyncEx;

public class DuelRoomManager
{
    private readonly DuelConfiguration _configuration;
    private readonly AsyncLock _lock = new AsyncLock();
    private readonly DuelRoom?[] _duelRooms;

    public int MaxRoomCount => this._configuration?.DuelAreas.Count ?? 0;

    public DuelRoomManager(DuelConfiguration configuration)
    {
        this._configuration = configuration;
        this._duelRooms = new DuelRoom?[MaxRoomCount];
    }

    public async ValueTask<DuelRoom?> GetFreeDuelRoomAsync(Player player1, Player player2, CancellationToken cancellationToken = default)
    {
        using var l = await this._lock.LockAsync(cancellationToken);
        for (int i = 0; i < this._duelRooms.Length; i++)
        {
            if (this._duelRooms[i] is null)
            {
                var area = this._configuration.DuelAreas.First(a => a.Index == i);

                return this._duelRooms[i] = new DuelRoom(area, player1, player2);
            }
        }

        return null;
    }

    public async ValueTask GiveBackDuelRoomAsync(DuelRoom duelRoom)
    {
        using var l = await this._lock.LockAsync();
        this._duelRooms[duelRoom.Index] = null;
    }

    public DuelRoom? GetRoomByIndex(byte requestedDuelIndex)
    {
        if (this.MaxRoomCount <= requestedDuelIndex)
        {
            return null;
        }

        return this._duelRooms[requestedDuelIndex];
    }

    public async ValueTask ShowRoomsAsync(Player player)
    {
        await player.InvokeViewPlugInAsync<IDuelStatusUpdatePlugIn>(p => p.UpdateStatusAsync(this._duelRooms)).ConfigureAwait(false);
    }
}