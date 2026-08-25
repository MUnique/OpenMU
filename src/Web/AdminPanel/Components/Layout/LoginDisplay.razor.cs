// <copyright file="LoginDisplay.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Layout;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Pages;

/// <summary>
/// Shows the currently signed in user and allows to sign out.
/// </summary>
public partial class LoginDisplay : IAsyncDisposable
{
    private IJSObjectReference? _authModule;

    [Inject]
    private AdminAuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

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

    private void OpenAccountSecurity()
    {
        this.NavigationManager.NavigateTo(AdminAuthenticationDefaults.SecurityPath.TrimStart('/'));
    }

    /// <summary>
    /// Sends the user to the page where it can create the first user.
    /// </summary>
    /// <remarks>
    /// As long as no user exists, the panel is reachable without a login. There is no own account
    /// to manage then, so the account security page would be empty - creating a user is what
    /// actually needs to happen.
    /// </remarks>
    private void OpenUserCreation()
    {
        this.NavigationManager.NavigateTo(AdminUsers.CreateUserUrl);
    }

    private async Task LogoutAsync()
    {
        if (this._authModule is { } module)
        {
            await module.InvokeVoidAsync("signOut").ConfigureAwait(true);
        }

        this.AuthenticationStateProvider.NotifySignedOut();
        this.NavigationManager.NavigateTo(AdminAuthenticationDefaults.LoginPath.TrimStart('/'));
    }
}
