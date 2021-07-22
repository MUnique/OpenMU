// <copyright file="LogController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using MUnique.Log4Net.CoreSignalR;
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// Controller which manages the shown entries on the log page.
    /// </summary>
    public class LogController
    {
        private readonly LogService logService;

        private readonly RingBuffer<LogEventData> entries = new (20);
        private string loggerFilter = string.Empty;
        private string characterFilter = string.Empty;
        private string serverFilter = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogController"/> class.
        /// </summary>
        /// <param name="logService">The log service.</param>
        public LogController(LogService logService)
        {
            this.logService = logService;
            this.UpdateEntries();
            this.logService.LogEventReceived += (sender, e) =>
            {
                if (this.MeetsFilterCriteria(e.Data))
                {
                    this.entries.Add(e.Data);
                    this.EntriesChanged?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        /// <summary>
        /// Occurs when the visible entries changed, e.g. after changing filter settings or new log messages.
        /// </summary>
        public event EventHandler? EntriesChanged;

        /// <summary>
        /// Gets or sets the logger filter.
        /// </summary>
        public string LoggerFilter
        {
            get => this.loggerFilter;
            set
            {
                this.loggerFilter = value;
                this.UpdateEntries();
            }
        }

        /// <summary>
        /// Gets or sets the character filter.
        /// </summary>
        public string CharacterFilter
        {
            get => this.characterFilter;
            set
            {
                this.characterFilter = value;
                this.UpdateEntries();
            }
        }

        /// <summary>
        /// Gets or sets the server filter.
        /// </summary>
        public string ServerFilter
        {
            get => this.serverFilter;
            set
            {
                this.serverFilter = value;
                this.UpdateEntries();
            }
        }

        /// <summary>
        /// Gets the last entries which fit into the filter criteria.
        /// </summary>
        public IEnumerable<LogEventData> Entries => this.entries.GetEnumerable();

        private static string GetProperty(LogEventData logEventData, string name)
        {
            if (logEventData.Properties[name] is string stringProperty)
            {
                return stringProperty;
            }

            if (logEventData.Properties[name] is JsonElement { ValueKind: JsonValueKind.String } jsonElement)
            {
                return jsonElement.GetString() ?? string.Empty;
            }

            return string.Empty;
        }

        private void UpdateEntries()
        {
            this.entries.Clear();
            this.GetLastEntries().Take(this.entries.Size).Reverse().ForEach(this.entries.Add);
            this.EntriesChanged?.Invoke(this, EventArgs.Empty);
        }

        private IEnumerable<LogEventData> GetLastEntries()
        {
            var currentNode = this.logService.Entries.Last;
            while (currentNode != null)
            {
                var logEvent = currentNode.Value;
                if (this.MeetsFilterCriteria(logEvent))
                {
                    yield return logEvent;
                }

                currentNode = currentNode.Previous;
            }
        }

        private bool MeetsFilterCriteria(LogEventData logEvent)
        {
            return (string.IsNullOrEmpty(this.LoggerFilter) || logEvent.LoggerName == this.LoggerFilter)
                && (string.IsNullOrEmpty(this.CharacterFilter) || Equals(GetProperty(logEvent, "character"), this.CharacterFilter))
                && (string.IsNullOrEmpty(this.ServerFilter) || Equals(GetProperty(logEvent, "gameserver"), this.ServerFilter));
        }
    }
}