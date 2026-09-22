// <copyright file="Setup.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Components;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// The set up page.
/// </summary>
public partial class Setup
{
    private DataInitializationState _dataState;

    private ClientVersion? _gameClientVersion;

    /// <summary>
    /// Gets or sets a value indicating whether to show the <see cref="Install"/> component.
    /// </summary>
    public bool ShowInstall { get; set; }

    /// <summary>
    /// Gets or sets the setup service.
    /// </summary>
    [Inject]
    public SetupService SetupService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the javascript runtime.
    /// </summary>
    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;

    /// <inheritdoc />
    protected override Task OnInitializedAsync()
    {
        return this.LoadDataStateAsync();
    }

    private async Task LoadDataStateAsync()
    {
        this._dataState = await this.SetupService.GetDataInitializationStateAsync().ConfigureAwait(false);
        this._gameClientVersion = this._dataState == DataInitializationState.Initialized
            ? await this.SetupService.GetCurrentGameClientVersionAsync().ConfigureAwait(false)
            : null;
    }

    private async Task OnInstallationFinishedAsync()
    {
        // We load the state first, so that the page doesn't show the state of the
        // uninitialized database for a moment when it's rendered again.
        await this.LoadDataStateAsync().ConfigureAwait(false);
        this.ShowInstall = false;
    }

    private async Task OnUpdateClickAsync()
    {
        await this.SetupService.InstallUpdatesAsync(default).ConfigureAwait(false);

        // Before the update, the state could not be determined on the outdated schema.
        await this.LoadDataStateAsync().ConfigureAwait(false);
    }

    private void OnInstallClick()
    {
        this.ShowInstall = true;
    }

    private async Task OnReInstallClickAsync()
    {
        if (await this.JsRuntime.InvokeAsync<bool>("confirm", Resources.ReinstallConfirmation).ConfigureAwait(false))
        {
            this.ShowInstall = true;
        }
    }
}