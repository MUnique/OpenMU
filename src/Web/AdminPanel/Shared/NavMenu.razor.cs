// <copyright file="NavMenu.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Shared;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Services;
using Nito.AsyncEx.Synchronous;

/// <summary>
/// Navigation menu of the admin panel.
/// </summary>
public partial class NavMenu
{
    private bool _collapseNavMenu = true;

    [Inject]
    private IPersistenceContextProvider PersistenceContextProvider { get; set; } = null!;

    [Inject]
    private SetupService SetupService { get; set; } = null!;

    private GameConfiguration? GameConfiguration { get; set; }

    private bool OnlyShowSetup { get; set; }

    /// <summary>
    /// Gets the class for the entries of the navigation menu.
    /// "collapse" is a class of bootstrap which hides it.
    /// In our css we also define to show it anyway, if the width sufficient.
    /// </summary>
    private string NavMenuCssClass => this._collapseNavMenu ? "collapse" : string.Empty;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.LoadGameConfiguration();
        this.SetupService.DatabaseInitialized += this.OnDatabaseInitialized;
    }

    private void OnDatabaseInitialized(object? sender, EventArgs args)
    {
        // We have to reload, because the old links are not correct anymore.
        this.InvokeAsync(() =>
        {
            this.LoadGameConfiguration();
            this.StateHasChanged();
        }).WaitAndUnwrapException();
    }

    private void LoadGameConfiguration()
    {
        using var context = this.PersistenceContextProvider.CreateNewConfigurationContext();
        this.GameConfiguration = context.Get<GameConfiguration>().FirstOrDefault();
        this.OnlyShowSetup = this.GameConfiguration is null;
    }

    private void ToggleNavMenu()
    {
        this._collapseNavMenu = !this._collapseNavMenu;
    }
}