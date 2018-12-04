// <copyright file="IPlayerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Persistence context which is used by in-game players.
    /// </summary>
    public interface IPlayerContext : IContext
    {
        /// <summary>
        /// Gets the letter body by the id of its header.
        /// </summary>
        /// <param name="headerId">The id of its header.</param>
        /// <returns>The body of the header.</returns>
        LetterBody GetLetterBodyByHeaderId(Guid headerId);

        /// <summary>
        /// Determines if the letter can be saved.
        /// </summary>
        /// <param name="letterHeader">The letter header.</param>
        /// <returns><c>True</c>, if successful; Otherwise, <c>false</c>.</returns>
        bool CanSaveLetter(LetterHeader letterHeader);

        /// <summary>
        /// Gets the account by login name if the password is correct.
        /// </summary>
        /// <param name="loginName">The login name.</param>
        /// <param name="password">The password.</param>
        /// <returns>The account, if the password is correct. Otherwise, null.</returns>
        Account GetAccountByLoginName(string loginName, string password);

        /// <summary>
        /// Gets the accounts ordered by login name.
        /// </summary>
        /// <param name="skip">The skip count.</param>
        /// <param name="count">The count.</param>
        /// <returns>The account objects, without depending data.</returns>
        IEnumerable<Account> GetAccountsOrderedByLoginName(int skip, int count);
    }
}