// <copyright file="LoginServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.LoginServer;

using MUnique.OpenMU.Interfaces;
using Nito.AsyncEx;

/// <summary>
/// The login server which keeps track of connected accounts in-memory.
/// </summary>
public class LoginServer : ILoginServer
{
    private readonly Dictionary<string, byte> _connectedAccounts = new();

    private readonly Dictionary<string, Reservation> _reservations = new(StringComparer.OrdinalIgnoreCase);

    private readonly AsyncLock _syncRoot = new();

    private readonly TimeProvider _time;

    /// <summary>Initializes a new instance of the <see cref="LoginServer"/> class.</summary>
    public LoginServer()
        : this(TimeProvider.System)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="LoginServer"/> class with an overridable clock.</summary>
    /// <param name="time">The clock used for administrative reservation expiry.</param>
    public LoginServer(TimeProvider time) => this._time = time;

    /// <inheritdoc />
    public async ValueTask<bool> TryReserveAccountAsync(string accountName, int reservationId)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(reservationId, byte.MaxValue);
        using var gate = await this._syncRoot.LockAsync().ConfigureAwait(false);
        this.RemoveExpiredReservation(accountName);
        if (this._connectedAccounts.ContainsKey(accountName) || (this._reservations.TryGetValue(accountName, out var existing) && existing.Id != reservationId))
        {
            return false;
        }

        this._reservations[accountName] = new Reservation(reservationId, this._time.GetUtcNow().AddMinutes(5));
        return true;
    }

    /// <inheritdoc />
    public async ValueTask ReleaseAccountReservationAsync(string accountName, int reservationId)
    {
        using var gate = await this._syncRoot.LockAsync().ConfigureAwait(false);
        if (this._reservations.TryGetValue(accountName, out var existing) && existing.Id == reservationId)
        {
            this._reservations.Remove(accountName);
        }
    }

    /// <inheritdoc/>
    public async ValueTask<Dictionary<string, byte>> GetSnapshotAsync()
    {
        using var l = await this._syncRoot.LockAsync();
        return new(this._connectedAccounts);
    }

    /// <inheritdoc/>
    public async Task<bool> TryLoginAsync(string accountName, byte serverId)
    {
        using var l = await this._syncRoot.LockAsync();
        this.RemoveExpiredReservation(accountName);
        if (this._connectedAccounts.ContainsKey(accountName) || this._reservations.ContainsKey(accountName))
        {
            return false;
        }

        this._connectedAccounts.Add(accountName, serverId);
        return true;
    }

    /// <inheritdoc/>
    public async ValueTask LogOffAsync(string accountName, byte serverId)
    {
        using var l = await this._syncRoot.LockAsync();

        // A late log off of another server must not remove the current login of the account.
        if (this._connectedAccounts.TryGetValue(accountName, out var currentServerId) && currentServerId == serverId)
        {
            this._connectedAccounts.Remove(accountName);
        }
    }

    private void RemoveExpiredReservation(string accountName)
    {
        if (this._reservations.TryGetValue(accountName, out var reservation) && reservation.ExpiresAt <= this._time.GetUtcNow())
        {
            this._reservations.Remove(accountName);
        }
    }

    private sealed record Reservation(int Id, DateTimeOffset ExpiresAt);
}
