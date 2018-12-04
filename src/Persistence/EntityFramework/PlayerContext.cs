// <copyright file="PlayerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Persistence context which is used by in-game players.
    /// </summary>
    internal class PlayerContext : EntityFrameworkContext, IPlayerContext
    {
        /// <summary>
        /// The logger of this class.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(PlayerContext));

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerContext"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contextProvider">The contextProvider.</param>
        public PlayerContext(DbContext context, PersistenceContextProvider contextProvider)
            : base(context, contextProvider)
        {
        }

        /// <inheritdoc/>
        public DataModel.Entities.LetterBody GetLetterBodyByHeaderId(Guid headerId)
        {
            using (this.ContextProvider.UseContext(this))
            {
                var repository = this.ContextProvider.RepositoryManager.GetRepository<LetterBody>() as LetterBodyRepository;
                return repository?.GetBodyByHeaderId(headerId);
            }
        }

        /// <inheritdoc/>
        public bool CanSaveLetter(Interfaces.LetterHeader letterHeader)
        {
            if (!(letterHeader is LetterHeader persistentHeader))
            {
                Log.Error($"Letter header is not of persistent type {typeof(LetterHeader)}.");
                return false;
            }

            persistentHeader.Receiver = this.Context.Set<Character>().FirstOrDefault(c => c.Name == letterHeader.ReceiverName);
            return persistentHeader.Receiver != null;
        }

        /// <inheritdoc />
        public DataModel.Entities.Account GetAccountByLoginName(string loginName, string password)
        {
            using (this.ContextProvider.UseContext(this))
            {
                var repository = this.ContextProvider.RepositoryManager.GetRepository<Account>() as AccountRepository;
                return repository?.GetAccountByLoginName(loginName, password);
            }
        }

        /// <inheritdoc />
        public IEnumerable<DataModel.Entities.Account> GetAccountsOrderedByLoginName(int skip, int count)
        {
            using (this.ContextProvider.UseContext(this))
            {
                return this.Context.Set<Account>().OrderBy(a => a.LoginName).Skip(skip).Take(count).ToList();
            }
        }
    }
}