// <copyright file="NavMenu.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Shared;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Navigation menu of the admin panel.
/// </summary>
public partial class NavMenu
{
    private bool _collapseNavMenu = true;

    private bool _isLoadingConfig = false;

    private bool _onlyShowSetup;

    [Inject]
    private IPersistenceContextProvider PersistenceContextProvider { get; set; } = null!;

    [Inject]
    private SetupService SetupService { get; set; } = null!;

    [Inject]
    private IUserService UserService { get; set; } = null!;

    private GameConfiguration? GameConfiguration { get; set; }

    /// <summary>
    /// Gets the class for the entries of the navigation menu.
    /// "collapse" is a class of bootstrap which hides it.
    /// In our css we also define to show it anyway, if the width sufficient.
    /// </summary>
    private string NavMenuCssClass => this._collapseNavMenu ? "collapse" : string.Empty;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);
        this.SetupService.DatabaseInitialized += this.OnDatabaseInitializedAsync;
        _ = Task.Run(this.LoadGameConfigurationAsync);
    }

    private async ValueTask OnDatabaseInitializedAsync()
    {
        // We have to reload, because the old links are not correct anymore.
        this.GameConfiguration = null;
        await this.LoadGameConfigurationAsync().ConfigureAwait(false);
    }

    private async Task LoadGameConfigurationAsync()
    {
        if (this.GameConfiguration is not null || this._isLoadingConfig)
        {
            return;
        }

        this._isLoadingConfig = true;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);

        try
        {
            using var context = this.PersistenceContextProvider.CreateNewConfigurationContext();
            this.GameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).FirstOrDefault();
        }
        catch
        {
            this.GameConfiguration = null;
        }

        this._onlyShowSetup = this.GameConfiguration is null;
        this._isLoadingConfig = false;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
    }

    private void ToggleNavMenu()
    {
        this._collapseNavMenu = !this._collapseNavMenu;
    }
}