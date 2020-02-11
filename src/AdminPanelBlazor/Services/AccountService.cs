// <copyright file="AccountService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace MUnique.OpenMU.AdminPanelBlazor.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Service for <see cref="Account"/>s.
    /// </summary>
    public class AccountService : IDataService<Account>
    {
        private readonly IPersistenceContextProvider persistenceContextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public AccountService(IPersistenceContextProvider persistenceContextProvider)
        {
            this.persistenceContextProvider = persistenceContextProvider;
        }

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
                using var context = this.persistenceContextProvider.CreateNewPlayerContext(null);
                return Task.FromResult(context.GetAccountsOrderedByLoginName(offset, count).ToList());
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
            using var context = this.persistenceContextProvider.CreateNewPlayerContext(null);
            context.Attach(account);
            account.State = AccountState.Banned;
            context.SaveChanges();
        }

        /// <summary>
        /// Unbans the specified account.
        /// </summary>
        /// <param name="account">The account.</param>
        public void Unban(Account account)
        {
            using var context = this.persistenceContextProvider.CreateNewPlayerContext(null);
            context.Attach(account);
            account.State = AccountState.Normal;
            context.SaveChanges();
        }
    }
}
