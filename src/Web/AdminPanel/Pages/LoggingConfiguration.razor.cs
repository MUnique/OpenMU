// <copyright file="LoggingConfiguration.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// The page which edits the logging configuration of the server.
/// </summary>
public partial class LoggingConfiguration
{
    private LoggingSettings? _settings;
    private string _sectionJson = string.Empty;
    private string? _loadError;
    private bool _showJson;
    private bool _isSaving;

    [Inject]
    private AdminOperationExecutor Executor { get; set; } = null!;

    [Inject]
    private LoggingConfigurationService LoggingConfigurationService { get; set; } = null!;

    [Inject]
    private IToastService ToastService { get; set; } = null!;

    [Inject]
    private ILogger<LoggingConfiguration> Logger { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(true);
        if (AdminPanelEnvironment.IsHostingEmbedded)
        {
            await this.LoadAsync().ConfigureAwait(true);
        }
    }

    private async Task LoadAsync()
    {
        try
        {
            this._settings = await this.LoggingConfigurationService.LoadAsync().ConfigureAwait(true);
            this._sectionJson = this._settings.SectionJson;
            this._loadError = null;
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            this.Logger.LogError(ex, "Couldn't load the logging configuration from {filePath}.", this.LoggingConfigurationService.FilePath);
            this._settings = null;
            this._loadError = ex.Message;
        }
    }

    private async Task SaveAsync()
    {
        if (this._settings is not { } settings)
        {
            return;
        }

        this._isSaving = true;
        try
        {
            if (this._showJson)
            {
                await this.Executor.ExecuteAsync(
                    AdminRoles.Administrator,
                    "logs_set_configuration",
                    new { fullSection = true },
                    async _ =>
                    {
                        await this.LoggingConfigurationService.SaveSectionAsync(this._sectionJson).ConfigureAwait(false);
                        return true;
                    },
                    default).ConfigureAwait(true);
            }
            else if (settings.Overrides.Any(o => string.IsNullOrWhiteSpace(o.Source))
                     || settings.Overrides.GroupBy(o => o.Source.Trim()).Any(g => g.Count() > 1))
            {
                this.ToastService.ShowError(Resources.LogLevelOverridesInvalid);
                return;
            }
            else
            {
                await this.Executor.ExecuteAsync(
                    AdminRoles.Administrator,
                    "logs_set_level",
                    new { settings.DefaultLevel, settings.Overrides },
                    async _ =>
                    {
                        await this.LoggingConfigurationService.SaveMinimumLevelsAsync(settings.DefaultLevel, settings.Overrides).ConfigureAwait(false);
                        return true;
                    },
                    default).ConfigureAwait(true);
            }

            this.ToastService.ShowSuccess(Resources.LoggingConfigurationSaved);
            await this.LoadAsync().ConfigureAwait(true);
        }
        catch (JsonException ex) when (this._showJson)
        {
            this.ToastService.ShowError(string.Format(Resources.LoggingConfigurationInvalidJson, ex.Message));
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            this.Logger.LogError(ex, "Couldn't save the logging configuration to {filePath}.", this.LoggingConfigurationService.FilePath);
            this.ToastService.ShowError(string.Format(Resources.LoggingConfigurationSaveError, ex.Message));
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "The audited logging configuration operation failed.");
            this.ToastService.ShowError(Resources.ResourceManager.GetString("AuditUnavailable", Resources.Culture) ?? Resources.UnexpectedErrorOccurred);
        }
        finally
        {
            this._isSaving = false;
        }
    }
}
