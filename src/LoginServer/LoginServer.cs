// <copyright file="LoginServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.LoginServer;

using Nito.AsyncEx;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// The login server which keeps track of connected accounts in-memory.
/// </summary>
public class LoginServer : ILoginServer
{
    private readonly Dictionary<string, byte> _connectedAccounts = new();

    private readonly AsyncLock _syncRoot = new ();

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
        if (this._connectedAccounts.ContainsKey(accountName))
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
        this._connectedAccounts.Remove(accountName);
    }
}