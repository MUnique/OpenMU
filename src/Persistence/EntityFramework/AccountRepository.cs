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
    internal class AccountRepository : GenericRepository<Account>, IAccountRepository<Account>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountRepository"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public AccountRepository(IRepositoryManager manager)
            : base(manager)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to load accoutns as json.
        /// Currently, that's not fully working, so default is false.
        /// </summary>
        public bool LoadAsJson { get; set; }

        /// <inheritdoc/>
        public Account GetAccountByLoginName(string loginName, string password)
        {
            using (var context = this.GetContext())
            {
                if (this.LoadAsJson)
                {
                    var accountByJson = this.LoadAccountByLoginNameByJsonQuery(loginName, password, context);
                    context.Context.Entry(accountByJson).State = EntityState.Unchanged;
                    context.Context.ChangeTracker.DetectChanges();
                    context.Context.ChangeTracker.AcceptAllChanges();
                    return accountByJson;
                }

                var account = context.Context.Set<Account>()
                    .Include(a => a.RawVault) // TODO: Check if items are loaded as well when loading dependent data
                    .FirstOrDefault(a => a.LoginName == loginName && BCrypt.Verify(password, a.PasswordHash, false));
                if (account != null)
                {
                    this.LoadDependentData(account, context.Context);
                }

                //// context.Context.ChangeTracker.DetectChanges();
                //// context.Context.ChangeTracker.AcceptAllChanges();
                return account;
            }
        }

        private Account LoadAccountByLoginNameByJsonQuery(string loginName, string password, EntityFrameworkContext context)
        {
            var manager = this.RepositoryManager as RepositoryManager;
            if (manager != null)
            {
                var accountInfo = context.Context.Set<Account>()
                    .Select(a => new {a.Id, a.LoginName, a.PasswordHash})
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
