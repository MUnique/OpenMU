// <copyright file="AdminUsers.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// The page which manages the users of the admin panel.
/// </summary>
public partial class AdminUsers
{
    private IList<AdminUser> _users = new List<AdminUser>();
    private bool _isLoading = true;

    [Inject]
    private AdminUserManagementService UserManagementService { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(true);
        await this.ReloadAsync().ConfigureAwait(true);
    }

    private async Task ReloadAsync()
    {
        this._isLoading = true;
        try
        {
            this._users = await this.UserManagementService.GetUsersAsync().ConfigureAwait(true);
        }
        finally
        {
            this._isLoading = false;
        }
    }

    private async Task OnCreateNewAsync()
    {
        if (await this.UserManagementService.CreateNewInModalDialogAsync().ConfigureAwait(true))
        {
            await this.ReloadAsync().ConfigureAwait(true);
        }
    }

    private async Task OnChangePasswordAsync(AdminUser user)
    {
        await this.UserManagementService.ChangePasswordInModalDialogAsync(user).ConfigureAwait(true);
    }

    private async Task OnResetTwoFactorAsync(AdminUser user)
    {
        await this.UserManagementService.ResetTwoFactorAsync(user).ConfigureAwait(true);
        await this.ReloadAsync().ConfigureAwait(true);
    }

    private async Task OnDeleteAsync(AdminUser user)
    {
        if (await this.UserManagementService.DeleteAsync(user).ConfigureAwait(true))
        {
            await this.ReloadAsync().ConfigureAwait(true);
        }
    }

    private async Task OnRoleChangedAsync(AdminUser user, string? role)
    {
        if (string.IsNullOrEmpty(role) || role == user.Roles)
        {
            return;
        }

        await this.UserManagementService.SetRoleAsync(user, role).ConfigureAwait(true);
        await this.ReloadAsync().ConfigureAwait(true);
    }
}
