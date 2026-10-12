// <copyright file="DuelRoomManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Threading;
using MUnique.OpenMU.GameLogic.Views.Duel;
using Nito.AsyncEx;

/// <summary>
/// A class that manages several instances of <see cref="DuelRoom"/>.
/// </summary>
public class DuelRoomManager
{
    private readonly DuelConfiguration? _configuration;
    private readonly AsyncLock _lock = new AsyncLock();
    private readonly DuelRoom?[] _duelRooms;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelRoomManager"/> class.
    /// </summary>
    /// <param name="configuration">The configuration; <c>null</c>, if the game configuration doesn't define one.</param>
    public DuelRoomManager(DuelConfiguration? configuration)
    {
        this._configuration = configuration;
        this._duelRooms = new DuelRoom?[this.MaxRoomCount];
    }

    /// <summary>
    /// Gets the maximum <see cref="DuelRoom"/>s count.
    /// </summary>
    public int MaxRoomCount => this._configuration?.DuelAreas.Count ?? 0;

    /// <summary>
    /// Gets a <see cref="DuelRoom"/> for the two duelist players.
    /// </summary>
    /// <param name="player1">The first player.</param>
    /// <param name="player2">The second player.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> with a free <see cref="DuelRoom"/>.</returns>
    public async ValueTask<DuelRoom?> GetFreeDuelRoomAsync(Player player1, Player player2, CancellationToken cancellationToken = default)
    {
        if (this._configuration is not { } configuration)
        {
            return null;
        }

        if (configuration.Variant == DuelVariant.CurrentMap)
        {
            // A duel in the current map doesn't need an area of the duel arena. It also can't be
            // watched, so it doesn't occupy one of the duel channels and isn't limited in count.
            return new DuelRoom(null, player1, player2);
        }

        using var l = await this._lock.LockAsync(cancellationToken).ConfigureAwait(false);
        for (int i = 0; i < this._duelRooms.Length; i++)
        {
            if (this._duelRooms[i] is null)
            {
                var area = configuration.DuelAreas.First(a => a.Index == i);

                return this._duelRooms[i] = new DuelRoom(area, player1, player2);
            }
        }

        return null;
    }

    /// <summary>
    /// Discards a <see cref="DuelRoom"/>.
    /// </summary>
    /// <param name="duelRoom">The <see cref="DuelRoom"/>.</param>
    /// <returns>A <see cref="ValueTask"/>.</returns>
    public async ValueTask GiveBackDuelRoomAsync(DuelRoom duelRoom)
    {
        using var l = await this._lock.LockAsync().ConfigureAwait(false);
        var index = duelRoom.Index;
        if (index >= 0 && index < this._duelRooms.Length && this._duelRooms[index] == duelRoom)
        {
            // Rooms of duels which take place in the current map were never added here,
            // so only the room which actually occupies the slot is given back.
            this._duelRooms[index] = null;
        }
    }

    /// <summary>
    /// Gets a <see cref="DuelRoom"/> by its index number.
    /// </summary>
    /// <param name="requestedDuelIndex">The index of the <see cref="DuelRoom"/>.</param>
    /// <returns>A <see cref="DuelRoom"/>.</returns>
    public DuelRoom? GetRoomByIndex(byte requestedDuelIndex)
    {
        if (this.MaxRoomCount <= requestedDuelIndex)
        {
            return null;
        }

        return this._duelRooms[requestedDuelIndex];
    }

    /// <summary>
    /// Shows the <see cref="DuelRoom"/>s to a player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>A <see cref="ValueTask"/>.</returns>
    public async ValueTask ShowRoomsAsync(Player player)
    {
        await player.InvokeViewPlugInAsync<IDuelStatusUpdatePlugIn>(p => p.UpdateStatusAsync(this._duelRooms)).ConfigureAwait(false);
    }
}