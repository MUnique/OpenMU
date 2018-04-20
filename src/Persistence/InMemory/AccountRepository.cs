// <copyright file="AccountRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// In-Memory repository for accounts.
    /// </summary>
    internal class AccountRepository : MemoryRepository<Account>, IAccountRepository
    {
        /// <inheritdoc />
        public DataModel.Entities.Account GetAccountByLoginName(string loginName, string password)
        {
            var account = this.GetAll().FirstOrDefault(a => a.LoginName == loginName && BCrypt.Net.BCrypt.Verify(password, a.PasswordHash));
            return account;
        }

        /// <inheritdoc />
        public IEnumerable<DataModel.Entities.Account> GetAccountsOrderedByLoginName(int skip, int count)
        {
            return this.GetAll().OrderBy(a => a.LoginName).Skip(skip).Take(count);
        }
    }
}