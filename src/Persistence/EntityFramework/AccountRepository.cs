// <copyright file="AccountRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System.Linq;
    using BCrypt.Net;
    using Microsoft.EntityFrameworkCore;
    using MUnique.OpenMU.Persistence.EntityFramework.Json;

    /// <summary>
    /// Repository for accounts.
    /// </summary>
    internal class AccountRepository : GenericRepository<Account>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountRepository"/> class.
        /// </summary>
        /// <param name="contextProvider">The context provider.</param>
        public AccountRepository(PersistenceContextProvider contextProvider)
            : base(contextProvider)
        {
        }

        /// <summary>
        /// Gets the account by login name if the password is correct.
        /// </summary>
        /// <param name="loginName">The login name.</param>
        /// <param name="password">The password.</param>
        /// <returns>The account, if the password is correct. Otherwise, null.</returns>
        internal DataModel.Entities.Account GetAccountByLoginName(string loginName, string password)
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

        private Account LoadAccountByLoginNameByJsonQuery(string loginName, string password, EntityFrameworkContext context)
        {
            var accountInfo = context.Context.Set<Account>()
                .Select(a => new { a.Id, a.LoginName, a.PasswordHash })
                .FirstOrDefault(a => a.LoginName == loginName);
            if (accountInfo != null && BCrypt.Verify(password, accountInfo.PasswordHash))
            {
                this.ContextProvider.RepositoryManager.EnsureCachesForCurrentGameConfiguration();

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

            return null;
        }
    }
}
