// <copyright file="LoginServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.LoginServer
{
    using System.Collections.Generic;

    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// The login server which keeps track of connected accounts.
    /// </summary>
    public class LoginServer : ILoginServer
    {
        private readonly IDictionary<string, byte> connectedAccounts = new Dictionary<string, byte>();

        private readonly object syncRoot = new ();

        /// <inheritdoc/>
        public bool TryLogin(string accountName, byte serverId)
        {
            lock (this.syncRoot)
            {
                if (this.connectedAccounts.ContainsKey(accountName))
                {
                    return false;
                }

                this.connectedAccounts.Add(accountName, serverId);
            }

            return true;
        }

        /// <inheritdoc/>
        public void LogOff(string accountName, byte serverId)
        {
            if (accountName != null)
            {
                lock (this.syncRoot)
                {
                    this.connectedAccounts.Remove(accountName);
                }
            }
        }
    }
}
