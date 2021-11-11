// <copyright file="LogService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MUnique.Log4Net.CoreSignalR;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Service which connects to the log hub.
/// </summary>
public class LogService
{
    private readonly HubConnection _connection;

    private long _idOfLastMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogService"/> class.
    /// </summary>
    public LogService()
    {
        this._connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:1234/signalr/hubs/logHub")
            .Build();

        this._connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000).ConfigureAwait(false);
            await this.Connect().ConfigureAwait(false);
        };

        this._connection.On<string, LogEventData, long>("OnLoggedEvent", this.OnLoggedEvent);
        this._connection.On<string[], LogEntry[]>("Initialize", this.OnInitialize);

        Task EnsureConnected()
        {
            if (this._connection.State == HubConnectionState.Disconnected)
            {
                return this.Connect();
            }

            return Task.CompletedTask;
        }

        this.Initialization = EnsureConnected();
    }

    /// <summary>
    /// Occurs when a log event was received.
    /// </summary>
    public event EventHandler<LogEntryReceivedEventArgs>? LogEventReceived;

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
    /// Gets a value indicating whether this instance is connected to the log hub.
    /// </summary>
    public bool IsConnected => this._connection.State == HubConnectionState.Connected;

    /// <summary>
    /// Gets the initialization task.
    /// </summary>
    public Task Initialization { get; }

    /// <summary>
    /// Gets or sets the maximum size of <see cref="Entries"/>.
    /// </summary>
    public int MaximumEntries { get; set; } = 500;

    private void OnInitialize(string[] loggers, LogEntry[] cachedEntries)
    {
        this.Loggers = loggers.ToList();
        cachedEntries.Select(entry => entry.LoggingEvent).ForEach(e => this.Entries.AddLast(e));
    }

    private async Task Connect()
    {
        await this._connection.StartAsync();
        await this._connection.InvokeAsync("SubscribeToGroupWithMessageOffset", "MyGroup", this._idOfLastMessage);
        var isConnectedChanged = this.IsConnectedChanged;
        if (isConnectedChanged != null)
        {
            await isConnectedChanged.Value.InvokeAsync(this.IsConnected);
        }
    }

    private void OnLoggedEvent(string formattedEvent, LogEventData entry, long id)
    {
        this._idOfLastMessage = id;
        this.Entries.AddLast(entry);
        while (this.Entries.Count > this.MaximumEntries)
        {
            this.Entries.RemoveFirst();
        }

        this.LogEventReceived?.Invoke(this, new LogEntryReceivedEventArgs(entry));
    }
}