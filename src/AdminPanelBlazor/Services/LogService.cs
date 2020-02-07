// <copyright file="LogService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanelBlazor.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.SignalR.Client;
    using MUnique.Log4Net.CoreSignalR;

    public class LogService
    {
        private HubConnection connection;

        public LogService()
        {
            this.connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:1234/signalr/hubs/logHub")
                .Build();

            this.connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await this.Connect();
            };
            this.connection.On<LogEntry>(nameof(ILogHubClient.OnLoggedEvent), this.OnLoggedEvent);
            this.connection.On<string[], LogEntry[]>(nameof(ILogHubClient.Initialize), this.OnInitialize);

            Task EnsureConnected()
            {
                if (this.connection.State == HubConnectionState.Disconnected)
                {
                    return this.Connect();
                }

                return Task.CompletedTask;
            }

            this.Initialization = EnsureConnected();
        }

        public ICollection<string> Loggers { get; private set; }

        public LinkedList<LogEntry> Entries { get; private set; }

        public EventCallback<LinkedList<LogEntry>>? EntriesChanged;

        public EventCallback<bool>? IsConnectedChanged;

        public bool IsConnected => this.connection.State == HubConnectionState.Connected;

        public Task Initialization { get; }

        public int MaximumEntries { get; set; } = 500;

        private void OnInitialize(string[] loggers, LogEntry[] cachedEntries)
        {
            this.Loggers = loggers.ToList();
            this.Entries = new LinkedList<LogEntry>(cachedEntries);
        }

        private long idOfLastMessage;

        private async Task Connect()
        {
            await this.connection.StartAsync();
            await this.connection.InvokeAsync(nameof(LogHub.SubscribeToGroupWithMessageOffset), "MyGroup", this.idOfLastMessage);
            await this.IsConnectedChanged?.InvokeAsync(this.IsConnected);
        }

        private Task OnLoggedEvent(LogEntry entry)
        {
            this.idOfLastMessage = entry.Id;
            this.Entries.AddLast(entry);
            while (this.Entries.Count > this.MaximumEntries)
            {
                this.Entries.RemoveFirst();
            }

            return this.EntriesChanged?.InvokeAsync(this.Entries) ?? Task.CompletedTask;
        }
    }
}
