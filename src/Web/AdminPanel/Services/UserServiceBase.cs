// <copyright file="UserServiceBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Blazored.Modal;
using Blazored.Modal.Services;
using MUnique.OpenMU.Web.AdminPanel.Components.Form;

/// <summary>
/// A basic implementation of a <see cref="IUserService"/> which opens some modal dialogs to retrieve user input.
/// </summary>
public abstract class UserServiceBase : IUserService
{
    private readonly IModalService _modalService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserServiceBase"/> class.
    /// </summary>
    /// <param name="modalService">The modal service.</param>
    protected UserServiceBase(IModalService modalService)
    {
        this._modalService = modalService;
    }

    /// <inheritdoc />
    public abstract bool IsAvailable { get; }

    /// <summary>
    /// Gets the users.
    /// </summary>
    public abstract ICollection<string> Users { get; }

    /// <summary>
    /// Deletes a user with the specified name.
    /// </summary>
    /// <param name="user">The username.</param>
    public abstract Task DeleteUserAsync(string user);

    /// <summary>
    /// Creates a new Account in a modal dialog.
    /// </summary>
    public async Task CreateNewInModalDialogAsync()
    {
        var userParameters = new UserCreationParameters();
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<UserCreationParameters>.Item), userParameters);
        var options = new ModalOptions
        {
            DisableBackgroundCancel = true,
        };

        var modal = this._modalService.Show<ModalCreateNew<UserCreationParameters>>("Create User", parameters, options);
        var result = await modal.Result.ConfigureAwait(false);
        if (!result.Cancelled)
        {
            await this.CreateUserAsync(userParameters.LoginName, userParameters.Password).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Changes the password of a user in a modal dialog.
    /// </summary>
    /// <param name="user">The username.</param>
    public async Task ChangePasswordInModalDialogAsync(string user)
    {
        var userParameters = new UserPasswordChangeParameters();
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<UserPasswordChangeParameters>.Item), userParameters);
        var options = new ModalOptions
        {
            DisableBackgroundCancel = true,
        };

        var modal = this._modalService.Show<ModalCreateNew<UserPasswordChangeParameters>>("Enter the new password", parameters, options);
        var result = await modal.Result.ConfigureAwait(false);
        if (!result.Cancelled)
        {
            await this.ChangePasswordAsync(user, userParameters.Password).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="password">The password.</param>
    protected abstract Task CreateUserAsync(string user, string password);

    /// <summary>
    /// Changes the password.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="newPassword">The new password.</param>
    protected abstract Task ChangePasswordAsync(string user, string newPassword);

    /// <summary>
    /// Parameters for the account creation which is used for the user interface.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Used by data binding.")]
    private class UserCreationParameters
    {
        [Display(Name = "Username")]
        [MaxLength(10)]
        [MinLength(3)]
        [Required]
        public string LoginName { get; set; } = string.Empty;

        [Display(Name = "Password")]
        [MaxLength(100)]
        [MinLength(3)]
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Parameters for the password change which is used for the user interface.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Used by data binding.")]
    private class UserPasswordChangeParameters
    {
        [Display(Name = "Password")]
        [MaxLength(100)]
        [MinLength(3)]
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;
    }
}