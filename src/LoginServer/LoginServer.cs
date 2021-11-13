// <copyright file="LoginServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.LoginServer;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// The login server which keeps track of connected accounts.
/// </summary>
public class LoginServer : ILoginServer
{
    private readonly IDictionary<string, byte> _connectedAccounts = new Dictionary<string, byte>();

    private readonly object _syncRoot = new ();

    /// <inheritdoc/>
    public bool TryLogin(string accountName, byte serverId)
    {
        lock (this._syncRoot)
        {
            if (this._connectedAccounts.ContainsKey(accountName))
            {
                return false;
            }

            this._connectedAccounts.Add(accountName, serverId);
        }

        return true;
    }

    /// <inheritdoc/>
    public void LogOff(string accountName, byte serverId)
    {
        if (accountName != null)
        {
            lock (this._syncRoot)
            {
                this._connectedAccounts.Remove(accountName);
            }
        }
    }
}