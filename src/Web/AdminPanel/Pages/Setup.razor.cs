// <copyright file="Setup.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Components;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// The set up page.
/// </summary>
public partial class Setup
{
    private bool _isDataInitialized;

    private ClientVersion? _gameClientVersion;

    private bool _isImporting;

    private string? _importMessage;

    private string _importMessageCssClass = string.Empty;

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
    /// Gets or sets the backup service.
    /// </summary>
    [Inject]
    public IBackupService BackupService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the javascript runtime.
    /// </summary>
    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        this._isDataInitialized = await this.SetupService.IsDataInitializedAsync().ConfigureAwait(false);
        if (this._isDataInitialized)
        {
            this._gameClientVersion = await this.SetupService.GetCurrentGameClientVersionAsync().ConfigureAwait(false);
        }
    }

    private Task OnUpdateClickAsync()
    {
        return this.SetupService.InstallUpdatesAsync(default);
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

    private async Task OnImportFileChangeAsync(InputFileChangeEventArgs e)
    {
        var file = e.File;
        this._importMessage = null;
        this._isImporting = true;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);

        try
        {
            await using var stream = file.OpenReadStream(maxAllowedSize: long.MaxValue);
            await this.SetupService.CreateDatabaseAsync(
                () => this.BackupService.RestoreBackupAsync(stream)).ConfigureAwait(false);
            this._importMessage = Resources.BackupImportSucceeded;
            this._importMessageCssClass = "text-success";
        }
        catch (Exception ex)
        {
            this._importMessage = $"{Resources.BackupImportFailed} {ex.Message}";
            this._importMessageCssClass = "text-danger";
        }
        finally
        {
            this._isImporting = false;
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
        }
    }
}