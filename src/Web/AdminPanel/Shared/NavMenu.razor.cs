// <copyright file="NavMenu.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Shared;

using System.Threading;
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
    private IMigratableDatabaseContextProvider PersistenceContextProvider { get; set; } = null!;

    [Inject]
    private SetupService SetupService { get; set; } = null!;

    [Inject]
    private IUserService UserService { get; set; } = null!;

    [Inject]
    private DataUpdateService UpdateService { get; set; } = null!;

    [Inject]
    private NavigationHistory NavigationHistory { get; set; } = null!;

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
        this.UpdateService.UpdatesInstalled -= this.OnUpdatesInstalledAsync;
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);
        this.SetupService.DatabaseInitialized += this.OnDatabaseInitializedAsync;
        this.UpdateService.UpdatesInstalled += this.OnUpdatesInstalledAsync;
        _ = Task.Run(async () =>
        {
            await this.LoadGameConfigurationAsync().ConfigureAwait(false);
            await this.CheckForUpdatesAsync().ConfigureAwait(false);
        });
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
        await this.CheckForUpdatesAsync().ConfigureAwait(false);
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
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            if (!await this.PersistenceContextProvider.CanConnectToDatabaseAsync(cts.Token).ConfigureAwait(true)
                || !await this.PersistenceContextProvider.DatabaseExistsAsync(cts.Token).ConfigureAwait(true))
            {
                return;
            }

            using var context = this.PersistenceContextProvider.CreateNewConfigurationContext();
            this.GameConfigurationId = await context.GetDefaultGameConfigurationIdAsync(cts.Token).ConfigureAwait(true);
        }
        catch
        {
            this.GameConfigurationId = null;
        }
        finally
        {
            this._onlyShowSetup = this.GameConfigurationId is null;
            this._isLoadingConfig = false;
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
        }
    }

    private async Task CheckForUpdatesAsync()
    {
        try
        {
            if (this.GameConfigurationId is null)
            {
                this._availableConfigUpdates = 0;
                return;
            }

            var updates = await this.UpdateService.DetermineAvailableUpdatesAsync().ConfigureAwait(false);
            this._availableConfigUpdates = updates.Count;
        }
        catch
        {
            this._availableConfigUpdates = 0;
        }
        finally
        {
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
        }
    }

    private void ToggleNavMenu()
    {
        this._collapseNavMenu = !this._collapseNavMenu;
    }
}