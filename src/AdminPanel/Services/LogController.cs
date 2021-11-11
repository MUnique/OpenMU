// <copyright file="LogController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services;

using System.Text.Json;
using MUnique.Log4Net.CoreSignalR;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Controller which manages the shown entries on the log page.
/// </summary>
public class LogController
{
    private readonly LogService _logService;

    private readonly RingBuffer<LogEventData> _entries = new (20);
    private string _loggerFilter = string.Empty;
    private string _characterFilter = string.Empty;
    private string _serverFilter = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogController"/> class.
    /// </summary>
    /// <param name="logService">The log service.</param>
    public LogController(LogService logService)
    {
        this._logService = logService;
        this.UpdateEntries();
        this._logService.LogEventReceived += (sender, e) =>
        {
            if (this.MeetsFilterCriteria(e.Data))
            {
                this._entries.Add(e.Data);
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
        get => this._loggerFilter;
        set
        {
            this._loggerFilter = value;
            this.UpdateEntries();
        }
    }

    /// <summary>
    /// Gets or sets the character filter.
    /// </summary>
    public string CharacterFilter
    {
        get => this._characterFilter;
        set
        {
            this._characterFilter = value;
            this.UpdateEntries();
        }
    }

    /// <summary>
    /// Gets or sets the server filter.
    /// </summary>
    public string ServerFilter
    {
        get => this._serverFilter;
        set
        {
            this._serverFilter = value;
            this.UpdateEntries();
        }
    }

    /// <summary>
    /// Gets the last entries which fit into the filter criteria.
    /// </summary>
    public IEnumerable<LogEventData> Entries => this._entries.GetEnumerable();

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
        this._entries.Clear();
        this.GetLastEntries().Take(this._entries.Size).Reverse().ForEach(this._entries.Add);
        this.EntriesChanged?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerable<LogEventData> GetLastEntries()
    {
        var currentNode = this._logService.Entries.Last;
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