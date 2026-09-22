// <copyright file="AdminUserManagementService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Components.Form.Modal;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// Manages the users which are allowed to log into the admin panel.
/// </summary>
public class AdminUserManagementService
{
    private readonly UserManager<AdminUser> _userManager;
    private readonly IAdminUserRepository _repository;
    private readonly AuthenticatorSetupService _authenticatorSetupService;
    private readonly AdminUserAvailabilityService _userAvailability;
    private readonly IModalService _modalService;
    private readonly IToastService _toastService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminUserManagementService"/> class.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    /// <param name="repository">The repository of the stored users.</param>
    /// <param name="authenticatorSetupService">The authenticator setup service.</param>
    /// <param name="userAvailability">The service which knows whether any user exists.</param>
    /// <param name="modalService">The modal service.</param>
    /// <param name="toastService">The toast service.</param>
    public AdminUserManagementService(
        UserManager<AdminUser> userManager,
        IAdminUserRepository repository,
        AuthenticatorSetupService authenticatorSetupService,
        AdminUserAvailabilityService userAvailability,
        IModalService modalService,
        IToastService toastService)
    {
        this._userManager = userManager;
        this._repository = repository;
        this._authenticatorSetupService = authenticatorSetupService;
        this._userAvailability = userAvailability;
        this._modalService = modalService;
        this._toastService = toastService;
    }

    /// <summary>
    /// Gets all stored users.
    /// </summary>
    /// <returns>All stored users.</returns>
    public async Task<IList<AdminUser>> GetUsersAsync()
    {
        return await this._repository.GetAllAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new user, asking for its data in a modal dialog.
    /// </summary>
    /// <returns><c>true</c>, if a user has been created; otherwise, <c>false</c>.</returns>
    public async Task<bool> CreateNewInModalDialogAsync()
    {
        var input = new UserCreationParameters();
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<UserCreationParameters>.Item), input);
        var modal = this._modalService.Show<ModalCreateNew<UserCreationParameters>>(Resources.CreateUser, parameters, new ModalOptions { DisableBackgroundCancel = true });
        var result = await modal.Result.ConfigureAwait(false);
        if (result.Cancelled)
        {
            return false;
        }

        var user = new AdminUser
        {
            LoginName = input.LoginName,
            Roles = input.Role.ToString(),
        };

        var identityResult = await this._userManager.CreateAsync(user, input.Password).ConfigureAwait(false);
        if (!identityResult.Succeeded)
        {
            this._toastService.ShowError(string.Join(' ', identityResult.Errors.Select(e => e.Description)));
            return false;
        }

        this._userAvailability.Invalidate();
        this._toastService.ShowSuccess(Resources.UserCreated);
        return true;
    }

    /// <summary>
    /// Changes the password of the specified user, asking for the new one in a modal dialog.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns><c>true</c>, if the password has been changed; otherwise, <c>false</c>.</returns>
    public async Task<bool> ChangePasswordInModalDialogAsync(AdminUser user)
    {
        if (!this.EnsureIsEditable(user))
        {
            return false;
        }

        var input = new PasswordChangeParameters();
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<PasswordChangeParameters>.Item), input);
        var modal = this._modalService.Show<ModalCreateNew<PasswordChangeParameters>>(Resources.ChangePassword, parameters, new ModalOptions { DisableBackgroundCancel = true });
        var result = await modal.Result.ConfigureAwait(false);
        if (result.Cancelled)
        {
            return false;
        }

        var token = await this._userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
        var identityResult = await this._userManager.ResetPasswordAsync(user, token, input.Password).ConfigureAwait(false);
        if (!identityResult.Succeeded)
        {
            this._toastService.ShowError(string.Join(' ', identityResult.Errors.Select(e => e.Description)));
            return false;
        }

        this._toastService.ShowSuccess(Resources.PasswordChanged);
        return true;
    }

    /// <summary>
    /// Assigns the specified role to the specified user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="role">The role.</param>
    public async Task SetRoleAsync(AdminUser user, string role)
    {
        if (!this.EnsureIsEditable(user))
        {
            return;
        }

        user.Roles = role;

        // The claims of running sessions carry the old role, so they have to be invalidated.
        await this._userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes the second factor of the specified user, e.g. when it lost its authenticator app.
    /// </summary>
    /// <param name="user">The user.</param>
    public async Task ResetTwoFactorAsync(AdminUser user)
    {
        if (!this.EnsureIsEditable(user))
        {
            return;
        }

        await this._authenticatorSetupService.DisableAsync(user).ConfigureAwait(false);
        this._toastService.ShowSuccess(Resources.TwoFactorResetForUser);
    }

    /// <summary>
    /// Deletes the specified user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns><c>true</c>, if the user has been deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteAsync(AdminUser user)
    {
        if (!this.EnsureIsEditable(user))
        {
            return false;
        }

        if (await this._repository.GetCountAsync().ConfigureAwait(false) <= 1)
        {
            this._toastService.ShowError(Resources.CannotDeleteLastUser);
            return false;
        }

        var identityResult = await this._userManager.DeleteAsync(user).ConfigureAwait(false);
        if (!identityResult.Succeeded)
        {
            this._toastService.ShowError(string.Join(' ', identityResult.Errors.Select(e => e.Description)));
            return false;
        }

        this._userAvailability.Invalidate();
        this._toastService.ShowSuccess(Resources.UserDeleted);
        return true;
    }

    private bool EnsureIsEditable(AdminUser user)
    {
        if (BootstrapAdminUserProvider.IsBootstrapUser(user))
        {
            this._toastService.ShowError(Resources.CannotModifyBootstrapUser);
            return false;
        }

        return true;
    }

    /// <summary>
    /// The parameters to create a new user.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Used by data binding.")]
    private class UserCreationParameters
    {
        [Display(ResourceType = typeof(Web.Shared.Properties.Resources), Name = nameof(Web.Shared.Properties.Resources.UserCreationParameters_LoginName_Name))]
        [MaxLength(100)]
        [MinLength(3)]
        [Required]
        public string LoginName { get; set; } = string.Empty;

        [Display(ResourceType = typeof(Web.Shared.Properties.Resources), Name = nameof(Web.Shared.Properties.Resources.UserCreationParameters_Password_Name))]
        [MaxLength(100)]
        [MinLength(12)]
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;

        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.Role))]
        [Required]
        public AdminRole Role { get; set; } = AdminRole.Administrator;
    }

    /// <summary>
    /// The parameters to change the password of a user.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Used by data binding.")]
    private class PasswordChangeParameters
    {
        [Display(ResourceType = typeof(Web.Shared.Properties.Resources), Name = nameof(Web.Shared.Properties.Resources.UserCreationParameters_Password_Name))]
        [MaxLength(100)]
        [MinLength(12)]
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;
    }
}
