// <copyright file="ILogHubClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Collections.Generic;
    using log4net.SignalR;

    /// <summary>
    /// Interface for clients of the <see cref="SignalrAppenderHubBase{TClient}"/>.
    /// </summary>
    /// <seealso cref="log4net.SignalR.ISignalrAppenderHubClient" />
    public interface ILogHubClient : ISignalrAppenderHubClient
    {
        /// <summary>
        /// Initializes the client with the available loggers and cached log entries.
        /// </summary>
        /// <param name="loggers">The loggers.</param>
        /// <param name="cachedEntries">The cached entries.</param>
        void Initialize(IList<string> loggers, IList<LogEntry> cachedEntries);
    }
}
