// <copyright file="AccountModule.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence;
    using Nancy;
    using Nancy.ModelBinding;

    /// <summary>
    /// <see cref="NancyModule"/> for all account related actions.
    /// </summary>
    public class AccountModule : NancyModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AccountModule));
        private readonly IPersistenceContextProvider persistenceContextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountModule"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public AccountModule(IPersistenceContextProvider persistenceContextProvider)
            : base("/admin/account")
        {
            this.persistenceContextProvider = persistenceContextProvider;

            this.Post["save"] = this.SaveAccount;
            this.Post["delete/{accountId:string}"] = this.DeleteAccount;
            this.Get["list/{offset:int}/{count:int}"] = this.GetAccounts;
        }

        private object SaveAccount(dynamic parameters)
        {
            byte[] body = new byte[this.Request.Body.Length];
            this.Request.Body.Position = 0;
            this.Request.Body.Read(body, 0, body.Length);
            var bodyString = System.Text.Encoding.UTF8.GetString(body);
            Log.Debug(bodyString);

            try
            {
                using (var configContext = this.persistenceContextProvider.CreateNewConfigurationContext())
                {
                    var configuration = configContext.Get<GameConfiguration>().FirstOrDefault();
                    using (var context = this.persistenceContextProvider.CreateNewPlayerContext(configuration))
                    {
                        var dto = this.Bind<AccountDto>();
                        Account account = dto.Id == Guid.Empty
                            ? context.CreateNew<Account>()
                            : context.GetById<Account>(dto.Id);

                        this.AssignAccountValues(account, dto);

                        context.SaveChanges();
                        return account.GetId();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return false;
            }
        }

        private void AssignAccountValues(Account account, AccountDto dto)
        {
            account.EMail = dto.EMail;
            account.State = dto.State;
            account.LoginName = dto.LoginName;
            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            }
        }

        private object DeleteAccount(dynamic parameters)
        {
            var accountId = this.ExtractAccountId(parameters);
            try
            {
                using (var context = this.persistenceContextProvider.CreateNewContext())
                {
                    var success = context.Delete(accountId);

                    return success && context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return false;
            }
        }

        private Guid ExtractAccountId(dynamic parameters)
        {
            var accountIdString = (string)parameters.accountId;
            Guid.TryParse(accountIdString, out Guid result);
            return result;
        }

        private List<AccountDto> GetAccounts(dynamic parameters)
        {
            var offset = (int)parameters.offset;
            var count = (int)parameters.count;
            using (var context = this.persistenceContextProvider.CreateNewPlayerContext(null))
            {
                var accounts = context.GetAccountsOrderedByLoginName(offset, count)
                    .Select(a => new AccountDto
                    {
                        Id = a.GetId(),
                        LoginName = a.LoginName,
                        EMail = a.EMail,
                        State = a.State,
                    })
                    .ToList();

                return accounts;
            }
        }

        /// <summary>
        /// Data transfer object for <see cref="Account"/>.
        /// </summary>
        public class AccountDto
        {
            /// <summary>
            /// Gets or sets the identifier of the account.
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            /// Gets or sets the e mail address of the account.
            /// </summary>
            public string EMail { get; set; }

            /// <summary>
            /// Gets or sets the name of the login of the account.
            /// </summary>
            /// <value>
            /// The name of the login.
            /// </value>
            public string LoginName { get; set; }

            /// <summary>
            /// Gets or sets the new password of a changed account.
            /// </summary>
            public string NewPassword { get; set; }

            /// <summary>
            /// Gets or sets the state of the account.
            /// </summary>
            public AccountState State { get; set; }
        }
    }
}