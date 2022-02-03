// <copyright file="LogEntryReceivedEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Event args for a received log entry.
/// </summary>
/// <seealso cref="EventArgs" />
public class LogEntryReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntryReceivedEventArgs"/> class.
    /// </summary>
    /// <param name="data">The log event data.</param>
    public LogEntryReceivedEventArgs(string data)
    {
        this.Data = data;
    }

    /// <summary>
    /// Gets the log event data.
    /// </summary>
    public string Data { get; }
}