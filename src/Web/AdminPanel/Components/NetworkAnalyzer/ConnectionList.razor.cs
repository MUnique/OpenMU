// <copyright file="ConnectionList.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.NetworkAnalyzer;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Analyzer;

/// <summary>
/// The list of the connections which can be analyzed, grouped by their server.
/// </summary>
public partial class ConnectionList
{
    private readonly HashSet<(ServerType ServerType, int ServerId)> _collapsedGroups = new();

    private string? _searchTerm;

    /// <summary>
    /// Gets or sets the connections which should be listed.
    /// </summary>
    [Parameter]
    public IReadOnlyList<ICapturedConnectionInfo> Connections { get; set; } = [];

    /// <summary>
    /// Gets or sets the identifier of the currently selected connection.
    /// </summary>
    [Parameter]
    public Guid? SelectedConnectionId { get; set; }

    /// <summary>
    /// Gets or sets the callback which is invoked when a connection is selected.
    /// </summary>
    [Parameter]
    public EventCallback<ICapturedConnectionInfo> OnSelect { get; set; }

    /// <summary>
    /// Gets or sets the callback which is invoked when a connection should be disconnected.
    /// </summary>
    [Parameter]
    public EventCallback<ICapturedConnectionInfo> OnDisconnect { get; set; }

    /// <summary>
    /// Gets or sets the callback which is invoked when the list should be refreshed.
    /// </summary>
    [Parameter]
    public EventCallback OnRefresh { get; set; }

    private string? SearchTerm => this._searchTerm;

    private IEnumerable<IGrouping<(ServerType ServerType, int ServerId), ICapturedConnectionInfo>> FilteredGroups =>
        this.Connections
            .Where(this.MatchesSearchTerm)
            .GroupBy(connection => (connection.ServerType, connection.ServerId))
            .OrderBy(group => group.Key.ServerType)
            .ThenBy(group => group.Key.ServerId);

    private bool MatchesSearchTerm(ICapturedConnectionInfo connection)
    {
        if (string.IsNullOrWhiteSpace(this._searchTerm))
        {
            return true;
        }

        return connection.DisplayName.Contains(this._searchTerm, StringComparison.OrdinalIgnoreCase)
               || connection.RemoteEndPoint?.Contains(this._searchTerm, StringComparison.OrdinalIgnoreCase) is true;
    }

    private bool IsCollapsed((ServerType ServerType, int ServerId) group) => this._collapsedGroups.Contains(group);

    private void ToggleGroup((ServerType ServerType, int ServerId) group)
    {
        if (!this._collapsedGroups.Remove(group))
        {
            this._collapsedGroups.Add(group);
        }
    }

    private void OnSearchChanged(string? searchTerm)
    {
        this._searchTerm = searchTerm;
    }
}
