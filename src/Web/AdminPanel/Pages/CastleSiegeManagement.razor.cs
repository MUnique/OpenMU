// <copyright file="CastleSiegeManagement.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Displays and administers the local Castle Siege runtime.
/// </summary>
public partial class CastleSiegeManagement : ComponentBase
{
    private IReadOnlyList<CastleSiegeManagementGameServer> _servers = [];
    private CastleSiegeAdministrationSnapshot? _snapshot;
    private string? _statusMessage;
    private int? _selectedGameServerId;
    private CastleSiegeState _forcedState;
    private byte _chaosTax;
    private byte _storeTax;
    private int _huntTax;
    private bool _isWorking;

    /// <summary>
    /// Gets or sets the Castle Siege management service.
    /// </summary>
    [Inject]
    public CastleSiegeManagementService ManagementService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the toast service.
    /// </summary>
    [Inject]
    public IToastService ToastService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the modal service.
    /// </summary>
    [Inject]
    public IModalService ModalService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [Inject]
    public ILogger<CastleSiegeManagement> Logger { get; set; } = null!;

    /// <summary>
    /// Gets or sets the guild name entered in the owner form.
    /// </summary>
    public string OwnerGuildName { get; set; } = string.Empty;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        this._servers = this.ManagementService.AvailableGameServers;
        this._selectedGameServerId = this._servers.FirstOrDefault()?.Id;
        await this.RefreshAsync().ConfigureAwait(true);
    }

    private static string? GetErrorMessage(CastleSiegeAdministrationError error)
    {
        return error switch
        {
            CastleSiegeAdministrationError.None => null,
            CastleSiegeAdministrationError.InvalidState => Resources.CastleSiegeErrorInvalidState,
            CastleSiegeAdministrationError.NotInitialized => Resources.CastleSiegeErrorNotInitialized,
            CastleSiegeAdministrationError.GuildNameRequired => Resources.CastleSiegeErrorGuildNameRequired,
            CastleSiegeAdministrationError.GameServerContextRequired => Resources.CastleSiegeErrorGameServerContextRequired,
            CastleSiegeAdministrationError.GuildNotFound => Resources.CastleSiegeErrorGuildNotFound,
            CastleSiegeAdministrationError.OwnerChangeDuringBattle => Resources.CastleSiegeErrorOwnerChangeDuringBattle,
            CastleSiegeAdministrationError.ResetDuringActiveSiege => Resources.CastleSiegeErrorResetDuringActiveSiege,
            CastleSiegeAdministrationError.TaxOutOfRange => Resources.CastleSiegeErrorTaxOutOfRange,
            CastleSiegeAdministrationError.TaxChangeDuringBattle => Resources.CastleSiegeErrorTaxChangeDuringBattle,
            CastleSiegeAdministrationError.TributeClearDuringBattle => Resources.CastleSiegeErrorTributeClearDuringBattle,
            CastleSiegeAdministrationError.RegistrationChangeOutsideRegistration => Resources.CastleSiegeErrorRegistrationChangeOutsideRegistration,
            CastleSiegeAdministrationError.RegistrationMissing => Resources.CastleSiegeErrorRegistrationMissing,
            CastleSiegeAdministrationError.GameServerUnavailable => Resources.CastleSiegeErrorGameServerUnavailable,
            CastleSiegeAdministrationError.AllInOneDeploymentRequired => Resources.CastleSiegeErrorAllInOneDeploymentRequired,
            CastleSiegeAdministrationError.PlugInInactive => Resources.CastleSiegeErrorPlugInInactive,
            _ => Resources.CastleSiegeErrorUnexpected,
        };
    }

    private async Task OnServerChangedAsync(ChangeEventArgs eventArgs)
    {
        this._selectedGameServerId = int.TryParse(eventArgs.Value?.ToString(), out var gameServerId)
            ? gameServerId
            : null;
        await this.RefreshAsync().ConfigureAwait(true);
    }

    private async Task RefreshAsync()
    {
        if (this._isWorking)
        {
            return;
        }

        this._isWorking = true;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
        try
        {
            await this.RefreshCoreAsync().ConfigureAwait(true);
        }
        finally
        {
            this._isWorking = false;
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
        }
    }

    private async Task RefreshCoreAsync()
    {
        if (this._selectedGameServerId is not { } gameServerId)
        {
            this._snapshot = null;
            this._statusMessage = null;
            return;
        }

        try
        {
            var result = await this.ManagementService.GetSnapshotAsync(gameServerId).ConfigureAwait(true);
            this._statusMessage = GetErrorMessage(result.Error);
            this._snapshot = result.Snapshot;
            if (this._statusMessage is null && this._snapshot is { } snapshot)
            {
                this._forcedState = snapshot.State;
                this._chaosTax = snapshot.ChaosTax;
                this._storeTax = snapshot.StoreTax;
                this._huntTax = snapshot.HuntTax;
            }
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, "The Castle Siege administration snapshot could not be refreshed.");
            this._snapshot = null;
            this._statusMessage = Resources.CastleSiegeErrorUnexpected;
        }
    }

    private Task ForceStateAsync()
    {
        return this.RunOperationAsync(
            () => this.ManagementService.ForceStateAsync(this._selectedGameServerId!.Value, this._forcedState),
            Resources.CastleSiegeStateScheduled);
    }

    private async Task SetOwnerAsync()
    {
        if (!string.IsNullOrWhiteSpace(this.OwnerGuildName)
            && !await this.ModalService.ShowQuestionAsync(
                    Resources.CastleSiegeConfirmOwnerTitle,
                    string.Format(Resources.CastleSiegeConfirmOwner, this.OwnerGuildName.Trim()))
                .ConfigureAwait(true))
        {
            return;
        }

        await this.RunOperationAsync(
            () => this.ManagementService.SetOwnerAsync(this._selectedGameServerId!.Value, this.OwnerGuildName),
            Resources.CastleSiegeOwnerUpdated).ConfigureAwait(true);
    }

    private async Task ResetCycleAsync()
    {
        if (!await this.ModalService.ShowQuestionAsync(
                Resources.CastleSiegeConfirmResetTitle,
                Resources.CastleSiegeConfirmReset)
            .ConfigureAwait(true))
        {
            return;
        }

        await this.RunOperationAsync(
            () => this.ManagementService.ResetCycleAsync(this._selectedGameServerId!.Value),
            Resources.CastleSiegeCycleReset).ConfigureAwait(true);
    }

    private Task SaveTaxesAsync()
    {
        return this.RunOperationAsync(
            () => this.ManagementService.SetTaxesAsync(this._selectedGameServerId!.Value, this._chaosTax, this._storeTax, this._huntTax),
            Resources.CastleSiegeTaxesUpdated);
    }

    private async Task ClearTributeAsync()
    {
        if (!await this.ModalService.ShowQuestionAsync(
                Resources.CastleSiegeConfirmClearTributeTitle,
                Resources.CastleSiegeConfirmClearTribute)
            .ConfigureAwait(true))
        {
            return;
        }

        await this.RunOperationAsync(
            () => this.ManagementService.ClearTributeAsync(this._selectedGameServerId!.Value),
            Resources.CastleSiegeTributeCleared).ConfigureAwait(true);
    }

    private async Task RemoveRegistrationAsync(Guid guildId, string guildName)
    {
        if (!await this.ModalService.ShowQuestionAsync(
                Resources.CastleSiegeConfirmRemoveRegistrationTitle,
                string.Format(Resources.CastleSiegeConfirmRemoveRegistration, guildName))
            .ConfigureAwait(true))
        {
            return;
        }

        await this.RunOperationAsync(
            () => this.ManagementService.RemoveRegistrationAsync(this._selectedGameServerId!.Value, guildId),
            Resources.CastleSiegeRegistrationRemoved).ConfigureAwait(true);
    }

    private async Task RunOperationAsync(
        Func<ValueTask<CastleSiegeAdministrationResult>> operation,
        string successMessage)
    {
        if (this._isWorking)
        {
            return;
        }

        this._isWorking = true;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
        try
        {
            var result = await operation().ConfigureAwait(true);
            if (result.IsSuccess)
            {
                this.ToastService.ShowSuccess(successMessage);
            }
            else
            {
                this.ToastService.ShowError(GetErrorMessage(result.Error) ?? Resources.CastleSiegeErrorUnexpected);
            }
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, "A Castle Siege administration operation failed.");
            this.ToastService.ShowError(Resources.CastleSiegeErrorUnexpected);
        }
        finally
        {
            await this.RefreshCoreAsync().ConfigureAwait(true);
            this._isWorking = false;
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
        }
    }
}
