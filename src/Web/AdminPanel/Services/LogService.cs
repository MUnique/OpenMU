// <copyright file="LogService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.GameLogic;
using System.IO;


public class LogEventData
{
    public DateTime TimeStamp { get; set; }

    public string LoggerName { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string Level { get; set; } = "DEBUG";

    public string? ExceptionString { get; set; }
}

/// <summary>
/// Service which connects to the log hub.
/// </summary>
public class LogService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogService"/> class.
    /// </summary>
    public LogService()
    {
        // todo File.OpenRead("log.txt");
    }

    /// <summary>
    /// Occurs when a log event was received.
    /// </summary>
    public event EventHandler<LogEventData>? LogEventReceived;

    /// <summary>
    /// Gets the known loggers.
    /// </summary>
    public ICollection<string> Loggers { get; private set; } = new List<string>();

    /// <summary>
    /// Gets the captured entries.
    /// </summary>
    public LinkedList<LogEventData> Entries { get; } = new ();

    /// <summary>
    /// Gets or sets an event callback which occurs when the connection state to the hub changed.
    /// </summary>
    public EventCallback<bool>? IsConnectedChanged { get; set; }

    /// <summary>
    /// Gets or sets the maximum size of <see cref="Entries"/>.
    /// </summary>
    public int MaximumEntries { get; set; } = 500;

    private void OnLoggedEvent(LogEventData eventData)
    {
        this.Entries.AddLast(eventData);
        while (this.Entries.Count > this.MaximumEntries)
        {
            this.Entries.RemoveFirst();
        }

        this.LogEventReceived?.Invoke(this, eventData);
    }
}