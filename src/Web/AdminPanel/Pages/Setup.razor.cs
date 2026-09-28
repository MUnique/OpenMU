// <copyright file="Setup.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.IO;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Components;
using MUnique.OpenMU.Web.AdminPanel.Properties;

/// <summary>
/// The set up page.
/// </summary>
public partial class Setup
{
    private DataInitializationState _dataState;

    private ClientVersion? _gameClientVersion;

    private bool _includeAccounts = true;

    private bool _isImporting;

    private string? _importMessage;

    private bool _showJsonBackup;

    private bool _showSnapshotBackup = true;

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
    /// Gets or sets the database snapshot service. It's only available for a real database.
    /// </summary>
    [Inject]
    public IDatabaseSnapshotService? SnapshotService { get; set; }

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

    private static async Task<MemoryStream> ReadFileAsync(IBrowserFile file)
    {
        // BrowserFileStream doesn't support synchronous reads (which ZipArchive requires),
        // so copy it into a MemoryStream first. Pre-size with file.Size to avoid reallocations.
        var memoryStream = new MemoryStream((int)Math.Min(file.Size, int.MaxValue));
        await using var browserStream = file.OpenReadStream(maxAllowedSize: long.MaxValue);
        await browserStream.CopyToAsync(memoryStream).ConfigureAwait(false);
        memoryStream.Position = 0;
        return memoryStream;
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

    private async Task OnSnapshotFileChangeAsync(InputFileChangeEventArgs e)
    {
        if (this.SnapshotService is not { } snapshotService)
        {
            return;
        }

        this._importMessage = null;
        this._isImporting = true;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);

        try
        {
            using var memoryStream = await ReadFileAsync(e.File).ConfigureAwait(false);
            if (await snapshotService.GetRestoreBlockingReasonAsync(memoryStream).ConfigureAwait(false) is { } blockingReason)
            {
                this._importMessage = blockingReason;
                this._importMessageCssClass = "text-danger";
                return;
            }

            await this.SetupService.RestoreDatabaseAsync(() => snapshotService.RestoreSnapshotAsync(memoryStream)).ConfigureAwait(false);
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

    private async Task OnImportFileChangeAsync(InputFileChangeEventArgs e)
    {
        var file = e.File;
        this._importMessage = null;
        this._isImporting = true;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);

        try
        {
            using var memoryStream = await ReadFileAsync(file).ConfigureAwait(false);
            if (!this.BackupService.ContainsRestorableData(memoryStream))
            {
                this._importMessage = Resources.SelectedFileIsNoBackup;
                this._importMessageCssClass = "text-danger";
                return;
            }

            await this.SetupService.CreateDatabaseAsync(
                () => this.BackupService.RestoreBackupAsync(memoryStream)).ConfigureAwait(false);
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
