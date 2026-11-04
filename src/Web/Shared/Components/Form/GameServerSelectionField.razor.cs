// <copyright file="GameServerSelectionField.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Form;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Selection field for <see cref="GameLogic.PlugIns.PeriodicTasks.PeriodicTaskConfiguration.AllowedServerIds"/>.
/// Shows every configured game server with a checkbox, loaded live from the
/// database, so the list always reflects the current server setup.
/// An empty selection means the event runs on all servers.
/// </summary>
public partial class GameServerSelectionField
{
    private bool _isLoading = true;

    private bool _loadFailed;

    private IReadOnlyList<GameServerDefinition> _servers = Array.Empty<GameServerDefinition>();

    /// <summary>
    /// Gets or sets the persistence context provider, used to load the game servers.
    /// </summary>
    [Inject]
    public IPersistenceContextProvider ContextProvider { get; set; } = null!;

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [Inject]
    public ILogger<GameServerSelectionField> Logger { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(true);
        await this.LoadServersAsync().ConfigureAwait(true);

        if (this.Value is null)
        {
            this.CurrentValue = new List<byte>();
        }
    }

    private async Task LoadServersAsync()
    {
        try
        {
            using var context = this.ContextProvider.CreateNewConfigurationContext();
            this._servers = (await context.GetAsync<GameServerDefinition>().ConfigureAwait(true))
                .OrderBy(server => server.ServerID)
                .ToList();
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error while loading the game servers for the server selection.");
            this._servers = Array.Empty<GameServerDefinition>();
            this._loadFailed = true;
        }
        finally
        {
            this._isLoading = false;
        }
    }

    private bool IsSelected(byte serverId)
    {
        return this.CurrentValue?.Contains(serverId) == true;
    }

    /// <summary>
    /// Gets the selected server ids which are no longer configured, so they can still
    /// be shown and removed instead of lingering invisibly in the selection.
    /// </summary>
    private IReadOnlyList<byte> GetStaleServerIds()
    {
        return (this.CurrentValue ?? Enumerable.Empty<byte>())
            .Where(id => !this._servers.Any(server => server.ServerID == id))
            .ToList();
    }

    private void OnToggled(byte serverId, bool isSelected)
    {
        var selection = this.CurrentValue?.ToList() ?? new List<byte>();
        if (isSelected && !selection.Contains(serverId))
        {
            selection.Add(serverId);
        }
        else if (!isSelected)
        {
            selection.Remove(serverId);
        }
        else
        {
            // Already selected, nothing to do.
        }

        this.CurrentValue = selection;
    }

    private void SelectAll()
    {
        this.CurrentValue = this._servers.Select(server => server.ServerID).ToList();
    }

    private void ClearSelection()
    {
        this.CurrentValue = new List<byte>();
    }

    private string GetCheckboxId(byte serverId)
    {
        return $"{this.FieldIdentifier.FieldName}_{serverId}";
    }
}
