// <copyright file="CashShopCoins.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// The page which shows and grants the cash shop coins of an account.
/// </summary>
public partial class CashShopCoins : ComponentBase
{
    private readonly GrantInput _input = new();
    private CashShopCoinSummary? _summary;
    private bool _isLoading = true;
    private bool _isGranting;

    /// <summary>
    /// Gets or sets the login name of the account.
    /// </summary>
    [Parameter]
    public string LoginName { get; set; } = string.Empty;

    [Inject]
    private IPersistenceContextProvider PersistenceContextProvider { get; set; } = null!;

    [Inject]
    private IDataSource<GameConfiguration> GameConfigurationSource { get; set; } = null!;

    [Inject]
    private IToastService ToastService { get; set; } = null!;

    [Inject]
    private ILogger<CashShopCoins> Logger { get; set; } = null!;

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationState { get; set; }

    private CashShopCoinService CoinService => new(this.PersistenceContextProvider, this.GameConfigurationSource);

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await this.LoadAsync().ConfigureAwait(true);
        await base.OnParametersSetAsync().ConfigureAwait(true);
    }

    private static int GetBalance(CashShopCoinSummary summary, CashShopCoinType coinType)
    {
        return coinType switch
        {
            CashShopCoinType.WCoinC => summary.WCoinC,
            CashShopCoinType.WCoinP => summary.WCoinP,
            _ => summary.GoblinPoints,
        };
    }

    private string GetPendingText(CashShopCoinSummary summary, CashShopCoinType coinType)
    {
        var pending = summary.Grants.Where(g => g.CoinType == coinType && g.AppliedAt is null).Sum(g => (long)g.Amount);
        return pending == 0 ? string.Empty : string.Format(Resources.CashShopPendingAmount, pending.ToString("+#,0;-#,0"));
    }

    private async Task LoadAsync()
    {
        this._isLoading = true;
        try
        {
            this._summary = await this.CoinService.GetSummaryAsync(this.LoginName).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error loading the cash shop coins of account {LoginName}.", this.LoginName);
            this.ToastService.ShowError(string.Format(Resources.UnexpectedErrorOccurred, ex.Message));
        }
        finally
        {
            this._isLoading = false;
        }
    }

    private async Task OnGrantAsync()
    {
        if (this._input.Amount == 0)
        {
            this.ToastService.ShowError(Resources.CashShopAmountMustNotBeZero);
            return;
        }

        this._isGranting = true;
        try
        {
            var user = this.AuthenticationState is { } state ? (await state.ConfigureAwait(true)).User.Identity?.Name : null;
            var (status, _) = await this.CoinService
                .GrantAsync(this.LoginName, this._input.CoinType, this._input.Amount, this._input.Reason, null, user)
                .ConfigureAwait(true);
            if (status == CashShopCoinGrantStatus.Created)
            {
                this.Logger.LogInformation("Admin {User} granted {Amount} {CoinType} to account {LoginName}.", user, this._input.Amount, this._input.CoinType, this.LoginName);
                this.ToastService.ShowSuccess(Resources.CashShopCoinsGranted);
                this._input.Amount = 0;
                this._input.Reason = null;
            }
            else
            {
                this.ToastService.ShowError(Resources.CashShopCoinsAccountNotFound);
            }

            await this.LoadAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error granting cash shop coins to account {LoginName}.", this.LoginName);
            this.ToastService.ShowError(string.Format(Resources.UnexpectedErrorOccurred, ex.Message));
        }
        finally
        {
            this._isGranting = false;
        }
    }

    /// <summary>
    /// The input of a grant.
    /// </summary>
    private sealed class GrantInput
    {
        /// <summary>
        /// Gets or sets the coin type.
        /// </summary>
        public CashShopCoinType CoinType { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        [MaxLength(API.CashShopCoinGrantRequest.ReasonMaximumLength)]
        public string? Reason { get; set; }
    }
}
