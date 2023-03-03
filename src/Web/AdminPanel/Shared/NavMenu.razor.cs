// <copyright file="NavMenu.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Shared;

using Microsoft.AspNetCore.Components;

using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Navigation menu of the admin panel.
/// </summary>
public partial class NavMenu : IDisposable
{
    private bool _collapseNavMenu = true;

    private bool _isLoadingConfig = false;

    private bool _onlyShowSetup;

    private int _availableConfigUpdates;

    [Inject]
    private IPersistenceContextProvider PersistenceContextProvider { get; set; } = null!;

    [Inject]
    private SetupService SetupService { get; set; } = null!;

    [Inject]
    private IUserService UserService { get; set; } = null!;

    [Inject]
    private DataUpdateManager UpdateManager { get; set; } = null!;

    private Guid? GameConfigurationId { get; set; }

    /// <summary>
    /// Gets the class for the entries of the navigation menu.
    /// "collapse" is a class of bootstrap which hides it.
    /// In our css we also define to show it anyway, if the width sufficient.
    /// </summary>
    private string NavMenuCssClass => this._collapseNavMenu ? "collapse" : string.Empty;

    /// <inheritdoc />
    public void Dispose()
    {
        this.SetupService.DatabaseInitialized -= this.OnDatabaseInitializedAsync;
        this.UpdateManager.UpdatesInstalled -= this.OnUpdatesInstalledAsync;
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);
        this.SetupService.DatabaseInitialized += this.OnDatabaseInitializedAsync;
        this.UpdateManager.UpdatesInstalled += this.OnUpdatesInstalledAsync;
        _ = Task.Run(this.LoadGameConfigurationAsync);
        _ = Task.Run(this.CheckForUpdatesAsync);
    }

    private async ValueTask OnUpdatesInstalledAsync()
    {
        this._availableConfigUpdates = 0;
        _ = Task.Run(this.CheckForUpdatesAsync);
    }

    private async ValueTask OnDatabaseInitializedAsync()
    {
        // We have to reload, because the old links are not correct anymore.
        this.GameConfigurationId = null;
        await this.LoadGameConfigurationAsync().ConfigureAwait(false);
    }

    private async Task LoadGameConfigurationAsync()
    {
        if (this.GameConfigurationId is not null || this._isLoadingConfig)
        {
            return;
        }

        this._isLoadingConfig = true;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);

        try
        {
            using var context = this.PersistenceContextProvider.CreateNewConfigurationContext();
            this.GameConfigurationId = await context.GetDefaultGameConfigurationIdAsync();
        }
        catch
        {
            this.GameConfigurationId = null;
        }

        this._onlyShowSetup = this.GameConfigurationId is null;
        this._isLoadingConfig = false;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
    }

    private async Task CheckForUpdatesAsync()
    {
        try
        {
            var updates = await this.UpdateManager.DetermineAvailableUpdatesAsync().ConfigureAwait(false);
            this._availableConfigUpdates = updates.Count;
        }
        catch
        {
            this._availableConfigUpdates = 0;
        }

        await this.InvokeAsync(this.StateHasChanged);
    }

    private void ToggleNavMenu()
    {
        this._collapseNavMenu = !this._collapseNavMenu;
    }
}