// <copyright file="AccountRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System.Collections;
    using System.Linq;
    using BCrypt.Net;
    using Microsoft.EntityFrameworkCore;

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

        /// <inheritdoc/>
        public Account GetAccountByLoginName(string loginName, string password)
        {
            using (var context = this.GetContext())
            {
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
    }
}
