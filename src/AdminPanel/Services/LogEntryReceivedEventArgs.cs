// <copyright file="LogEntryReceivedEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services
{
    using System;
    using MUnique.Log4Net.CoreSignalR;

    /// <summary>
    /// Event args for a received <see cref="LogEventData"/>.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class LogEntryReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntryReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="data">The log event data.</param>
        public LogEntryReceivedEventArgs(LogEventData data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Gets the log event data.
        /// </summary>
        public LogEventData Data { get; }
    }
}