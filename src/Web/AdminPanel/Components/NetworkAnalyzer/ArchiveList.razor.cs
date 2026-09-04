// <copyright file="ArchiveList.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.NetworkAnalyzer;

using System.Globalization;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Network.Analyzer.Archive;

/// <summary>
/// The list of the archived sessions of the observed accounts, grouped by their account.
/// </summary>
public partial class ArchiveList
{
    private readonly HashSet<string> _collapsedGroups = new();

    /// <summary>
    /// Gets or sets the archived sessions which should be listed.
    /// </summary>
    [Parameter]
    public IReadOnlyList<ArchivedSessionInfo> Sessions { get; set; } = [];

    /// <summary>
    /// Gets or sets the identifier of the currently opened session.
    /// </summary>
    [Parameter]
    public string? SelectedSessionId { get; set; }

    /// <summary>
    /// Gets or sets the route which downloads a session, to which its identifier is appended.
    /// </summary>
    [Parameter]
    public string DownloadRoute { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the callback which is invoked when a session should be opened.
    /// </summary>
    [Parameter]
    public EventCallback<ArchivedSessionInfo> OnSelect { get; set; }

    /// <summary>
    /// Gets or sets the callback which is invoked when a session should be deleted.
    /// </summary>
    [Parameter]
    public EventCallback<ArchivedSessionInfo> OnDelete { get; set; }

    /// <summary>
    /// Gets or sets the callback which is invoked when the list should be refreshed.
    /// </summary>
    [Parameter]
    public EventCallback OnRefresh { get; set; }

    private IEnumerable<IGrouping<string, ArchivedSessionInfo>> AccountGroups =>
        this.Sessions
            .GroupBy(session => session.Metadata.AccountName)
            .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the description of a session, which is shown below its date.
    /// </summary>
    /// <param name="session">The session.</param>
    /// <returns>The description of the session.</returns>
    private static string GetDescription(ArchivedSessionInfo session)
    {
        return string.Create(
            CultureInfo.InvariantCulture,
            $"{session.Metadata.PacketCount} × {FormatDuration(session.Duration)} × {FormatSize(session.SizeInBytes)}");
    }

    /// <summary>
    /// Escapes the identifier of a session, so that it can be used in the url of the download.
    /// </summary>
    /// <param name="sessionId">The identifier of the session.</param>
    /// <returns>The escaped identifier.</returns>
    /// <remarks>
    /// Only the segments are escaped: the separators between the account and the session stay
    /// as they are, so that the route of the download still recognizes them.
    /// </remarks>
    private static string EscapeSessionId(string sessionId)
    {
        return string.Join('/', sessionId.Split('/').Select(Uri.EscapeDataString));
    }

    private static string FormatDuration(TimeSpan duration)
    {
        return duration < TimeSpan.FromHours(1)
            ? duration.ToString(@"mm\:ss", CultureInfo.InvariantCulture)
            : duration.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
    }

    private static string FormatSize(long sizeInBytes)
    {
        return sizeInBytes switch
        {
            < 1024 => string.Create(CultureInfo.InvariantCulture, $"{sizeInBytes} B"),
            < 1024 * 1024 => string.Create(CultureInfo.InvariantCulture, $"{sizeInBytes / 1024.0:F1} KB"),
            _ => string.Create(CultureInfo.InvariantCulture, $"{sizeInBytes / (1024.0 * 1024.0):F1} MB"),
        };
    }

    private bool IsCollapsed(string accountName) => this._collapsedGroups.Contains(accountName);

    private void ToggleGroup(string accountName)
    {
        if (!this._collapsedGroups.Remove(accountName))
        {
            this._collapsedGroups.Add(accountName);
        }
    }
}
