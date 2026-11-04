// <copyright file="Install.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Properties;

/// <summary>
/// The component which allows to initialize the database.
/// </summary>
public sealed partial class Install
{
    /// <summary>
    /// Gets or sets the selected version.
    /// </summary>
    public IDataInitializationPlugIn? SelectedVersion { get; set; }

    /// <summary>
    /// Gets or sets the game server count.
    /// </summary>
    public int GameServerCount { get; set; } = 2;

    /// <summary>
    /// Gets or sets a value indicating whether to create test accounts.
    /// </summary>
    public bool CreateTestAccounts { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance is installing.
    /// </summary>
    public bool IsInstalling { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance has installed.
    /// </summary>
    public bool IsInstalled { get; private set; }

    /// <summary>
    /// Gets the error message of the last failed installation.
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Gets or sets the installation finished callback.
    /// </summary>
    [Parameter]
    public EventCallback InstallationFinished { get; set; }

    /// <summary>
    /// Gets or sets the setup service.
    /// </summary>
    [Inject]
    public SetupService SetupService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the server provider.
    /// </summary>
    [Inject]
    public IServerProvider ServerProvider { get; set; } = null!;

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [Inject]
    public ILogger<Install> Logger { get; set; } = null!;

    private int CurrentConnections => this.ServerProvider.Servers.Where(s => s.ServerState != ServerState.Timeout).Sum(s => s.CurrentConnections);

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        this.SelectedVersion = this.SetupService.Versions.First();
    }

    /// <summary>
    /// Starts the installation.
    /// </summary>
    private async Task StartInstallationAsync()
    {
        this.ErrorMessage = null;
        this.IsInstalling = true;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
        try
        {
            await this.SetupService.CreateDatabaseAsync(() => this.SelectedVersion!.CreateInitialDataAsync((byte)this.GameServerCount, this.CreateTestAccounts)).ConfigureAwait(false);
            this.IsInstalled = true;
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error during the installation of the database.");
            this.ErrorMessage = string.Format(Resources.UnexpectedErrorCheckLogs, ex.Message);
        }
        finally
        {
            this.IsInstalling = false;
        }
    }

    private void OnSelectVersion(string key)
    {
        this.SelectedVersion = this.SetupService.Versions.First(v => v.Key == key);
    }

    private void OnGameServerCountChange(ChangeEventArgs obj)
    {
        if (obj.Value is string strValue
            && int.TryParse(strValue, out var count))
        {
            this.GameServerCount = count;
        }
    }

    private void OnTestAccountsChange(ChangeEventArgs obj)
    {
        if (obj.Value is bool value)
        {
            this.CreateTestAccounts = value;
        }
    }
}