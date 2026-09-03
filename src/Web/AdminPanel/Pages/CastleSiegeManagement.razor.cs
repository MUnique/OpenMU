// <copyright file="CastleSiegeManagement.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.Web.AdminPanel.Services;
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
    private string _ownerGuildName = string.Empty;
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

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        this._servers = this.ManagementService.GetAvailableGameServers();
        this._selectedGameServerId = this._servers.FirstOrDefault()?.Id;
        await this.RefreshAsync().ConfigureAwait(false);
    }

    private async Task OnServerChangedAsync(ChangeEventArgs eventArgs)
    {
        this._selectedGameServerId = int.TryParse(eventArgs.Value?.ToString(), out var gameServerId)
            ? gameServerId
            : null;
        await this.RefreshAsync().ConfigureAwait(false);
    }

    private async Task RefreshAsync()
    {
        if (this._selectedGameServerId is not { } gameServerId)
        {
            this._snapshot = null;
            this._statusMessage = null;
            return;
        }

        try
        {
            var result = await this.ManagementService.GetSnapshotAsync(gameServerId).ConfigureAwait(false);
            this._snapshot = result.Snapshot;
            this._statusMessage = result.ErrorMessage;
            if (result.Snapshot is { } snapshot)
            {
                this._forcedState = snapshot.State;
                this._chaosTax = snapshot.ChaosTax;
                this._storeTax = snapshot.StoreTax;
                this._huntTax = snapshot.HuntTax;
            }
        }
        catch (Exception exception)
        {
            this._snapshot = null;
            this._statusMessage = exception.Message;
        }
    }

    private Task ForceStateAsync()
    {
        return this.RunOperationAsync(
            () => this.ManagementService.ForceStateAsync(this._selectedGameServerId!.Value, this._forcedState),
            "The Castle Siege state transition has been scheduled.");
    }

    private Task SetOwnerAsync()
    {
        return this.RunOperationAsync(
            () => this.ManagementService.SetOwnerAsync(this._selectedGameServerId!.Value, this._ownerGuildName),
            "The Castle Siege owner has been updated.");
    }

    private Task ResetCycleAsync()
    {
        return this.RunOperationAsync(
            () => this.ManagementService.ResetCycleAsync(this._selectedGameServerId!.Value),
            "Castle Siege registrations were cleared and Idle1 was scheduled.");
    }

    private Task SaveTaxesAsync()
    {
        return this.RunOperationAsync(
            () => this.ManagementService.SetTaxesAsync(this._selectedGameServerId!.Value, this._chaosTax, this._storeTax, this._huntTax),
            "Castle Siege taxes have been updated.");
    }

    private Task ClearTributeAsync()
    {
        return this.RunOperationAsync(
            () => this.ManagementService.ClearTributeAsync(this._selectedGameServerId!.Value),
            "Castle Siege tribute has been cleared.");
    }

    private Task RemoveRegistrationAsync(Guid guildId)
    {
        return this.RunOperationAsync(
            () => this.ManagementService.RemoveRegistrationAsync(this._selectedGameServerId!.Value, guildId),
            "The guild registration has been removed.");
    }

    private async Task RunOperationAsync(
        Func<ValueTask<CastleSiegeAdministrationResult>> operation,
        string successMessage)
    {
        this._isWorking = true;
        try
        {
            var result = await operation().ConfigureAwait(false);
            if (result.IsSuccess)
            {
                this.ToastService.ShowSuccess(successMessage);
            }
            else
            {
                this.ToastService.ShowError(result.ErrorMessage!);
            }
        }
        catch (Exception exception)
        {
            this.ToastService.ShowError(exception.Message);
        }
        finally
        {
            this._isWorking = false;
            await this.RefreshAsync().ConfigureAwait(false);
        }
    }

    private string FormatRemainingTime(DateTime endTimeUtc)
    {
        var remaining = endTimeUtc - DateTime.UtcNow;
        return remaining > TimeSpan.Zero
            ? remaining.ToString("g")
            : "Elapsed";
    }
}
