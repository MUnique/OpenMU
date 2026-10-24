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

    private readonly Dictionary<string, int> _reservations = new(StringComparer.OrdinalIgnoreCase);

    private readonly AsyncLock _syncRoot = new();

    /// <inheritdoc />
    public async ValueTask<bool> TryReserveAccountAsync(string accountName, int reservationId)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(reservationId, byte.MaxValue);
        using var gate = await this._syncRoot.LockAsync().ConfigureAwait(false);
        if (this._connectedAccounts.ContainsKey(accountName) || (this._reservations.TryGetValue(accountName, out var existing) && existing != reservationId))
        {
            return false;
        }

        this._reservations[accountName] = reservationId;
        return true;
    }

    /// <inheritdoc />
    public async ValueTask ReleaseAccountReservationAsync(string accountName, int reservationId)
    {
        using var gate = await this._syncRoot.LockAsync().ConfigureAwait(false);
        if (this._reservations.TryGetValue(accountName, out var existing) && existing == reservationId)
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
}