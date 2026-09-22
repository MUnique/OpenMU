// <copyright file="AccountSecurity.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// The page at which a user manages the second factor of its own account.
/// </summary>
public partial class AccountSecurity : IAsyncDisposable
{
    private AdminUser? _user;
    private AuthenticatorSetup? _setup;
    private IReadOnlyList<string>? _recoveryCodes;
    private string _confirmationCode = string.Empty;
    private string? _errorMessage;
    private bool _isBusy;
    private bool _isLoading = true;
    private bool _isTwoFactorRequired;
    private int _remainingRecoveryCodes;
    private IJSObjectReference? _authModule;

    [Inject]
    private CurrentAdminUserService CurrentUserService { get; set; } = null!;

    [Inject]
    private AuthenticatorSetupService SetupService { get; set; } = null!;

    [Inject]
    private AdminLoginService LoginService { get; set; } = null!;

    [Inject]
    private AdminAuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    [Inject]
    private IOptions<AdminPanelAuthOptions> AuthOptions { get; set; } = null!;

    [Inject]
    private IToastService ToastService { get; set; } = null!;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Inject]
    private ILogger<AccountSecurity> Logger { get; set; } = null!;

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (this._authModule is { } module)
        {
            this._authModule = null;
            try
            {
                await module.DisposeAsync().ConfigureAwait(false);
            }
            catch (JSDisconnectedException)
            {
                // The circuit is already gone.
            }
        }

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(true);
        this._isTwoFactorRequired = this.AuthOptions.Value.RequireTwoFactor;
        await this.LoadUserAsync().ConfigureAwait(true);
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(true);
        if (firstRender)
        {
            this._authModule = await this.JsRuntime
                .InvokeAsync<IJSObjectReference>("import", AdminAuthenticationDefaults.AuthScriptPath)
                .ConfigureAwait(true);
        }
    }

    private async Task LoadUserAsync()
    {
        try
        {
            this._user = await this.CurrentUserService.GetCurrentUserAsync().ConfigureAwait(true);
            if (this._user is { IsTwoFactorEnabled: true })
            {
                this._remainingRecoveryCodes = await this.SetupService
                    .GetRemainingRecoveryCodeCountAsync(this._user)
                    .ConfigureAwait(true);
            }
        }
        finally
        {
            this._isLoading = false;
        }
    }

    private async Task BeginSetupAsync()
    {
        if (this._user is not { } user)
        {
            return;
        }

        this._isBusy = true;
        this._errorMessage = null;
        this._recoveryCodes = null;
        try
        {
            this._setup = await this.SetupService.BeginSetupAsync(user).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "The authenticator setup could not be started.");
            this._errorMessage = Resources.UnhandledErrorOccurred;
        }
        finally
        {
            this._isBusy = false;
        }
    }

    private void CancelSetup()
    {
        this._setup = null;
        this._confirmationCode = string.Empty;
        this._errorMessage = null;
    }

    private async Task ConfirmSetupAsync()
    {
        if (this._user is not { } user || this._setup is null)
        {
            return;
        }

        this._isBusy = true;
        this._errorMessage = null;
        try
        {
            var codes = await this.SetupService.ConfirmSetupAsync(user, this._confirmationCode).ConfigureAwait(true);
            if (codes is null)
            {
                this._errorMessage = Resources.InvalidTwoFactorCode;
                return;
            }

            this._setup = null;
            this._confirmationCode = string.Empty;
            this._recoveryCodes = codes;
            this._remainingRecoveryCodes = codes.Count;

            // Enabling the second factor rotated the security stamp, so the running session needs a fresh cookie.
            await this.RefreshSessionAsync(user, usedSecondFactor: true).ConfigureAwait(true);
            this.ToastService.ShowSuccess(Resources.TwoFactorEnabled);
        }
        finally
        {
            this._isBusy = false;
        }
    }

    private async Task DisableTwoFactorAsync()
    {
        if (this._user is not { } user)
        {
            return;
        }

        this._isBusy = true;
        try
        {
            await this.SetupService.DisableAsync(user).ConfigureAwait(true);
            this._recoveryCodes = null;
            this._remainingRecoveryCodes = 0;
            await this.RefreshSessionAsync(user, usedSecondFactor: false).ConfigureAwait(true);
            this.ToastService.ShowSuccess(Resources.TwoFactorDisabled);
        }
        finally
        {
            this._isBusy = false;
        }
    }

    private async Task GenerateRecoveryCodesAsync()
    {
        if (this._user is not { } user)
        {
            return;
        }

        this._isBusy = true;
        try
        {
            this._recoveryCodes = await this.SetupService.GenerateRecoveryCodesAsync(user).ConfigureAwait(true);
            this._remainingRecoveryCodes = this._recoveryCodes.Count;
        }
        finally
        {
            this._isBusy = false;
        }
    }

    /// <summary>
    /// Exchanges the authentication cookie for one which carries the updated claims.
    /// </summary>
    private async Task RefreshSessionAsync(AdminUser user, bool usedSecondFactor)
    {
        if (this._authModule is not { } module)
        {
            return;
        }

        var (ticket, claims) = this.LoginService.IssueSessionTicket(user, usedSecondFactor);
        if (await module.InvokeAsync<bool>("signIn", ticket).ConfigureAwait(true))
        {
            this.AuthenticationStateProvider.NotifySignedIn(claims);
        }
        else
        {
            this.Logger.LogWarning("The session of user '{LoginName}' could not be refreshed after a security change.", user.LoginName);
        }
    }
}
