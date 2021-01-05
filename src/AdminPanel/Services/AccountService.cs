// <copyright file="AccountService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Blazored.Modal;
    using Blazored.Modal.Services;
    using MUnique.OpenMU.AdminPanel.Components.Form;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Service for <see cref="Account"/>s.
    /// </summary>
    public class AccountService : IDataService<Account>, ISupportDataChangedNotification
    {
        private readonly IPlayerContext playerContext;
        private readonly IModalService modalService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>
        /// <param name="playerContext">The player context.</param>
        /// <param name="modalService">The modal service.</param>
        public AccountService(IPlayerContext playerContext, IModalService modalService)
        {
            this.playerContext = playerContext;
            this.modalService = modalService;
        }

        /// <inheritdoc />
        public event EventHandler? DataChanged;

        /// <summary>
        /// Returns a slice of the account list, defined by an offset and a count.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>A slice of the account list, defined by an offset and a count.</returns>
        public Task<List<Account>> Get(int offset, int count)
        {
            try
            {
                return Task.FromResult(this.playerContext.GetAccountsOrderedByLoginName(offset, count).ToList());
            }
            catch (NotImplementedException)
            {
                return Task.FromResult(new List<Account>());
            }
        }

        /// <summary>
        /// Bans the specified account.
        /// </summary>
        /// <param name="account">The account.</param>
        public void Ban(Account account)
        {
            account.State = AccountState.Banned;
            this.playerContext.SaveChanges();
        }

        /// <summary>
        /// Unbans the specified account.
        /// </summary>
        /// <param name="account">The account.</param>
        public void Unban(Account account)
        {
            account.State = AccountState.Normal;
            this.playerContext.SaveChanges();
        }

        /// <summary>
        /// Creates a new Account in a modal dialog.
        /// </summary>
        public async Task CreateNewInModalDialog()
        {
            var accountParameters = new AccountCreationParameters();
            var parameters = new ModalParameters();
            parameters.Add(nameof(ModalCreateNew<AccountCreationParameters>.Item), accountParameters);
            var options = new ModalOptions
            {
                DisableBackgroundCancel = true,
            };

            var modal = this.modalService.Show<ModalCreateNew<AccountCreationParameters>>($"Create {nameof(Account)}", parameters, options);
            var result = await modal.Result;
            if (!result.Cancelled)
            {
                var item = this.playerContext.CreateNew<Account>();
                item.LoginName = accountParameters.LoginName;
                item.PasswordHash = BCrypt.Net.BCrypt.HashPassword(accountParameters.Password);
                item.EMail = accountParameters.EMail;
                item.State = accountParameters.State;
                item.SecurityCode = accountParameters.SecurityCode;
                item.RegistrationDate = DateTime.Now;
                this.playerContext.SaveChanges();
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
}
