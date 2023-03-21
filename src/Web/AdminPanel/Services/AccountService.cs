// <copyright file="AccountService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Blazored.Modal;
using Blazored.Modal.Services;
using MUnique.OpenMU.Web.AdminPanel.Components.Form;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Service for <see cref="Account"/>s.
/// </summary>
public class AccountService : IDataService<Account>, ISupportDataChangedNotification
{
    private readonly IDataSource<Account> _dataSource;
    private readonly IModalService _modalService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountService"/> class.
    /// </summary>
    /// <param name="dataSource">The player context.</param>
    /// <param name="modalService">The modal service.</param>
    public AccountService(IDataSource<Account> dataSource, IModalService modalService)
    {
        this._dataSource = dataSource;
        this._modalService = modalService;
    }

    /// <inheritdoc />
    public event EventHandler? DataChanged;

    /// <summary>
    /// Returns a slice of the account list, defined by an offset and a count.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="count">The count.</param>
    /// <returns>A slice of the account list, defined by an offset and a count.</returns>
    public async Task<List<Account>> GetAsync(int offset, int count)
    {
        try
        {
            var playerContext = (IPlayerContext) await this._dataSource.GetContextAsync().ConfigureAwait(false);
            return (await playerContext.GetAccountsOrderedByLoginNameAsync(offset, count).ConfigureAwait(false)).ToList();
        }
        catch
        {
            return new List<Account>();
        }
    }

    /// <summary>
    /// Bans the specified account.
    /// </summary>
    /// <param name="account">The account.</param>
    public async ValueTask BanAsync(Account account)
    {
        account.State = AccountState.Banned;
        var context = await this._dataSource.GetContextAsync().ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Unbans the specified account.
    /// </summary>
    /// <param name="account">The account.</param>
    public async ValueTask UnbanAsync(Account account)
    {
        account.State = AccountState.Normal;
        var context = await this._dataSource.GetContextAsync().ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new Account in a modal dialog.
    /// </summary>
    public async Task CreateNewInModalDialogAsync()
    {
        var accountParameters = new AccountCreationParameters();
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<AccountCreationParameters>.Item), accountParameters);
        var options = new ModalOptions
        {
            DisableBackgroundCancel = true,
        };

        var modal = this._modalService.Show<ModalCreateNew<AccountCreationParameters>>($"Create {nameof(Account)}", parameters, options);
        var result = await modal.Result.ConfigureAwait(false);
        if (!result.Cancelled)
        {
            var context = await this._dataSource.GetContextAsync().ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
            var item = context.CreateNew<Account>();
            item.LoginName = accountParameters.LoginName;
            item.PasswordHash = BCrypt.Net.BCrypt.HashPassword(accountParameters.Password);
            item.EMail = accountParameters.EMail;
            item.State = accountParameters.State;
            item.SecurityCode = accountParameters.SecurityCode;
            item.RegistrationDate = DateTime.UtcNow;
            await context.SaveChangesAsync().ConfigureAwait(false);
            this.RaiseDataChanged();
        }
    }

    private void RaiseDataChanged() => this.DataChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Parameters for the account creation which is used for the user interface.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Used by data binding.")]
    private class AccountCreationParameters
    {
        [Display(Name = "Login Name")]
        [MaxLength(10)]
        [MinLength(3)]
        [Required]
        public string LoginName { get; set; } = string.Empty;

        [Display(Name = "Password")]
        [MaxLength(20)]
        [MinLength(3)]
        [Required]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Security Code")]
        [MaxLength(10)]
        [MinLength(3)]
        [Required]
        public string SecurityCode { get; set; } = string.Empty;

        [Display(Name = "E-Mail")]
        public string EMail { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public AccountState State { get; set; }
    }
}