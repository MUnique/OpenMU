// <copyright file="AccountRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Linq;
    using BCrypt.Net;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.Persistence.EntityFramework.Json;
    using MUnique.OpenMU.Persistence.EntityFramework.Model;

    /// <summary>
    /// Repository for accounts.
    /// </summary>
    internal class AccountRepository : CachingGenericRepository<Account>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountRepository" /> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        /// <param name="logger">The logger.</param>
        public AccountRepository(CachingRepositoryManager repositoryManager, ILogger<AccountRepository> logger)
            : base(repositoryManager, logger)
        {
        }

        /// <inheritdoc />
        public override Account GetById(Guid id)
        {
            ((CachingRepositoryManager)this.RepositoryManager).EnsureCachesForCurrentGameConfiguration();
            using var context = this.GetContext();
            context.Context.Database.OpenConnection();
            try
            {
                var objectLoader = new AccountJsonObjectLoader();
                var account = objectLoader.LoadObject<Account>(id, context.Context);
                if (account != null && !(context.Context.Entry(account) is { } entry && entry.State != EntityState.Detached))
                {
                    context.Context.Attach(account);
                }

                return account;
            }
            finally
            {
                context.Context.Database.CloseConnection();
            }
        }

        /// <summary>
        /// Gets the account by login name if the password is correct.
        /// </summary>
        /// <param name="loginName">The login name.</param>
        /// <param name="password">The password.</param>
        /// <returns>The account, if the password is correct. Otherwise, null.</returns>
        internal DataModel.Entities.Account GetAccountByLoginName(string loginName, string password)
        {
            using var context = this.GetContext();
            return this.LoadAccountByLoginNameByJsonQuery(loginName, password, context);
        }

        private Account LoadAccountByLoginNameByJsonQuery(string loginName, string password, EntityFrameworkContextBase context)
        {
            var accountInfo = context.Context.Set<Account>()
                .Select(a => new { a.Id, a.LoginName, a.PasswordHash })
                .AsNoTracking()
                .FirstOrDefault(a => a.LoginName == loginName);

            if (accountInfo != null && BCrypt.Verify(password, accountInfo.PasswordHash))
            {
                return this.GetById(accountInfo.Id);
            }

            return null;
        }
    }
}
