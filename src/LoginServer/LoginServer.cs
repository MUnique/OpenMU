// <copyright file="LoginServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading;

namespace MUnique.OpenMU.LoginServer;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// The login server which keeps track of connected accounts in-memory.
/// </summary>
public class LoginServer : ILoginServer
{
    private readonly IDictionary<string, byte> _connectedAccounts = new Dictionary<string, byte>();

    private readonly SemaphoreSlim _syncRoot = new(1);

    /// <inheritdoc/>
    public async Task<bool> TryLogin(string accountName, byte serverId)
    {
        await this._syncRoot.WaitAsync();
        try
        {
            if (this._connectedAccounts.ContainsKey(accountName))
            {
                return false;
            }

            this._connectedAccounts.Add(accountName, serverId);
            return true;
        }
        finally
        {
            this._syncRoot.Release();
        }
    }

    /// <inheritdoc/>
    public void LogOff(string accountName, byte serverId)
    {
        this._syncRoot.Wait();
        try
        {
            this._connectedAccounts.Remove(accountName);
        }
        finally
        {
            this._syncRoot.Release();
        }
    }
}