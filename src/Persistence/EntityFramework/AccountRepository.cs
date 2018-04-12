// <copyright file="AccountRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System.Collections.Generic;
    using System.Linq;
    using BCrypt.Net;
    using Microsoft.EntityFrameworkCore;
    using MUnique.OpenMU.Persistence.EntityFramework.Json;

    /// <summary>
    /// Repository for accounts.
    /// </summary>
    internal class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountRepository"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public AccountRepository(IRepositoryManager manager)
            : base(manager)
        {
        }

        /// <inheritdoc/>
        public DataModel.Entities.Account GetAccountByLoginName(string loginName, string password)
        {
            using (var context = this.GetContext())
            {
                var account = this.LoadAccountByLoginNameByJsonQuery(loginName, password, context);
                if (account != null)
                {
                    context.Context.Attach(account);
                }

                return account;
            }
        }

        /// <inheritdoc />
        public IEnumerable<DataModel.Entities.Account> GetAccountsOrderedByLoginName(int skip, int count)
        {
            using (var context = this.GetContext())
            {
                var accounts = context.Context.Set<Account>()
                    .OrderBy(a => a.LoginName)
                    .Skip(skip)
                    .Take(count);
                return accounts.ToList();
            }
        }

        private Account LoadAccountByLoginNameByJsonQuery(string loginName, string password, EntityFrameworkContext context)
        {
            if (this.RepositoryManager is RepositoryManager manager)
            {
                var accountInfo = context.Context.Set<Account>()
                    .Select(a => new { a.Id, a.LoginName, a.PasswordHash })
                    .FirstOrDefault(a => a.LoginName == loginName && BCrypt.Verify(password, a.PasswordHash, false));
                if (accountInfo != null)
                {
                    manager.EnsureCachesForCurrentGameConfiguration();

                    context.Context.Database.OpenConnection();
                    try
                    {
                        var objectLoader = new AccountJsonObjectLoader();
                        return objectLoader.LoadObject<Account>(accountInfo.Id, context.Context);
                    }
                    finally
                    {
                        context.Context.Database.CloseConnection();
                    }
                }
            }

            return null;
        }
    }
}
